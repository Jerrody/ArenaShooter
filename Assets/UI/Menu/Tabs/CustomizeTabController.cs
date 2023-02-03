using System;
using System.Collections.Generic;
using Game.Global.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Menu.Tabs
{
    public sealed class CustomizeTabController : TabController
    {
        [Header("References")]
        [SerializeField] private TMP_Text selectedWeaponText;
        [SerializeField] private Image weaponImage;
        [SerializeField] private List<Sprite> weaponSprites = new(3);
        [SerializeField] private Image noScope;
        [SerializeField] private Image scopeDot;

        private int _currentIndex;

        private void Start()
        {
            if (!Data.jsonData.isOpenedScope)
                scopeDot.gameObject.SetActive(false);

            weaponImage.sprite = weaponSprites[_currentIndex];
            selectedWeaponText.text = $"3 / {_currentIndex + 1}";

            SetCurrentWeaponScope();
        }

        public void OnArrowUpClick()
        {
            _currentIndex = (_currentIndex + 1) % weaponSprites.Count;
            weaponImage.sprite = weaponSprites[_currentIndex];
            selectedWeaponText.text = $"3 / {_currentIndex + 1}";
            SetCurrentWeaponScope();
        }

        public void OnArrowDownClick()
        {
            if (_currentIndex - 1 < 0)
            {
                _currentIndex = 2;
            }
            else
            {
                _currentIndex = (_currentIndex - 1) % weaponSprites.Count;
            }

            weaponImage.sprite = weaponSprites[_currentIndex];
            selectedWeaponText.text = $"3 / {_currentIndex + 1}";
            SetCurrentWeaponScope();
        }

        public void OnNoScopeClick()
        {
            var color = Color.gray;
            noScope.color = color;
            color.a = 0.0f;
            scopeDot.color = color;

            Data.SetWeaponScope(_currentIndex, Scope.NoScope);
        }

        public void OnScopeDotClick()
        {
            var color = Color.gray;
            scopeDot.color = color;
            color.a = 0.0f;
            noScope.color = color;

            Data.SetWeaponScope(_currentIndex, Scope.ScopeDot);
        }

        private void SetCurrentWeaponScope()
        {
            var color = Color.gray;
            color.a = 0.0f;
            noScope.color = color;
            scopeDot.color = color;

            color.a = 1.0f;
            switch (Data.jsonData.weaponData[_currentIndex].currentScope)
            {
                case Scope.NoScope:
                    noScope.color = color;
                    break;
                case Scope.ScopeDot:
                    scopeDot.color = color;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}