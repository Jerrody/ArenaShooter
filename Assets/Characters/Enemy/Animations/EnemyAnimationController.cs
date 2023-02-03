using System;
using UnityEngine;

namespace Characters.Enemy
{
    public sealed class EnemyAnimationController : MonoBehaviour
    {
        public Action AttackAnimationEndEvent;
        public Action DeathAnimationEndEvent;

        private static readonly int VelocityParam = Animator.StringToHash("Velocity");
        private static readonly int AttackParam = Animator.StringToHash("Attack");
        private static readonly int DeadParam = Animator.StringToHash("Dead");

        private EnemyController _enemyController;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _enemyController = GetComponentInParent<EnemyController>();
        }

        private void Start()
        {
            _enemyController.AttackEvent += AttackEvent;
            _enemyController.DeathEvent += DeathEvent;
        }

        private void Update()
        {
            _animator.SetFloat(VelocityParam, _enemyController.velocity);
        }


        public void AttackAnimationEnd()
        {
            AttackAnimationEndEvent?.Invoke();
        }

        public void DeathAnimationEnd()
        {
            DeathAnimationEndEvent?.Invoke();
        }

        private void DeathEvent()
        {
            _enemyController.DeathEvent -= DeathEvent;
            _animator.SetTrigger(DeadParam);
        }

        private void AttackEvent()
        {
            _animator.SetTrigger(AttackParam);
        }
    }
}