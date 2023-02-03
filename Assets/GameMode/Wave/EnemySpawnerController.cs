using System;
using System.Collections;
using Characters.Enemy;
using UnityEngine;

namespace Game.Gamemode.Wave
{
    public sealed class EnemySpawnerController : MonoBehaviour
    {
        public event Action WaveEndEvent;
        public event Action EnemyDeathEvent;

        [Header("References")] [SerializeField]
        private EnemyController enemy;

        [Header("Stats")] [SerializeField] private float spawnTime = 1.0f;

        private uint _enemyCount;
        private uint _spawnedEnemiesCount = 1;

        public void SpawnEnemy(Transform target, uint count)
        {
            for (var i = 0; i < count; i++)
            {
                StartCoroutine(Spawn(target, count));
            }
        }

        private IEnumerator Spawn(Transform target, uint count)
        {
            yield return new WaitForSeconds(spawnTime);

            var position = transform.position;
            var spawnedEnemy = Instantiate(enemy, position, Quaternion.identity);
            spawnedEnemy.target = target;
            spawnedEnemy.DeathEvent += () => EnemyDeathEvent?.Invoke();

            if (_spawnedEnemiesCount == count)
            {
                _spawnedEnemiesCount = 0;
                WaveEndEvent?.Invoke();
            }
            else
            {
                _spawnedEnemiesCount++;
            }
        }
    }
}