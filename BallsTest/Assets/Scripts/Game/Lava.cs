using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Test.Gameplay;

namespace Test.Game
{
    public class Lava : MonoBehaviour
    {
        private float Y;

        void KillPlayer(PlayerDeath pd)
        {
            pd.Die();
            pd.Spawn(3f);
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.transform.parent.TryGetComponent<PlayerDeath>(out PlayerDeath pd))
            {
                KillPlayer(pd);
            }
        }

        void Start()
        {
            Y = transform.position.y;
        }
        void Update()
        {
            transform.position =
            new Vector3(
                transform.position.x
                , Y
                , transform.position.z
            );
        }
    }
}