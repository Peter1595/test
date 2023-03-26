using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using Test.Gameplay;

namespace Test.Game
{
    public class KillerEnemy : EnemyBase
    {
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

        void DestroyToPlayer(GameObject player)
        {
            if (player.transform.parent.TryGetComponent<PlayerDeath>(out PlayerDeath pd))
            {
                pd.Die();

                pd.Spawn(3f);

                return;
            }

            Die();
        }

        protected override void OnCollideWithPlayer(GameObject player)
        {
            DestroyToPlayer(player);
        }


    }
}