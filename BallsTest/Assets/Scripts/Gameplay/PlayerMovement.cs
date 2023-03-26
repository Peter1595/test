using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Test.Game;

namespace Test.Gameplay
{

    public class PlayerMovement : MonoBehaviour
    {
        [Header("Refrences")]
        public Rigidbody2D PlayerRigidbody;
        public CameraController PlayerCamera;
        public Transform PlayerTransform;
        public LayerMask Ground;
        public Slider StamnaSlider;

        public LineRenderer LinePrefab;

        private Vector2 PlayerPos
        {
            get
            {
                return (Vector2)PlayerTransform.position;
            }
            set
            {
                PlayerTransform.position = value;
            }
        }
        private float PlayerHalfHeight
        {
            get
            {
                return PlayerTransform.localScale.y / 2;
            }
            set
            {
                PlayerTransform.localScale =
                new Vector2(
                    PlayerTransform.localScale.x
                    , value * 2
                );
            }
        }

        private LineRenderer line;

        [Header("Parameters")]
        public float PushForce;
        public float CameraFOVPerForce;
        public float HoldSlowMotionTimeScale;
        public float MaxSpeed;
        public float MaxStamna = 100;
        public float StamnaDecressPerSecond = 5;
        public float StamnaIncressPerSecond = 10;
        public float StamnaSlidingSpeed = 10;
        public float PushCooldown = .1f;

        private float startTimeScale = 1;
        private float startFixedDeltaTime = 0;
        private float startDeltaTime = 0;
        private float currentStamna = 0;

        public float CurrentStamna
        {
            get
            {
                return currentStamna;
            }
            set
            {
                currentStamna = value;
            }
        }
        public bool CanPush
        {
            get;
            set;
        }

        private Vector3 startScale;
        private Vector3 desiredScale;

        private float pushCooldownTime;

        private void UpdateStamnaSlider()
        {
            StamnaSlider.value = Mathf.Lerp(StamnaSlider.value
            , CurrentStamna / MaxStamna
            , StamnaSlidingSpeed * startDeltaTime);
        }
        private void StartSlowMotion()
        {
            if (StartMenu.isPaused)
            {
                return;
            }

            Time.timeScale = HoldSlowMotionTimeScale;
            Time.fixedDeltaTime = startFixedDeltaTime * HoldSlowMotionTimeScale;
        }
        private void StopSlowMotion()
        {
            if (StartMenu.isPaused)
            {
                return;
            }
            Time.timeScale = startTimeScale;
            Time.fixedDeltaTime = startFixedDeltaTime;
        }

        public void ResetCanPush()
        {
            CanPush = true;
        }

        private void Push(Vector2 targetPos)
        {
            if (!CanPush)
            {
                return;
            }

            float distance = Vector2.Distance(targetPos, PlayerPos);
            float distancePercent = distance / 100;

            LookAt(targetPos);

            Vector2 pushPower = PlayerTransform.up * (PushForce * distancePercent);

            PlayerRigidbody.angularVelocity = 0;
            PlayerRigidbody.velocity = Vector2.zero;

            PlayerRigidbody.AddForce(pushPower, ForceMode2D.Impulse);

            CanPush = false;

            pushCooldownTime = Time.time + PushCooldown;
        }
        private bool IsGrounded()
        {
            float distance = PlayerHalfHeight + (0.01f * PlayerHalfHeight);

            Debug.DrawLine(PlayerPos, PlayerPos + Vector2.down * distance, Color.yellow);

            return Physics2D.Raycast(
                PlayerPos
                , Vector2.down
                , distance
                , Ground
            )
            // && PlayerRigidbody.velocity.y <= 0
            && Time.time >= pushCooldownTime
            ;
        }
        private void CheckCanPush()
        {
            if (IsGrounded())
            {
                CanPush = true;
            }
        }
        private void UpdateCameraFOV()
        {
            PlayerCamera.SetFOV("Push"
            , (Vector2.Distance(PlayerPos, PlayerCamera.Cam.transform.position) / 1) * CameraFOVPerForce);
        }

