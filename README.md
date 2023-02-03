# Welcome

This is a game made in 37 hours!
Prebuild game version for Windows can be found [here](https://drive.google.com/drive/folders/1pMYLDrPvMPC-EO5E-T0ETQBeSHiKHsW7?usp=sharing).

## Unity Version

Currently used version 2022.2.3f1

### NOTE

In Editor mode could appear unexpected internal engine warnings that cannot be affected, like:

```cs
[Worker0] Internal: There are remaining Allocations on the JobTempAlloc. This is a leak, and will impact performance
```

```cs
[Worker0] To Debug, run app with -diag-job-temp-memory-leak-validation cmd line argument. This will output the callstacks of the leaked allocations.
```

## Features

- Data Storage
- Weapon System
- Health System
- Enemies
- Attachment system (more like hardcoded than flexible)
- Progress System
- Particle System for the weapons (not so much)
- Wave Gamemode

## Controls

- WASD - movement
- Space - Jump
- Left Click - Shoot
- Right Click - Aim
- R - Reload
- Shift - Run and Zoom (in Aim mode)
- F1/Escape - Menu

## Code Style

Access order in code:

```cs
    public float x;

    protected  bool Y;

    private uint _z;
```

`const`, `static` and local variables:

```cs
const string Foo;

static uint Bar;

float x;
```

```cs
using System;
using System.Collections;
using Game.Characters.Components;
using Game.Global.Data;
using Game.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Characters.Player
{
    [RequireComponent(typeof(PlayerInput), typeof(CharacterController))]
    public sealed class FooController : MonoBehaviour
    {
        public const string Name = "Foo";

        public static LayerMask layerMask { get; private set; }

        public event Action EscapePressedEvent;

        public Action<bool> AimEvent;

        [Header("Stats")]
        [SerializeField] private AnimationCurve jumpFallOff;
        [SerializeField] private float runSpeed = 30.0f;

        [Header("Preferences")]
        [SerializeField]
        private float mouseSensitivity = 15.0f;

        public bool isMoving => _controller.velocity.z is > float.Epsilon or < -float.Epsilon;

        public WeaponHolderController weaponHolderController { get; private set; }
        public bool isAiming { get; private set; }

        // References
        private CharacterController _controller;

        private float _speed;
        private bool _isJumping;


        // Unity `MonoBehaviour` events.
        public void Awake() {}

        private void Start() {}

        private void Update() {}

        // Action Events or Input Events.
        public void Move(InputAction.CallbackContext ctx) {}

        // Access order.
        public void Hi()
        {
            print("Hi!");
        }

        // First goes parent methods and after child classes in acess order.
        protected override void Bey() {}
        protected void Wow() {}
        
        private static void OnDeath() {}

        private void SetCameraFieldOfViewAndZoom() {}
    }
}
```
