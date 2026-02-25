using UnityEngine;
using System.Collections;

namespace KikiNgao.SimpleBikeControl
{
    public class SimpleBike : MonoBehaviour
    {
        [Header("Core")]
        public bool noBikerCtrl;
        public Transform bikerHolder;

        [Header("Wheel Setup")]
        public WheelCollider frontWheelCollider;
        public WheelCollider rearWheelCollider;
        public GameObject frontWheel;
        public GameObject rearWheel;
        public Transform handlerBar;
        public Transform cranksetTransform;

        [Header("Pedal / IK Targets")]
        public Transform leftHandTarget;
        public Transform rightHandTarget;
        public Transform leftPendalTarget;
        public Transform rightPendalTarget;

        [Header("Bike Power")]
        public float legPower = 10;
        public float powerUpMax = 2;
        public float powerUpSpeed = 0.5f;
        public float airResistance = 6;
        public float turningSmooth = 0.8f;
        public float restDrag = 2;
        public float restAngularDrag = 0.2f;
        public float forceRatio = 2;

        Rigidbody m_Rigidbody;
        InputManager inputManager;
        EventManager eventManager;

        float currentLegPower;
        float reversePower;

        // ================= INIT =================

        void Awake()
        {
            if (!m_Rigidbody)
                m_Rigidbody = GetComponent<Rigidbody>();

            // GameManager systems auto-find
            if (GameManager.Instance != null)
            {
                if (!inputManager)
                    inputManager = GameManager.Instance.GetInputManager;

                if (!eventManager)
                    eventManager = GameManager.Instance.GetEventManager;
            }

            // Auto find SeatMount if not assigned
            if (!bikerHolder)
            {
                Transform t = transform.Find("SeatMount");
                if (t) bikerHolder = t;
            }
        }


        IEnumerator Start()
        {
            // Rigidbody auto find
            if (!m_Rigidbody)
                m_Rigidbody = GetComponent<Rigidbody>();

            // Input manager safe find
            if (GameManager.Instance != null)
            {
                inputManager = GameManager.Instance.GetInputManager;
                eventManager = GameManager.Instance.GetEventManager;
            }

            // Safety checks (THIS STOPS NULL ERRORS)
            if (!frontWheelCollider || !rearWheelCollider)
                Debug.LogError(name + "  WheelCollider missing");

            if (!frontWheel || !rearWheel)
                Debug.LogError(name + "  Wheel mesh missing");

            if (!handlerBar)
                Debug.LogError(name + "  HandlerBar missing");

            if (!cranksetTransform)
                Debug.LogError(name + "  Pedal/Crank missing");

            // Freeze physics at spawn (stop flying bikes)
            if (m_Rigidbody)
                m_Rigidbody.isKinematic = true;

            yield return new WaitForSeconds(0.3f);

            if (m_Rigidbody)
                m_Rigidbody.isKinematic = false;
        }

        // ================= INPUT HELPERS =================

        public bool IsReverse()
        {
            if (!inputManager) return false;
            return inputManager.vertical < 0;
        }

        public bool IsMovingToward()
        {
            if (!inputManager) return false;
            return inputManager.vertical > 0;
        }

        public bool IsMoving()
        {
            if (!inputManager) return false;
            return inputManager.vertical != 0;
        }

        bool IsTurning()
        {
            if (!inputManager) return false;
            return inputManager.horizontal != 0;
        }

        bool IsSpeedUp()
        {
            if (!inputManager) return false;
            return inputManager.speedUp;
        }

        // ================= BIKE STATE =================

        float GetBikeSpeedMs()
        {
            if (!m_Rigidbody) return 0f;
            return m_Rigidbody.velocity.magnitude;
        }

        float GetBikeSpeedKm() => GetBikeSpeedMs() * 3.6f;

        float WrapAngle(float a)
        {
            if (a > 180) a -= 360;
            return a;
        }

        float GetBikeAngle() => WrapAngle(transform.eulerAngles.z);

        public bool TiltToRight() => GetBikeAngle() <= 0;

        public bool Freeze
        {
            get => m_Rigidbody && m_Rigidbody.isKinematic;
            set { if (m_Rigidbody) m_Rigidbody.isKinematic = value; }
        }

        public bool FreezeCrankset { get; set; }

        public bool ReadyToRide()
        {
            if (noBikerCtrl) return true;
            if (!bikerHolder) return false;
            if (bikerHolder.childCount == 0) return false;
            return bikerHolder.GetChild(0).CompareTag("Player");
        }

        // ================= PHYSICS =================

        void FixedUpdate()
        {
            if (!m_Rigidbody || !inputManager) return;

            // REST STATE
            if (!IsMoving())
            {
                m_Rigidbody.drag = restDrag;
                m_Rigidbody.angularDrag = restAngularDrag;
                return;
            }

            // MOVEMENT
            m_Rigidbody.drag = 0.05f;
            m_Rigidbody.angularDrag = 0.05f;

            float vertical = inputManager.vertical;
            float horizontal = inputManager.horizontal;

            // Forward / Reverse force
            Vector3 force = transform.forward * vertical * legPower * forceRatio;
            m_Rigidbody.AddForce(force, ForceMode.Acceleration);

            // Turning
            if (IsTurning())
            {
                float turn = horizontal * turningSmooth * 100f * Time.fixedDeltaTime;
                Quaternion rot = Quaternion.Euler(0, turn, 0);
                m_Rigidbody.MoveRotation(m_Rigidbody.rotation * rot);
            }

            // Wheel visual rotation
            UpdateWheelVisual(frontWheelCollider, frontWheel);
            UpdateWheelVisual(rearWheelCollider, rearWheel);
        }

        // ================= WHEEL VISUAL =================

        void UpdateWheelVisual(WheelCollider col, GameObject wheel)
        {
            if (!col || !wheel) return;

            Vector3 pos;
            Quaternion rot;
            col.GetWorldPose(out pos, out rot);

            wheel.transform.position = pos;
            wheel.transform.rotation = rot;
        }

        // ================= FALL =================

        public void Falling()
        {
            if (!m_Rigidbody) return;

            m_Rigidbody.drag = 1;
            m_Rigidbody.angularDrag = 0.01f;
        }
    }
}