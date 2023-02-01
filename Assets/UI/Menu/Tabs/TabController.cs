using System;
using UnityEngine;

namespace Game.UI.Menu.Tabs
{
    [Serializable]
    public enum TabType : uint
    {
        Play,
        Customize,
    }

    public abstract class TabController : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private GameObject tab;

        [Header("Info")] [SerializeField] protected TabType tabType;

        public void OnClick()
        {
            TabsController.TabChangeEvent?.Invoke((uint)tabType);
        }

        public void DisableTab()
        {
            tab.gameObject.SetActive(false);
        }

        public void EnableTab()
        {
            tab.gameObject.SetActive(true);
        }
    }
}