        private void UpdateLine(Vector2 targetPos)
        {
            if (!line)
            {
                line = Instantiate(LinePrefab);
            }

            line.SetPosition(0, PlayerPos);
            line.SetPosition(1, targetPos);
        }
        private void DestroyLine()
        {
            if (line)
            {
                Destroy(line.gameObject);

                line = null;
            }
        }
        private void LookAt(Vector2 targetPos)
        {
            Vector2 direction = (targetPos - PlayerPos).normalized;

            PlayerTransform.up = direction;
        }
        private void LookAtVelocity()
        {
            if (PlayerRigidbody.velocity.magnitude <= 0)
            {
                return;
            }

            PlayerTransform.up = PlayerRigidbody.velocity.normalized;
        }
        private void SqueezeForVelocity()
        {
            float distancePercent =
            PlayerRigidbody.velocity.magnitude / 40;

            distancePercent /= 2;

            distancePercent += 1;

            if (distancePercent < 1)
            {
                distancePercent = 1;
            }

            desiredScale = new Vector3(
                startScale.x / distancePercent
                , startScale.y * distancePercent
                , PlayerTransform.localScale.z
            );
        }
        private void SqueezeForDistance(Vector2 targetPos)
        {
            float distancePercent =
            Vector2.Distance(targetPos, PlayerCamera.Cam.transform.position) / 20;

            distancePercent /= 2;

            distancePercent += 1;

            if (distancePercent < 1)
            {
                distancePercent = 1;
            }

            desiredScale = new Vector3(
                startScale.x * distancePercent
                , startScale.y / distancePercent
                , PlayerTransform.localScale.z
            );
        }

        private void UpdateScale()
        {
            PlayerTransform.localScale =
            Vector3.Lerp(
                PlayerTransform.localScale
                , desiredScale
                , 25 * Time.deltaTime
            );
        }

        public void StopHoldingPush()
        {
            DestroyLine();
            LookAtVelocity();
            SqueezeForVelocity();

            StopSlowMotion();
        }

        void ControlSpeed()
        {
            if (PlayerRigidbody.velocity.magnitude > MaxSpeed)
            {
                PlayerRigidbody.velocity =
                PlayerRigidbody.velocity.normalized * MaxSpeed;
            }
        }

        void IncressStamna()
        {
            CurrentStamna += StamnaIncressPerSecond * startDeltaTime;

            if (CurrentStamna > MaxStamna)
            {
                CurrentStamna = MaxStamna;
            }
        }
        void DecressStamna()
        {
            CurrentStamna -= StamnaDecressPerSecond * startDeltaTime;

            if (CurrentStamna < 0)
            {
                CurrentStamna = 0;
            }
        }

        void Awake()
        {
            startScale = PlayerTransform.localScale;
            desiredScale = startScale;

            startTimeScale = Time.timeScale;
            startFixedDeltaTime = Time.fixedDeltaTime;
            startDeltaTime = Time.deltaTime;

            CurrentStamna = MaxStamna;
        }

        bool canHold = false;

        void Update()
        {
            if (StartMenu.isPaused)
            {
                canHold = false;

                return;
            }

            UpdateCameraFOV();
            CheckCanPush();
            UpdateScale();
            ControlSpeed();
            UpdateStamnaSlider();

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetKeyUp(KeyCode.Mouse0) && canHold)
            {
                Push(mousePos);

                canHold = false;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                canHold = true;
            }

            if (Input.GetKey(KeyCode.Mouse0) && canHold && CanPush && CurrentStamna > 0)
            {
                UpdateLine(mousePos);
                LookAt(mousePos);
                SqueezeForDistance(mousePos);

                StartSlowMotion();

                DecressStamna();
            }
            else
            {
                if (CurrentStamna <= 0)
                {
                    canHold = false;
                }

                StopHoldingPush();

                IncressStamna();
            }
        }
    }
}