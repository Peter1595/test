using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using Test.Gameplay;

namespace Test.Game
{
    public class FreeEnemy : EnemyBase
    {
        public GameObject deathEffect;
        public float forceMultiplier;

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

        void ResetPlayerPush(GameObject player)
        {
            PlayerMovement movement = player.GetComponent<PlayerMovement>();

            if (!movement)
            {
                return;
            }

            movement.ResetCanPush();

        }


        void RefreshPlayerPush(GameObject player)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

            if (!rb)
            {
                return;
            }

            float previousMagnitude =
            rb.velocity.magnitude;

            Vector2 previousDirection =
            rb.velocity.normalized;

            rb.velocity =
            Vector2.zero;

            rb.AddForce(previousDirection * (previousMagnitude * forceMultiplier), ForceMode2D.Impulse);
        }

        protected override void OnCollideWithPlayer(GameObject player)
        {
            ResetPlayerPush(player);

            RefreshPlayerPush(player);

            Die();
        }


    }
}