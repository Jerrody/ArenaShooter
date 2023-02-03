using System;
using Game.Utils;
using UnityEngine;

namespace Game.Gamemode.Wave
{
    public sealed class WaveManager : MonoBehaviour
    {
        public Action WavesEndEvent;

        [Header("References")] [SerializeField]
        private EnemySpawnerController[] enemySpawnerControllers;

        [SerializeField] private Transform target;

        [Header("Stats")] [SerializeField] private uint spawnEnemies;
        [SerializeField] private uint totalWaves = 3;
        [SerializeField] private float timeBeforeNextWave = 5.0f;

        private uint enemiesPerSpawnPoint => (uint)(spawnEnemies / enemySpawnerControllers.Length) / totalWaves;

        private uint _wavesCounter = 1;
        private uint _enemyCounter;
        private float _nextTimeToStartWave;

        private void Awake()
        {
            _nextTimeToStartWave = timeBeforeNextWave;
            foreach (var enemySpawnerController in enemySpawnerControllers)
                enemySpawnerController.WaveEndEvent += OnWaveEnd;
            foreach (var enemySpawnerController in enemySpawnerControllers)
                enemySpawnerController.EnemyDeathEvent += OnEnemyDeath;
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
            if (_wavesCounter == totalWaves)
            {
                foreach (var enemySpawnerController in enemySpawnerControllers)
                    enemySpawnerController.WaveEndEvent -= OnWaveEnd;
                return;
            }

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
            _enemyCounter++;
            if (_enemyCounter == spawnEnemies)
                Timer.Create(() => WavesEndEvent?.Invoke(), 5.0f);
        }
    }
}