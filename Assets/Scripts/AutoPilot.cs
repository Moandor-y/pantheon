using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pantheon {
  [DisallowMultipleComponent]
  public class AutoPilot : MonoBehaviour {
    private const float _minDistance = 0.01f;

    private Vector3 _destination;
    private bool _moving = false;

    public void Go(Vector2 destination) {
      _destination = new Vector3(destination.x, 0, destination.y);
      _moving = true;
    }

    private void Update() {
      if (!_moving) {
        return;
      }

      float distance = Vector3.Distance(_destination, transform.position);
      if (distance < _minDistance) {
        _moving = false;
        return;
      }

      transform.Translate(Mathf.Min(distance, GlobalContext.MaxMoveSpeed * Time.deltaTime) *
                              (_destination - transform.position).normalized,
                          Space.World);
    }
  }
}
