using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pantheon {
  [DisallowMultipleComponent]
  public class AutoPilot : MonoBehaviour {
    public float TargetDistance;
    public float Speed = GlobalContext.MaxPlayerMoveSpeed;

    private Vector3 _destination;
    private bool _moving = false;

    public void Go(Vector2 destination) {
      _destination = new Vector3(destination.x, 0, destination.y);
      _moving = true;
    }

    public void Face(Vector2 forward) {
      if (forward != Vector2.zero) {
        transform.rotation = Quaternion.LookRotation(new Vector3(forward.x, 0, forward.y));
      }
    }

    private void Update() {
      if (!_moving) {
        return;
      }

      float distance = Vector3.Distance(_destination, transform.position);
      if (distance < TargetDistance) {
        _moving = false;
        return;
      }

      transform.Translate(Mathf.Min(distance - TargetDistance, Speed * Time.deltaTime) *
                              (_destination - transform.position).normalized,
                          Space.World);
    }
  }
}
