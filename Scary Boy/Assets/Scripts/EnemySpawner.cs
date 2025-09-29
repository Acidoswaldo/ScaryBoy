using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScaryGame
{
    public class EnemySpawner : MonoBehaviour
    {
        // ======= Pooling variables ======//
        [Header("POOLING VARIABLES")]
        [SerializeField] Enemy _prefab;
        [SerializeField] Transform _prefabHolder;
        [SerializeField] int _poolSize;
        public Dictionary<string, Queue<Enemy>> PoolDictionary;
        public string _poolTag = "enemy";
        // ===== end Pooling variables ====//

        [Header("ENEMY SPAWNING")]
        [SerializeField] Transform _mainCharacter;
        [SerializeField] float _spawnRadius;
        [SerializeField] EnemyTemplate[] _enemyTemplates;
        bool _spawning = true;
        [SerializeField] float _waitTime = 5f;


        [Header("MAX ENEMY HANDLERS")]
        [SerializeField] int _maxNumberOfSpawns = 3;
        [SerializeField] int maxNumberOfMedEnemies;
        [HideInInspector] public int numberOfMedEnemies;
        [SerializeField] int maxNumberOfBigEnemies;
        [HideInInspector] public int numberOfBigEnemies;
        [SerializeField] int maxNumberOfHugeEnemies;
        [HideInInspector] public int numberOfHugeEnemies;

        [Header("RAMPING DIFICULTY")]
        [SerializeField] float timer = 0f;
        [SerializeField] bool[] difficultyStages;

        [Header("OTHER VARIABLES")]
        bool _pause;

        private void Awake()
        {
            PoolDictionary = new Dictionary<string, Queue<Enemy>>();
            _mainCharacter = FindObjectOfType<CharacterControls>().transform;
            InsantiatePools();
        }
        void InsantiatePools()
        {
            Queue<Enemy> ObjectPool = new Queue<Enemy>();
            for (int i = 0; i < _poolSize; i++)
            {
                Enemy obj = Instantiate(_prefab);
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(_prefabHolder.transform);
                obj.transform.localPosition = Vector3.zero;
                ObjectPool.Enqueue(obj);
            }
            PoolDictionary.Add(_poolTag, ObjectPool);
        }

        Enemy SpawnFroomPool(string tag)
        {
            if (!PoolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("pool With tag" + tag + " doesn't exist");
                return null;
            }
            Enemy ObjectToSpawn = PoolDictionary[tag].Dequeue();
            ObjectToSpawn.gameObject.SetActive(true);
            //PoolDictionary[tag].Enqueue(ObjectToSpawn);
            return ObjectToSpawn;
        }

        private void Start()
        {
            GameManager.OnGameStateChanged += OngameStateChanged;

        }
        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= OngameStateChanged;
        }
        void OngameStateChanged(GameManager.GameState state)
        {
            if (state != GameManager.GameState.running)
            {
                _pause = true;
                return;
            }
            _pause = false;
            StartCoroutine(SpawnEnemies());
        }
        private void Update()
        {
            if (_pause) return;
            timer += Time.deltaTime;
            if (timer >= 30f && !difficultyStages[0])
            {
                difficultyStages[0] = true;
                _waitTime = 4.5f;
            }
            if (timer >= 60f && !difficultyStages[1])
            {
                difficultyStages[1] = true;
                _waitTime = 4.0f;
            }
            if (timer >= 90f && !difficultyStages[2])
            {
                difficultyStages[2] = true;
                _waitTime = 3.5f;
            }
            if (timer >= 120f && !difficultyStages[3])
            {
                difficultyStages[3] = true;
                _waitTime = 3.0f;
            }
            if (timer >= 150f && !difficultyStages[4])
            {
                difficultyStages[4] = true;
                _waitTime = 2.5f;
            }
            if (timer >= 180f && !difficultyStages[5])
            {
                difficultyStages[5] = true;
                _waitTime = 2.5f;
                _maxNumberOfSpawns = 4;
            }
            if (timer >= 240f && !difficultyStages[6])
            {
                difficultyStages[6] = true;
                _waitTime = 2.5f;
                _maxNumberOfSpawns = 5;
            }
            if (timer >= 300f && !difficultyStages[7])
            {
                difficultyStages[7] = true;
                _waitTime = 2.0f;
                _maxNumberOfSpawns = 5;
            }
            if (timer >= 360f && !difficultyStages[8])
            {
                difficultyStages[8] = true;
                _waitTime = 2.0f;
                _maxNumberOfSpawns = 6;
            }

        }

        IEnumerator SpawnEnemies()
        {
            while (_spawning && GameManager.instance.gameState == GameManager.GameState.running)

            {
                int randomEnemyNumber = Random.Range(1, _maxNumberOfSpawns);
                for (int i = 0; i < randomEnemyNumber; i++)
                {
                    SpawnEnemy(_enemyTemplates[RandomEnemy()]);
                }
                yield return new WaitForSeconds(_waitTime);
            }
        }

        void SpawnEnemy(EnemyTemplate enemyTemplate)
        {
            Enemy enemy = SpawnFroomPool(_poolTag);
            enemy.transform.position = RandomCircle(_mainCharacter.position, _spawnRadius);
            enemy.SetEnemy(_mainCharacter, enemyTemplate);
        }

        Vector3 RandomCircle(Vector3 center, float radius)
        {
            float ang = Random.value * 360;
            Vector3 pos;
            pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            pos.z = center.z;
            return pos;
        }

        int RandomEnemy()
        {
            int enemy = 0;
            int rand = Random.Range(0, 100);
            if (rand >= 0 && rand < 40)
            {
                enemy = 0;
            }
            if (rand >= 40 && rand < 60)
            {
                enemy = 1;
            }
            if (rand >= 60 && rand < 75)
            {
                if (numberOfMedEnemies >= maxNumberOfMedEnemies)
                {
                    enemy = 0;
                }
                else
                {
                    enemy = 2;
                }
            }
            if (rand >= 75 && rand < 85)
            {
                if (numberOfBigEnemies >= maxNumberOfBigEnemies)
                {
                    enemy = 0;
                }
                else
                {
                    enemy = 3;
                }
            }
            if (rand >= 85 && rand < 90)
            {
                if (numberOfHugeEnemies >= maxNumberOfHugeEnemies)
                {
                    enemy = 0;
                }
                else
                {
                    enemy = 4;
                }
            }
            if (rand >= 90)
            {
                enemy = 5;
            }
            return enemy;

        }
    }


}

