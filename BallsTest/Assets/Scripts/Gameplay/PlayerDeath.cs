using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using UnityEngine.SceneManagement;
using Test.Game;

namespace Test.Gameplay
{
    public class PlayerDeath : MonoBehaviour
    {
        public GameObject player;
        public GameObject deathEffect;

        private SpriteRenderer r;
        private Rigidbody2D rb;
        private Transform t;

        private Vector2 startPosition;

        public bool isDead
        {
            get;
            private set;
        } = false;

        void SpawnEffect()
        {
            if (deathEffect)
                Instantiate(deathEffect, player.transform.position, Quaternion.identity);

            CameraShaker.Instance.ShakeOnce(6f, 6f, .1f, 4f);
        }

        public void Die()
        {
            if (isDead)
            {
                return;
            }

            if (player.TryGetComponent(out PlayerMovement movement))
            {
                movement.StopHoldingPush();
            }

            SpawnEffect();

            player.SetActive(false);

            isDead = true;
        }

        public void Spawn(Vector2 spawnPosition)
        {
            FindFirstObjectByType<StartMenu>().Pause();

            player.SetActive(true);

            t.position = spawnPosition;

            isDead = false;
        }
        public void Spawn()
        {
            Spawn(startPosition);
        }

        public void Spawn(float delay)
        {
            StartCoroutine(DelaySpawn(delay));
        }
        public void Spawn(Vector2 spawnPosition, float delay)
        {
            StartCoroutine(DelaySpawn(delay, spawnPosition));
        }

        IEnumerator DelaySpawn(float delay)
        {
            yield return new WaitForSeconds(delay);

            Spawn();
        }

        IEnumerator DelaySpawn(float delay, Vector2 spawnPosition)
        {
            yield return new WaitForSeconds(delay);

            Spawn(spawnPosition);
        }

        private void init()
        {
            r = player.GetComponent<SpriteRenderer>();
            rb = player.GetComponent<Rigidbody2D>();
            t = player.transform;
        }

        void Start()
        {
            if (!player)
            {
                player = gameObject;
            }

            startPosition = player.transform.position;

            init();
        }
    }
}