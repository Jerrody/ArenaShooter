using System;
using Game.Global.Data;
using TMPro;
using UnityEngine;

namespace Game.UI.Menu.Tabs
{
    public sealed class PlayTabController : TabController
    {
        [Header("References")]
        [SerializeField] private TMP_Text winsText;
        [SerializeField] private TMP_Text losesText;
        [SerializeField] private TMP_Text winrateText;
        [SerializeField] private TMP_Text killsText;
        [SerializeField] private TMP_Text deathsText;
        [SerializeField] private TMP_Text kdText;
        [SerializeField] private TMP_Text killsProgressBar;
        [SerializeField] private RectTransform progressBarTransform;

        private void Start()
        {
            var wins = Data.jsonData.wins;
            var loses = Data.jsonData.loses;
            var kills = Data.jsonData.kills;
            var deaths = Data.jsonData.deaths;

            var winsWithSingle = (float)wins;
            var losesWithSingle = (float)loses;
            var winrate = winsWithSingle /
                          (winsWithSingle + losesWithSingle == 0 ? 1 : winsWithSingle + losesWithSingle);

            var killsWithSingle = (float)kills;
            var deathsWithSingle = (float)deaths;
            var kd = killsWithSingle /
                     (killsWithSingle + deathsWithSingle == 0 ? 1 : killsWithSingle + deathsWithSingle);

            winsText.text += wins;
            losesText.text += loses;
            winrateText.text += winrate.ToString("0.0%");
            killsText.text += kills;
            deathsText.text += deaths;
            kdText.text += kd.ToString("0.0%");

            var killsToOpenScope = Data.jsonData.killsToOpenScope;
            var killsClamped = Math.Clamp(kills, 0, killsToOpenScope);
            killsProgressBar.text = $"{killsToOpenScope}/{killsClamped}";
            var progress = (float)killsClamped / killsToOpenScope;
            progressBarTransform.localScale = new Vector3(progress, 1.0f, 1.0f);
        }
    }
}