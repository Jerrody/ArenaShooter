using System;
using Characters.Enemy.Components;
using Game.Characters;
using Game.Characters.Components;
using Game.Global.Data;
using UnityEngine;
using UnityEngine.AI;

namespace Characters.Enemy
{
    public sealed class EnemyController : EntityController
    {
        public event Action AttackEvent;
        public event Action DeathEvent;

        public static LayerMask layerMask { get; private set; }

        [Header("References")] [SerializeField]
        private HitAreaComponent hitArea;

        [Header("Layers")] [SerializeField] private LayerMask targetLayer;

        [Header("Stats")] [SerializeField] private float timeBetweenAttacks;
        [SerializeField] private float attackRange = 2.0f;

        public Transform target;

        private NavMeshAgent _agent;
        private EnemyAnimationController _enemyAnimationController;

        public float velocity => _agent.velocity.z;

        private bool _alreadyAttacked;

        private bool _targetInAttackRange;

        private void Awake()
        {
            layerMask = LayerMask.NameToLayer("Enemy");

            health = GetComponent<HealthComponent>();
            health.DeathEvent += OnDeath;

            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = walkSpeed;

            _enemyAnimationController = GetComponentInChildren<EnemyAnimationController>();
            _enemyAnimationController.AttackAnimationEndEvent += OnAttackAnimationEnd;
            _enemyAnimationController.DeathAnimationEndEvent += OnDeathAnimationEnd;
        }

        private void FixedUpdate()
        {
            var position = transform.position;
            _targetInAttackRange = Physics.CheckSphere(position, attackRange, targetLayer);

            if (_targetInAttackRange)
                _agent.SetDestination(transform.position);
        }

        private void Update()
        {
            if (!_targetInAttackRange)
            {
                Chase();
            }
            else
            {
                Attack();
            }
        }

        private void OnAttackAnimationEnd()
        {
            hitArea.gameObject.SetActive(false);
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

        private void OnDeathAnimationEnd()
        {
            Data.AddKill();
            Destroy(gameObject);
        }

        private void Chase()
        {
            _alreadyAttacked = false;
            _agent.SetDestination(target.position);
        }

        private void Attack()
        {
            var currentTransform = transform;
            var currentRotation = currentTransform.rotation;

            currentTransform.LookAt(target);

            var newRotation = currentTransform.rotation;
            currentTransform.rotation = currentRotation;

            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, walkSpeed * 2.0f * Time.deltaTime);

            if (_alreadyAttacked) return;

            hitArea.gameObject.SetActive(true);
            AttackEvent?.Invoke();
            _alreadyAttacked = true;
        }

        private void ResetAttack()
        {
            _alreadyAttacked = false;
        }

        private void OnDeath()
        {
            enabled = false;
            target = transform;
            DeathEvent?.Invoke();
        }
    }
}