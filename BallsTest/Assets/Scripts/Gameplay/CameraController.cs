using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using Test.Game;

public static class CameraShakerEdits
{
    public static void EnemyDeathShake(this CameraShaker shaker)
    {
        shaker.ShakeOnce(5f, 10f, .1f, .5f);
    }
}

namespace Test.Gameplay
{
    public class CameraController : MonoBehaviour
    {
        [Header("Refrences")]
        public Camera Cam;
        public Transform PlayerTransform;

        [Header("Parameters")]
        public float LerpSpeed;
        public float CamFollowDamping;

        private float startFOV;

        private Vector3 followVelocity = Vector3.zero;

        private float startDeltaTime = 0;

        Dictionary<string, float> FOVs = new Dictionary<string, float>();

        public void SetFOV(string id, float fov)
        {
            if (FOVs.ContainsKey(id))
            {
                FOVs.Remove(id);
            }

            FOVs.Add(id, fov);
        }

        private void UpdateFOV()
        {
            float finalFOV = startFOV;

            foreach (float fov in FOVs.Values)
            {
                finalFOV += Mathf.Abs(fov);
            }

            // Debug.Log("Start fov: " + startFOV + ", " + finalFOV + ", " + Cam.orthographicSize);

            Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, finalFOV, LerpSpeed * startDeltaTime);
        }

        private void Follow(Vector2 targetPos)
        {
            transform.position = Vector3.SmoothDamp(
                Cam.transform.position
                , new Vector3(
                    targetPos.x
                    , targetPos.y
                    , Cam.transform.position.z
                )
                , ref followVelocity
                , CamFollowDamping

            );
        }

        void Start()
        {
            if (!Cam)
                Cam = GetComponent<Camera>();

            startFOV = Cam.orthographicSize;

            startDeltaTime = Time.fixedDeltaTime;
        }

        void FixedUpdate()
        {
            if (StartMenu.isPaused)
            {
                return;
            }

            UpdateFOV();

            if (PlayerTransform)
            {
                Follow(PlayerTransform.position);
            }
        }
    }
}