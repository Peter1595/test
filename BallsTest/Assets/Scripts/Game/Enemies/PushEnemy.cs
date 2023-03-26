using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using Test.Gameplay;

namespace Test.Game
{
    public class PushEnemy : EnemyBase
    {
        public float randomPushAngle = 180f;
        public float forceMultiplier = 1f;
        public float minForce = 0;
        public GameObject deathEffect;

        void SpawnEffect()
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);

            CameraShaker.Instance.EnemyDeathShake();
        }

        protected override void Die()
        {
            base.Die();

            SpawnEffect();
            Destroy(gameObject);
        }

        // Vector2 GetRandomDireciton(Vector2 mainDirection)
        // {
        //     Vector2 fixedMainDirection =
        //     -mainDirection;

        //     float randomPushAngleRatio = randomPushAngle / 180;

        //     Debug.Log("random push angle: " + randomPushAngleRatio);

        //     Vector2 randomOffset =
        //     new Vector2(
        //         fixedMainDirection.y * Random.Range(-randomPushAngleRatio, randomPushAngleRatio)
        //         , fixedMainDirection.x * Random.Range(-randomPushAngleRatio, randomPushAngleRatio)
        //     );

        //     Vector2 direction =
        //     (
        //         fixedMainDirection
        //         + randomOffset
        //     ).normalized;

        //     return direction;
        // }

        void PushPlayerRandomly(GameObject player)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

            if (!rb)
            {
                return;
            }

            float previousMagnitude =
            rb.velocity.magnitude;

            if (previousMagnitude < minForce)
            {
                previousMagnitude = minForce;
            }

            Vector2 previousDirection =
            Random.insideUnitCircle;

            rb.velocity =
            Vector2.zero;

            rb.AddForce(previousDirection * (previousMagnitude * forceMultiplier), ForceMode2D.Impulse);
        }

        void ResetPlayerPush(GameObject player)
        {
            PlayerMovement movement = player.GetComponent<PlayerMovement>();

            if (!movement)
            {
                return;
            }

            movement.ResetCanPush();

        }

        void DestroyToPlayer(GameObject player)
        {
            Die();

            PushPlayerRandomly(player);
            ResetPlayerPush(player);
        }

        protected override void OnCollideWithPlayer(GameObject player)
        {
            DestroyToPlayer(player);
        }

    }
}