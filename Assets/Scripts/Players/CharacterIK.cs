using UnityEngine;

namespace Roguelike.Players
{
    [RequireComponent(typeof(Animator))]
    public class FootIK : MonoBehaviour
    {
        [Header("Main")]
        [Range(0, 1)][SerializeField] private float _weight = 1f;
        [Header("Settings")]
        [SerializeField] private float _maxStep = 0.5f;
        [SerializeField] private float _footRadius = 0.15f;
        [SerializeField] private LayerMask _ground = 1;
        [SerializeField] private float _offset = 0f;
        [Header("Speed")]
        [SerializeField] private float _hipsPositionSpeed = 1f;
        [SerializeField] private float _feetPositionSpeed = 2f;
        [SerializeField] private float _feetRotationSpeed = 90;
        [Header("Weight")]
        [Range(0, 1)][SerializeField] private float _hipsWeight = 0.75f;
        [Range(0, 1)][SerializeField] private float _footPositionWeight = 1f;
        [Range(0, 1)][SerializeField] private float _footRotationWeight = 1f;
        [SerializeField] private bool _showDebug = true;
        //Private variables
        private Vector3 _lIKPosition, _rIKPosition, _lNormal, _rNormal;
        private Quaternion _lIKRotation, _rIKRotation, _lastLeftRotation, _lastRightRotation;
        private float _lastRFootHeight, _lastLFootHeight;

        private Animator _anim;
        private float _velocity;
        private float _falloffWeight;
        private float _lastHeight;
        private Vector3 _lastPosition;
        private bool _lGrounded, _rGrounded, _isGrounded;

        // Initialization
        private void Awake() {
            _anim = GetComponent<Animator>();
        }

        //Updating the position of each foot.
        private void FixedUpdate() {
            if (_weight == 0 || !_anim) { return; }

            Vector3 speed = (_lastPosition - _anim.transform.position) / Time.fixedDeltaTime;
            _velocity = Mathf.Clamp(speed.magnitude, 1, speed.magnitude);
            _lastPosition = _anim.transform.position;

            //Raycast to the ground to find positions
            FeetSolver(HumanBodyBones.LeftFoot, ref _lIKPosition, ref _lNormal, ref _lIKRotation, ref _lGrounded); //Left foot
            FeetSolver(HumanBodyBones.RightFoot, ref _rIKPosition, ref _rNormal, ref _rIKRotation, ref _rGrounded); //Right foot
                                                                                                                    //Grounding
            GetGrounded();
        }

        private void OnAnimatorIK(int layerIndex) {
            if (_weight == 0 || !_anim) { return; }

            //Pelvis height
            MovePelvisHeight();
            //Left foot IK
            MoveIK(AvatarIKGoal.LeftFoot, _lIKPosition, _lNormal, _lIKRotation, ref _lastLFootHeight, ref _lastLeftRotation);
            //Right foot IK
            MoveIK(AvatarIKGoal.RightFoot, _rIKPosition, _rNormal, _rIKRotation, ref _lastRFootHeight, ref _lastRightRotation);
        }

        //Set the pelvis height.
        private void MovePelvisHeight() {
            //Get height
            float leftOffset = _lIKPosition.y - _anim.transform.position.y;
            float rightOffset = _rIKPosition.y - _anim.transform.position.y;
            float totalOffset = (leftOffset < rightOffset) ? leftOffset : rightOffset;
            //Get hips position
            Vector3 newPosition = _anim.bodyPosition;
            float newHeight = totalOffset * (_hipsWeight * _falloffWeight);
            _lastHeight = Mathf.MoveTowards(_lastHeight, newHeight, _hipsPositionSpeed * Time.deltaTime);
            newPosition.y += _lastHeight + _offset;
            //Set position
            _anim.bodyPosition = newPosition;
        }

        //Feet
        private void MoveIK(AvatarIKGoal foot, Vector3 iKPosition, Vector3 normal, Quaternion iKRotation, ref float lastHeight, ref Quaternion lastRotation) {
            Vector3 position = _anim.GetIKPosition(foot);
            Quaternion rotation = _anim.GetIKRotation(foot);

            //Position
            position = _anim.transform.InverseTransformPoint(position);
            iKPosition = _anim.transform.InverseTransformPoint(iKPosition);
            lastHeight = Mathf.MoveTowards(lastHeight, iKPosition.y, _feetPositionSpeed * Time.deltaTime);
            position.y += lastHeight;

            position = _anim.transform.TransformPoint(position);
            position += normal * _offset;

            //Rotation
            Quaternion relative = Quaternion.Inverse(iKRotation * rotation) * rotation;
            lastRotation = Quaternion.RotateTowards(lastRotation, Quaternion.Inverse(relative), _feetRotationSpeed * Time.deltaTime);

            rotation *= lastRotation;

            //Set IK
            _anim.SetIKPosition(foot, position);
            _anim.SetIKPositionWeight(foot, _footPositionWeight * _falloffWeight);
            _anim.SetIKRotation(foot, rotation);
            _anim.SetIKRotationWeight(foot, _footRotationWeight * _falloffWeight);
        }

        private void GetGrounded() {
            //Set Weight
            _isGrounded = _lGrounded || _rGrounded;
            //Fading out MainWeight when is not grounded
            _falloffWeight = LerpValue(_falloffWeight, _isGrounded ? 1f : 0f, 1f, 10f, Time.fixedDeltaTime) * _weight;
        }


        public float LerpValue(float current, float desired, float increaseSpeed, float decreaseSpeed, float deltaTime) {
            if (current == desired) return desired;
            if (current < desired) return Mathf.MoveTowards(current, desired, (increaseSpeed * _velocity) * deltaTime);
            else return Mathf.MoveTowards(current, desired, (decreaseSpeed * _velocity) * deltaTime);
        }


        //Feet solver
        private void FeetSolver(HumanBodyBones foot, ref Vector3 iKPosition, ref Vector3 normal, ref Quaternion iKRotation, ref bool grounded) {
            Vector3 position = _anim.GetBoneTransform(foot).position;
            position.y = _anim.transform.position.y + _maxStep;

            //Raycast section 
            RaycastHit hit;
            //Add offset
            position -= normal * _offset;
            float feetHeight = _maxStep;

            if (_showDebug)
                Debug.DrawLine(position, position + Vector3.down * (_maxStep * 2), Color.yellow);

            if (Physics.SphereCast(position, _footRadius, Vector3.down, out hit, _maxStep * 2, _ground)) {
                //Position (height)
                feetHeight = _anim.transform.position.y - hit.point.y;
                iKPosition = hit.point;
                //Normal (Slope)
                normal = hit.normal;
                if (_showDebug)
                    Debug.DrawRay(hit.point, hit.normal, Color.blue);
                //Rotation (normal)
                Vector3 axis = Vector3.Cross(Vector3.up, hit.normal);
                float angle = Vector3.Angle(Vector3.up, hit.normal);
                iKRotation = Quaternion.AngleAxis(angle, axis);
            }

            grounded = feetHeight < _maxStep;

            if (!grounded) {
                iKPosition.y = _anim.transform.position.y - _maxStep;
                iKRotation = Quaternion.identity;
            }
        }
    }
}