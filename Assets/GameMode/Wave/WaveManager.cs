using System;
using Game.Utils;
using UnityEngine;

namespace Game.Gamemode.Wave
{
    public sealed class WaveManager : MonoBehaviour
    {
        public static Action WavesEndEvent;
        public static Action EnemyDeathEvent;

        [Header("References")] [SerializeField]
        private EnemySpawnerController[] enemySpawnerControllers;

        [SerializeField] private Transform target;

        [Header("Stats")] [SerializeField] private uint spawnEnemies;
        [SerializeField] private uint totalWaves = 3;
        [SerializeField] private float timeBeforeNextWave = 5.0f;

        private uint enemiesPerSpawnPoint => (uint)(spawnEnemies / enemySpawnerControllers.Length);

        private uint _wavesCounter;
        private float _nextTimeToStartWave;
        private uint _enemyCounter;

        private void Awake()
        {
            _nextTimeToStartWave = timeBeforeNextWave;
            foreach (var enemySpawnerController in enemySpawnerControllers)
                enemySpawnerController.WaveEndEvent += OnWaveEnd;

            EnemyDeathEvent += OnEnemyDeath;
        }

        private void Start()
        {
            StartWave();
        }

        private void StartWave()
        {
            foreach (var enemySpawnerController in enemySpawnerControllers)
                enemySpawnerController.SpawnEnemy(target, enemiesPerSpawnPoint);
        }

        private void OnWaveEnd()
        {
            if (_wavesCounter == totalWaves * enemySpawnerControllers.Length)
                return;

            _wavesCounter++;
            Timer.Create(() =>
            {
                foreach (var enemySpawnerController in enemySpawnerControllers)
                    enemySpawnerController.SpawnEnemy(target, enemiesPerSpawnPoint);
            }, _nextTimeToStartWave);
            _nextTimeToStartWave += timeBeforeNextWave;
        }

        private void OnEnemyDeath()
        {
            print($"{_enemyCounter} and {spawnEnemies}");
            _enemyCounter++;
            if (_enemyCounter == spawnEnemies)
                WavesEndEvent?.Invoke();
        }
    }
}