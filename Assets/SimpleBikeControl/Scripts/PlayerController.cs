using UnityEngine;

namespace KikiNgao.SimpleBikeControl
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Setting")]
        public bool disable;
        [SerializeField] private string AnimSpeedParaName = "Speed";
        [SerializeField] private float turnSpeed = 10f;
        [SerializeField] private float runSpeed = 3f;
        [SerializeField] private float rotationDamping = 40;
        [SerializeField] private float gravity = -9.8f;
        [SerializeField] private bool stopMoverment = false;

        public bool moving { get; set; }

        private Vector3 m_MoveVector;
        private Vector3 m_Velocity;

        private CharacterController characterCtrl;
        public Animator m_Animator;

        private Transform camTrans;
        private Vector3 camForward;
        private InputManager inputManager;
        private Vector3 gravityMagnitude;

        void Start()
        {
            characterCtrl = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();
            camTrans = Camera.main ? Camera.main.transform : null;
            gravityMagnitude = new Vector3(0, gravity, 0);

            if (GameManager.Instance != null)
                inputManager = GameManager.Instance.GetInputManager;

            if (inputManager == null)
                inputManager = FindObjectOfType<InputManager>(true);

            if (inputManager == null)
                Debug.LogError("PlayerController still can't find InputManager!");
        }

        public void DisablePlayerCtrl()
        {
            disable = true;
            if (characterCtrl) characterCtrl.enabled = false;
        }

        public void EnablePlayerCtrl()
        {
            disable = false;
            if (characterCtrl) characterCtrl.enabled = true;
        }

        void FixedUpdate()
        {
            if (disable || inputManager == null) return;

            float inputSpeed =
                Mathf.Clamp01(Mathf.Abs(inputManager.horizontal) +
                              Mathf.Abs(inputManager.vertical));

            bool has_H_Input = !Mathf.Approximately(inputManager.horizontal, 0);
            bool has_V_Input = !Mathf.Approximately(inputManager.vertical, 0);

            moving = !stopMoverment && (has_H_Input || has_V_Input);

            // camera-based movement
            if (camTrans != null)
            {
                camForward = Vector3.Scale(camTrans.forward, new Vector3(1, 0, 1)).normalized;
                m_MoveVector = inputManager.vertical * camForward +
                               inputManager.horizontal * camTrans.right;
                m_MoveVector.Normalize();
            }

            m_Velocity = inputSpeed * m_MoveVector * runSpeed * Time.deltaTime;

            if (!characterCtrl.isGrounded)
                m_Velocity += gravityMagnitude * Time.deltaTime;

            // animation safe
            if (m_Animator)
                m_Animator.SetFloat(AnimSpeedParaName, inputSpeed);

            // rotate safely
            if (m_MoveVector != Vector3.zero)
            {
                Vector3 desiredForward =
                    Vector3.RotateTowards(transform.forward,
                                          m_MoveVector,
                                          turnSpeed * Time.deltaTime,
                                          0f);

                Quaternion desiredRotation =
                    Quaternion.LookRotation(desiredForward);

                transform.rotation =
                    Quaternion.Lerp(transform.rotation,
                                    desiredRotation,
                                    Time.deltaTime * rotationDamping);
            }

            characterCtrl.Move(m_Velocity);
        }
    }
}