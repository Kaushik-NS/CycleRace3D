using UnityEngine;
using UnityEngine.Events;

namespace KikiNgao.SimpleBikeControl
{
    public class Biker : MonoBehaviour
    {
        public SimpleBike currentBike;

        [SerializeField] float standBodyOffset = 0.15f;
        [SerializeField] float standAngle = 15f;

        public UnityEvent OnEnter, OnRidding;

        private Animator m_Animator;
        private GameObject leftLegTargetObj, rightLegTargetObj;

        private bool riding = false;

        private float movingBodySmooth = 0.3f;
        private float rotateBodySmooth = 0.1f;
        private float movingLegSmooth = 0.1f;
        private float IKWeight = 1f;

        private bool MovingBike => currentBike && currentBike.IsMoving();

        void Start()
        {
            m_Animator = GetComponent<Animator>();

            if (!currentBike)
            {
                Debug.LogError("Assign bike in Biker inspector");
                return;
            }

            // AUTO ENTER BIKE AT START
            EnterBikeInstant();
        }

        void EnterBikeInstant()
        {
            if (!currentBike.bikerHolder)
            {
                Debug.LogError("Bike missing bikerHolder transform");
                return;
            }

            // create IK targets
            InitLegTarget(true);

            // parent rider to bike seat
            transform.parent = currentBike.bikerHolder;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            m_Animator.SetBool("Riding", true);
            riding = true;

            OnEnter?.Invoke();
        }

        void InitLegTarget(bool init)
        {
            if (!init) return;

            rightLegTargetObj = new GameObject("Right Leg Target");
            rightLegTargetObj.transform.parent = currentBike.rightPendalTarget.parent;
            rightLegTargetObj.transform.position = currentBike.rightPendalTarget.position;

            leftLegTargetObj = new GameObject("Left Leg Target");
            leftLegTargetObj.transform.parent = currentBike.leftPendalTarget.parent;
            leftLegTargetObj.transform.position = currentBike.leftPendalTarget.position;
        }

        void FixedUpdate()
        {
            if (!riding || !currentBike) return;
            RidingBike();
        }

        private void RidingBike()
        {
            if (!MovingBike)
            {
                // keep rider centered and ready to pedal
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

                MovingLegToPendal(leftLegTargetObj.transform,
                    currentBike.leftPendalTarget, movingLegSmooth);

                MovingLegToPendal(rightLegTargetObj.transform,
                    currentBike.rightPendalTarget, movingLegSmooth);

                return;
            }

            // MOVING
            transform.localPosition = Vector3.Lerp(transform.localPosition,
                Vector3.zero, rotateBodySmooth);

            transform.localRotation = Quaternion.Lerp(transform.localRotation,
                Quaternion.identity, rotateBodySmooth);

            if (currentBike.IsMovingToward())
            {
                currentBike.FreezeCrankset = false;
                m_Animator.SetBool("Reverse", false);

                MovingLegToPendal(leftLegTargetObj.transform,
                    currentBike.leftPendalTarget, movingLegSmooth);

                MovingLegToPendal(rightLegTargetObj.transform,
                    currentBike.rightPendalTarget, movingLegSmooth);
            }

            if (currentBike.IsReverse())
            {
                currentBike.FreezeCrankset = true;
                m_Animator.SetBool("Reverse", true);
                m_Animator.SetBool("Left", !currentBike.TiltToRight());
            }

            OnRidding?.Invoke();
        }

        private void MovingBody(bool toRight, float distance, float smooth)
        {
            if (toRight && transform.localPosition.x < distance)
                transform.localPosition += Vector3.right * Time.deltaTime * smooth;

            if (!toRight && transform.localPosition.x > -distance)
                transform.localPosition -= Vector3.right * Time.deltaTime * smooth;
        }

        private void MovingLegToPendal(Transform standPoint, Transform pendalPoint, float smooth)
        {
            standPoint.position = Vector3.Lerp(standPoint.position,
                pendalPoint.position, smooth);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (!riding || !currentBike) return;

            if (currentBike.leftHandTarget)
            {
                m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, IKWeight);
                m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, IKWeight);
                m_Animator.SetIKPosition(AvatarIKGoal.LeftHand,
                    currentBike.leftHandTarget.position);
                m_Animator.SetIKRotation(AvatarIKGoal.LeftHand,
                    currentBike.leftHandTarget.rotation);
            }

            if (currentBike.rightHandTarget)
            {
                m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, IKWeight);
                m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, IKWeight);
                m_Animator.SetIKPosition(AvatarIKGoal.RightHand,
                    currentBike.rightHandTarget.position);
                m_Animator.SetIKRotation(AvatarIKGoal.RightHand,
                    currentBike.rightHandTarget.rotation);
            }
        }
    }
}