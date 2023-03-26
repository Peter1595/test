using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Test.Gameplay;
using EZCameraShake;

namespace Test.Game
{
    public class EnemyBase : MonoBehaviour
    {
        public bool despawn = true;
        public float scoreForDeath = 0;
        public float despawnDistance = 100f;
        public UnityAction OnDie;

        protected virtual void Die()
        {
            FindFirstObjectByType<ScoreManager>().AddScore(scoreForDeath);

            OnDie?.Invoke();
        }
        protected virtual void OnCollideWithPlayer(GameObject player)
        {
            Debug.Log("Collided with the player");
        }

        protected virtual void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.GetComponent<PlayerMovement>())
            {
                OnCollideWithPlayer(collider.gameObject);
            }
        }

        public Transform CameraTranform;

        void Start()
        {
            CameraTranform = FindFirstObjectByType<CameraShaker>().transform;
        }

        void Update()
        {
            if (CameraTranform && despawn)
            {
                float distance = Vector2.Distance(CameraTranform.transform.position, transform.position);

                if (distance >= despawnDistance)
                {
                    OnDie?.Invoke();
                    Destroy(gameObject);
                }
            }
        }
    }
}