using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pantheon {
  public class AoeMarker : MonoBehaviour {
    public float Angle {
      get { return _material.GetFloat("_Angle"); }

      set { _material.SetFloat("_Angle", value); }
    }

    public Color TintColor {
      get { return _material.GetColor("_Color"); }

      set { _material.SetColor("_Color", value); }
    }

    public float Radius {
      get { return _projector.orthographicSize / 2; }

      set { _projector.orthographicSize = value * 2; }
    }

    public float InnerRadius {
      get { return _material.GetFloat("_InnerRadius") * Radius; }

      set { _material.SetFloat("_InnerRadius", value / Radius); }
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

    private Material _material;

    private float _prevAngle;
    private float _prevOpacity = -1;
    private float _prevInnerRadius = -1;

    private void Awake() {
      _material = new Material(_projector.material);
      _projector.material = _material;
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
      Vector3 eular = _projector.transform.localRotation.eulerAngles;
      eular.y = 180 - Angle / 2;
      _projector.transform.localRotation = Quaternion.Euler(eular);
    }
  }
}
