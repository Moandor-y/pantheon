using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pantheon
{
    [DisallowMultipleComponent]
    public class PlayerMovementController : MonoBehaviour
    {
        private CameraController cameraController;

        private MoveState moveState = MoveState.Idle;
        private float moveStartTime;

        private float _moveHorizontal;
        private float _moveVertical;

        private Vector3 _moveDirection;

        private void Start()
        {
            cameraController = CameraController.Instance;
            cameraController.Player = gameObject;
        }

        private void Update()
        {
            _moveHorizontal = Input.GetAxis("Horizontal");
            _moveVertical = Input.GetAxis("Vertical");

            UpdateMoveState();

            Vector3 forward = Vector3.ProjectOnPlane(cameraController.transform.forward, Vector3.up);
            Vector3 right = Vector3.ProjectOnPlane(cameraController.transform.right, Vector3.up);
            _moveDirection = (forward * _moveVertical + right * _moveHorizontal).normalized;

            if (moveState != MoveState.Idle) {
                transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            }
        }

        private void FixedUpdate() {
            transform.position += GetMoveSpeed() * Time.fixedDeltaTime * _moveDirection;
        }

        private void UpdateMoveState() {
            if (Mathf.Abs(_moveHorizontal) > _moveAxisThreashold || Mathf.Abs(_moveVertical) > _moveAxisThreashold) {
                if (moveState == MoveState.Idle) {
                    moveState = MoveState.Starting;
                    moveStartTime = Time.time;
                } else if (moveState == MoveState.Starting && Time.time > moveStartTime + _moveStartingDuration) {
                    moveState = MoveState.Moving;
                }
            } else {
                if (moveState == MoveState.Starting || moveState == MoveState.Moving) {
                    moveState = MoveState.Idle;
                }
            }
        }

        private float GetMoveSpeed() {
            switch (moveState) {
                case MoveState.Idle:
                return 0;
                case MoveState.Starting:
                return ((Time.time - moveStartTime) / _moveStartingDuration) * _maxMoveSpeed;
                case MoveState.Moving:
                return _maxMoveSpeed;
                default:
                throw new Exception("Unknown move state");
            }
        }

        private enum MoveState
        {
            Idle,
            Starting,
            Moving,
        };

        private const float _moveAxisThreashold = 0.5f;
        private const float _moveStartingDuration = 0.2f;
        private const float _maxMoveSpeed = 6;
    }
}
