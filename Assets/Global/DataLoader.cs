using Game.Global.Data;
using UnityEngine;

namespace Game
{
    public sealed class DataLoader : MonoBehaviour
    {
        private void Awake()
        {
            Data.Load();
            Destroy(this);
        }
    }
}