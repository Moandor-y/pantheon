using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

namespace Pantheon {
  public class AoeMarker : MonoBehaviour {
    public enum Shape {
      Round,
      Rectangle,
    }

    public struct RoundParams {
      public float Radius;
      public float InnerRadius;
      public float Angle;
    }

    public struct RectangleParams {
      public float Length;
      public float Width;
    }

    public struct ShapeParams : INetworkSerializeByMemcpy {
      public Shape Shape;
      public RoundParams RoundParams;
      public RectangleParams RectangleParams;
    }

    public Color TintColor {
      get { return _material.GetColor("_Color"); }

      set { _material.SetColor("_Color", value); }
    }

    public bool Visible {
      get { return _projector.gameObject.activeSelf; }

      set { _projector.gameObject.SetActive(value); }
    }

    public float Duration;

    [SerializeField]
    private Projector _projector;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private Material _roundMaterial;

    [SerializeField]
    private Material _rectangleMaterial;

    private Material _material;

    private float _prevAngle;
    private float _prevOpacity = -1;
    private float _prevInnerRadius = -1;

    private Shape _shape = Shape.Round;

    public void UpdateShape(ShapeParams shapeParams) {
      switch (shapeParams.Shape) {
        case Shape.Round:
          SetRound(shapeParams.RoundParams.Radius, shapeParams.RoundParams.InnerRadius,
                   shapeParams.RoundParams.Angle);
          break;
        case Shape.Rectangle:
          SetRectangle(shapeParams.RectangleParams.Length, shapeParams.RectangleParams.Width);
          break;
        default:
          throw new NotImplementedException();
      }
    }

    private IEnumerator Start() {
      if (Duration == 0) {
        Destroy(gameObject);
        yield break;
      }

      UpdateProperties();

      float startLength = Array
                              .Find(_animator.runtimeAnimatorController.animationClips,
                                    (AnimationClip clip) => clip.name == "AoeMarkerStart")
                              .length;
      float endLength = Array
                            .Find(_animator.runtimeAnimatorController.animationClips,
                                  (AnimationClip clip) => clip.name == "AoeMarkerEnd")
                            .length;

      if (Duration < startLength + endLength) {
        _animator.speed = (startLength + endLength) / Duration;
      }

      _animator.SetTrigger("Start");

      yield return new WaitForSeconds(Duration - endLength);

      _animator.SetTrigger("End");

      yield return new WaitForSeconds(endLength);

      Destroy(gameObject);
    }

    private void Update() {
      UpdateProperties();
    }

    private void UpdateProperties() {
      if (_material == null) {
        switch (_shape) {
          case Shape.Round:
            _material = new Material(_roundMaterial);
            break;
          case Shape.Rectangle:
            _material = new Material(_rectangleMaterial);
            break;
        }
      }

      if (_shape == Shape.Round) {
        Vector3 eular = _projector.transform.localRotation.eulerAngles;
        eular.y = 180 - _material.GetFloat("_Angle") / 2;
        _projector.transform.localRotation = Quaternion.Euler(eular);
      }
    }

    private void SetRound(float radius, float innerRadius, float angle) {
      _shape = Shape.Round;

      _material = new Material(_roundMaterial);
      _projector.material = _material;

      _projector.orthographicSize = radius;
      _material.SetFloat("_InnerRadius", innerRadius / radius);
      _material.SetFloat("_Angle", angle);
    }

    private void SetRectangle(float length, float width) {
      _shape = Shape.Rectangle;

      _material = new Material(_rectangleMaterial);
      _projector.material = _material;

      _projector.orthographicSize = 0.5f;
      _material.SetFloat("_Width", width);
      _material.SetFloat("_Length", length);
    }
  }
}
