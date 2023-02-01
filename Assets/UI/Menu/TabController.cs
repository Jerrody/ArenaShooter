using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public enum TabType : uint
    {
        Play,
        Customize,
    }

    public abstract class TabController : MonoBehaviour
    {
        [Header("Info")] [SerializeField] protected TabType tabType;
    }
}