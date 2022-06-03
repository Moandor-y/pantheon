using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pantheon {
  public class AoeMarker : MonoBehaviour {
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
      eular.y = 180 - _material.GetFloat("_Angle") / 2;
      _projector.transform.localRotation = Quaternion.Euler(eular);
    }

    public void SetRound(float radius, float innerRadius, float angle) {
      _material = new Material(_roundMaterial);
      _projector.material = _material;

      _projector.orthographicSize = radius;
      _material.SetFloat("_InnerRadius", innerRadius / radius);
      _material.SetFloat("_Angle", angle);
    }
  }
}
