using UnityEngine;

namespace Roguelike.Players
{
    public class PlayerCharacterController
    {
        private Transform _transform;
        private Rigidbody _rb;
        private PlayerCore _playerCore;

        // direction
        private bool _isLockOn;
        private Transform _target;

        // move
        private float _playerRotateSpeed = 10;
        private Vector3 _inputVelocity;
        private Vector3 _gravity;

        // groundcheck
        private float _floorOffsetY;
        private float _raycastDistance = 0.5f;
        private Vector3 _raycastFloorPos;
        private Vector3 _floorMovement;
        private Vector3 _combinedRaycast;

        // knockback
        private float _knockbackDuration = 1.0f;
        private float _knockbackTimer;
        private float _knockbackPower = 3.0f;
        private Vector3 _knockbackDirection;
        private bool _isKnockback = false;

        public PlayerCharacterController(Transform transform) {
            _transform = transform;
            transform.TryGetComponent(out _rb);
            transform.TryGetComponent(out _playerCore);
        }

        /// <summary>
        /// 入力方向を受け取り移動を行う
        /// </summary>
        internal void OnMoveInput(Vector3 input, float maxSpeed) {
            if (_isKnockback) return;
            var cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0f;

            var rot = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
            _inputVelocity = rot * input;
            SetPlayerRotation(input, cameraForward);

            _inputVelocity *= maxSpeed;
        }
        internal void CharacterMove() {
            if (_isLockOn && _playerCore.Parameter.IsMovable) {
                _transform.rotation = LockonRotation();
            }
            if (FloorRaycasts(0, 0, 0.6f) == Vector3.zero) {
                _gravity += Vector3.up * Physics.gravity.y * Time.deltaTime;
            }

            if (_isKnockback) {
                OnDuringKnockback();
                return;
            }

            _rb.velocity = _inputVelocity * Time.deltaTime + _gravity;
            GroundCheck();
            _inputVelocity = Vector3.zero;
        }
        internal void ToggleLockOn(bool frag) {
            _isLockOn = frag;
            _transform.rotation = LockonRotation();
        }
        internal void SetLockonTarget(Transform target) {
            _target = target;
        }


        /// <summary>
        /// カメラの方向を正面とし、入力方向へ移動させる
        /// </summary>
        private void SetPlayerRotation(Vector3 input, Vector3 cameraForward) {
            Quaternion targetRotation;
            if (_isLockOn) {
                targetRotation = LockonRotation();
            }
            else {
                var inputDirection = Quaternion.LookRotation(cameraForward) * input;
                if (inputDirection != Vector3.zero) {
                    targetRotation = Quaternion.LookRotation(inputDirection);
                }
                else {
                    targetRotation = _transform.rotation;
                }
            }
            _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, Time.deltaTime * _playerRotateSpeed);

        }
        private Quaternion LockonRotation() {
            Quaternion targetRotation = _transform.rotation;
            if (_target != null && _target.gameObject.activeSelf) {
                Vector3 targetDirection = _target.position - _transform.position;
                targetDirection.y = 0f;
                targetRotation = Quaternion.LookRotation(targetDirection);

                targetRotation = Quaternion.Slerp(_transform.rotation, targetRotation, Time.deltaTime * _playerRotateSpeed);
            }
            return targetRotation;
        }


        private void OnDuringKnockback() {
            _knockbackTimer -= Time.deltaTime;
            Vector3 moveBackPosition = _knockbackDirection * _knockbackPower + _gravity;
            _rb.velocity = moveBackPosition;
            GroundCheck();

            if (_knockbackTimer <= 0) {
                _isKnockback = false;
            }
        }

        private void GroundCheck() {
            _floorMovement = new Vector3(_rb.position.x, FindFloor().y + _floorOffsetY, _rb.position.z);

            if (FloorRaycasts(0, 0, 0.6f) != Vector3.zero && _floorMovement != _rb.position) {
                _rb.MovePosition(_floorMovement);
                _gravity.y = 0;
            }
        }

        public void ApplyKnockback(Vector3 direction, float power, float duration) {
            if (!_isKnockback) {
                _knockbackDirection = direction;
                _knockbackPower = power;
                _knockbackDuration = duration;
                _knockbackTimer = _knockbackDuration;
                _isKnockback = true;
            }
        }

        private Vector3 FindFloor() {
            int floorAverage = 1;

            _combinedRaycast = FloorRaycasts(0, 0, 1.6f);
            floorAverage += (getFloorAverage(_raycastDistance, 0) + getFloorAverage(-_raycastDistance, 0) + getFloorAverage(0, _raycastDistance) + getFloorAverage(0, -_raycastDistance));
            return _combinedRaycast / floorAverage;
        }

        private int getFloorAverage(float offsetx, float offsetz) {

            if (FloorRaycasts(offsetx, offsetz, 1.6f) != Vector3.zero) {
                _combinedRaycast += FloorRaycasts(offsetx, offsetz, 1.6f);
                return 1;
            }
            else { return 0; }
        }


        private Vector3 FloorRaycasts(float offsetx, float offsetz, float raycastLength) {
            RaycastHit hit;
            _raycastFloorPos = _transform.TransformPoint(0 + offsetx, 0 + 0.5f, 0 + offsetz);

            Debug.DrawRay(_raycastFloorPos, Vector3.down, Color.magenta);
            if (Physics.Raycast(_raycastFloorPos, -Vector3.up, out hit, raycastLength)) {
                return hit.point;
            }
            else return Vector3.zero;
        }
    }
}