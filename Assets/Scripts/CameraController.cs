using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon {
  [DisallowMultipleComponent]
  public class CameraController : Singleton<CameraController> {
    public GameObject Player;

    private float _distance = 16;

    private bool _cursorLocked = false;
    private Vector2 _cursorLockedPosition;

    private bool _moveCamera = false;

    void Update() {
      _moveCamera = Input.GetMouseButton(0) || Input.GetMouseButton(1);

      if (_moveCamera && !_cursorLocked) {
        _cursorLocked = true;
        _cursorLockedPosition = CursorUtil.GetPosition();
        Cursor.visible = false;
      }

      if (_cursorLocked) {
        CursorUtil.SetPosition(_cursorLockedPosition);
      }

      if (!_moveCamera && _cursorLocked) {
        _cursorLocked = false;
        Cursor.visible = true;
      }

      if (_moveCamera) {
        Vector3 euler = transform.rotation.eulerAngles;
        euler.x -= Input.GetAxis("Mouse Y");
        euler.x %= 360;
        if (euler.x > 180) {
          euler.x -= 360;
        }
        euler.x = Mathf.Clamp(euler.x, -89, 89);
        euler.y += Input.GetAxis("Mouse X");
        euler.z = 0;
        transform.rotation = Quaternion.Euler(euler);
      }
    }

    void LateUpdate() {
      if (Player != null) {
        transform.position = Player.transform.position - _distance * transform.forward;
      }
    }
  }
}
