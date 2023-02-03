using System;
using System.Linq;
using UnityEngine;

namespace Game.UI.Menu.Tabs
{
    public sealed class TabsController : MonoBehaviour
    {
        public Action<uint> TabChangeEvent;

        [SerializeField] private TabController[] tabControllers = new TabController[2];
        private TabController _currentTabController;

        private void Awake()
        {
            TabChangeEvent += OnTabChange;

            foreach (var tabController in tabControllers)
            {
                tabController.DisableTab();
            }

            if ((_currentTabController = tabControllers.First()) == null)
                throw new NullReferenceException("Empty array of `_tabControllers`");
            _currentTabController.EnableTab();
        }

        private void OnTabChange(uint tabIndex)
        {
            _currentTabController.DisableTab();

            _currentTabController = tabControllers[tabIndex];
            _currentTabController.EnableTab();
        }
    }
}