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
        [Header("References")]
        [SerializeField] private GameObject tab;

        [Header("Info")]
        [SerializeField] protected TabType tabType;

        private TabsController _tabsController;

        private void Awake()
        {
            _tabsController = GetComponentInParent<TabsController>();
        }

        public void OnClick()
        {
            _tabsController.TabChangeEvent?.Invoke((uint)tabType);
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