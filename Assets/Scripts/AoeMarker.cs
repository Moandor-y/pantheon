using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pantheon {
  public class AoeMarker : MonoBehaviour {
    public float Angle = 360;
    public float Opacity = 0;
    public float Duration;
    public float Radius;

    [SerializeField]
    private Projector _projector;

    [SerializeField]
    private Animator _animator;

    private Material _material;

    private float _prevAngle;
    private float _prevOpacity = 1;

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
      if (Angle != _prevAngle) {
        _material.SetFloat("_Angle", Angle);
        _prevAngle = Angle;
      }

      if (Opacity != _prevOpacity) {
        var color = _material.GetColor("_Color");
        color.a = Opacity;
        _material.SetColor("_Color", color);
        _prevOpacity = Opacity;
      }

      _projector.orthographicSize = 2 * Radius;
    }
  }
}
