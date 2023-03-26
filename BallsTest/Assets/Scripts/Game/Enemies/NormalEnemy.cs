using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Test.Gameplay;
using EZCameraShake;

namespace Test.Game
{
    public class NormalEnemy : EnemyBase
    {
        public float forceMultiplier;
        public float minForce;
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

        void PushPlayerUp(GameObject player)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

            if (!rb)
            {
                return;
            }

            float velocityPercent =
            rb.velocity.magnitude / 100;

            float force =
            this.forceMultiplier * velocityPercent;

            if (force < minForce)
            {
                force = minForce;
            }

            rb.velocity /= 2;

            if (rb.velocity.y < 0)
                rb.velocity -=
                new Vector2(
                    0
                    , rb.velocity.y
                );

            rb.AddForce((Vector2.up * (force
            * Random.Range(.75f, 1f)))
            + Vector2.right
            * ((force * .5f)
            * Random.Range(-.1f, .1f))
            , ForceMode2D.Impulse);
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

            PushPlayerUp(player);
            ResetPlayerPush(player);
        }

        protected override void OnCollideWithPlayer(GameObject player)
        {
            DestroyToPlayer(player);
        }
    }
}