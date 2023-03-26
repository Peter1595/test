using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test.Game
{
    [System.Serializable]
    public struct EnemyData
    {
        public EnemyBase enemyObject;
        public float weight;
    }

    public class EnemiesSpawner : MonoBehaviour
    {
        [Header("Parameters")]
        public List<EnemyData> EnemyTypes;
        public float minYPosition = 0;
        public float startMaxWeight = 0;
        public float timeBetweenSpawn;
        public int maxEnemiesPerSpawn;
        public float distanceFromPlayer;

        [Header("Refrences")]
        public BoxCollider2D boxCollider;
        public Rigidbody2D playerRB;
        public LayerMask enemiesLayerMask;
        public LayerMask playerLayerMask;

        float tiemToSpawn = 0;

        float currentWeight = 0;

        public float CurrentWeight
        {
            get
            {
                return currentWeight;
            }
            set
            {
                currentWeight = value;
            }
        }

        float maxWeight = 0;

        public float MaxWeight
        {
            get
            {
                return maxWeight;
            }
            set
            {
                maxWeight = value;
            }
        }

        public List<EnemyData> GetOptionalEnemies(float extraWeight = 0)
        {
            List<EnemyData> optionalEnemies =
            EnemyTypes.FindAll(x =>
                x.weight + extraWeight + CurrentWeight <= MaxWeight
            );

            // Debug.Log($"Current Weight: {CurrentWeight}"
            // + $"\n Extra Weigth: {extraWeight}"
            // + $"\n Optional Enemies: {optionalEnemies}"
            // + $"\n Max Weight: {MaxWeight}");

            return
            optionalEnemies;
        }

        Vector2 startBoxSize;

        void FixPosition()
        {
            float magnitude =
            (playerRB.velocity.magnitude
            * distanceFromPlayer);

            boxCollider.transform.localPosition =
            playerRB.velocity.normalized
            * magnitude;

            boxCollider.size =
            startBoxSize
            * (1 + (magnitude / 100));

            if (boxCollider.transform.position.y < minYPosition)
            {
                transform.position =
                new Vector2(
                    transform.position.x
                    , minYPosition
                );
            }

            if (boxCollider.transform.position.y
            - boxCollider.offset.y
            - (boxCollider.size.y / 2) < minYPosition)
            {
                boxCollider.transform.position =
                new Vector2(
                    transform.position.x
                    , minYPosition
                )
                + new Vector2(
                    0
                    , (boxCollider.size.y / 2) - boxCollider.offset.y
                );
            }
        }

        Vector4 GetBox()
        {
            return
            new Vector4(
                -boxCollider.size.x / 2
                , boxCollider.size.x / 2

                , -boxCollider.size.y / 2
                , boxCollider.size.y / 2
            );
        }
        Vector2 GetCenter()
        {
            return
            (Vector2)boxCollider.transform.position
            + boxCollider.offset;
        }

        Vector2 GetRandomPosition(Vector4 box, Vector2 center)
        {
            float x =
            Random.Range(
                box.x
                , box.y
            );

            float y =
            Random.Range(
                box.z
                , box.w
            );

            Vector2 position =
            center
            + new Vector2(
                x
                , y
            );

            return
            position;
        }

        bool CheckPosition(Vector2 position, Vector2 scale)
        {
            bool check =
            !Physics2D.BoxCast(
                position
                , scale * Random.Range((int)3, (int)30)
                , 0
                , Vector2.zero
                , 0
                , enemiesLayerMask
            )
            && !Physics2D.BoxCast(
                position
                , scale * 50
                , 0
                , Vector2.zero
                , 0
                , playerLayerMask
            );

            return check;
        }

        void OnEnemyDies(EnemyData enemy)
        {
            CurrentWeight -= enemy.weight;
        }

        void SpawnEnemy(EnemyData enemy, Vector2 position)
        {
            EnemyBase enemyObject = Instantiate(enemy.enemyObject, position, Quaternion.identity);

            enemyObject.OnDie +=
            () =>
            {
                OnEnemyDies(enemy);
            };

            CurrentWeight += enemy.weight;
        }

        void SpawnEnemies(EnemyData[] enemies)
        {
            Vector4 box =
            GetBox();

            Vector2 center =
            GetCenter();

            foreach (EnemyData enemy in enemies)
            {
                Vector2 position =
                GetRandomPosition(box, center);

                if (!CheckPosition(position, enemy.enemyObject.transform.localScale))
                {
                    continue;
                }

                SpawnEnemy(enemy, position);
            }



        }

        EnemyData ChooseEnemy(EnemyData[] optionalEnemies)
        {
            EnemyData enemy = optionalEnemies[Random.Range(0, optionalEnemies.Length)];

            return enemy;
        }

        EnemyData[] ChooseEnemies(int maxEnemies)
        {
            List<EnemyData> enemies = new List<EnemyData>();

            float choosenEnemiesWeight = 0;

            for (int i = 0; i < maxEnemies; i++)
            {
                EnemyData[] optionalEnemies = GetOptionalEnemies(choosenEnemiesWeight).ToArray();

                if (optionalEnemies.Length <= 0)
                {
                    break;
                }

                EnemyData enemy = ChooseEnemy(optionalEnemies);

                choosenEnemiesWeight += enemy.weight;

                enemies.Add(enemy);
            }

            return enemies.ToArray();
        }

        void Start()
        {
            MaxWeight = startMaxWeight;

            startBoxSize = boxCollider.size;
        }

        void Update()
        {
            FixPosition();

            if (Time.time >= tiemToSpawn)
            {
                if (!playerRB.gameObject.activeSelf)
                {
                    return;
                }

                SpawnEnemies(
                    ChooseEnemies(maxEnemiesPerSpawn)
                );

                tiemToSpawn = Time.time + timeBetweenSpawn;
            }
        }
    }
}