using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Global.Data
{
    public enum Scope : uint
    {
        NoScope,
        ScopeDot,
    }

    [System.Serializable]
    public sealed class WeaponData
    {
        public Scope currentScope = Scope.NoScope;

        public bool isScopeSet => currentScope == Scope.ScopeDot;

        public void SetScope(Scope scope)
        {
            currentScope = scope;
        }
    }

    [System.Serializable]
    public sealed class JsonData
    {
        public uint kills;
        public uint deaths;
        public uint wins;
        public uint loses;
        public uint killsToOpenScope = 10;

        [SerializeField]
        public List<WeaponData> weaponData = new() { new WeaponData(), new WeaponData(), new WeaponData() };

        public bool isOpenedScope => kills >= killsToOpenScope;

        public void SetKills(uint kill)
        {
            kills = kill;
        }

        public void SetDeaths(uint death)
        {
            deaths = death;
        }

        public void SetWins(uint win)
        {
            wins = win;
        }

        public void SetLoses(uint lose)
        {
            loses = lose;
        }

        public void SetWeaponData(int weaponIndex, Scope scope)
        {
            weaponData[weaponIndex].SetScope(scope);
        }
    }

    public static class Data
    {
#if UNITY_STANDALONE
        private static readonly string PathToFile =
            Application.persistentDataPath + Path.AltDirectorySeparatorChar + "data.json";

        private static readonly string PathToDir = Application.persistentDataPath + Path.AltDirectorySeparatorChar;
#else
        private static readonly string PathToFile = Application.dataPath + Path.AltDirectorySeparatorChar +
                                                    "data.json";
        private static readonly string PathToDir = Application.dataPath + Path.AltDirectorySeparatorChar;
#endif

        public static JsonData jsonData { get; private set; } = new();

        public static void Load()
        {
            if (!File.Exists(PathToFile))
            {
                Directory.CreateDirectory(PathToDir);
                File.Create(PathToFile).Close();
                Save();
            }

            var reader = new StreamReader(PathToFile);
            var json = reader.ReadToEnd();
            reader.Close();
            jsonData = JsonUtility.FromJson<JsonData>(json);
        }

        private static void Save()
        {
            var json = JsonUtility.ToJson(jsonData);

            var writer = new StreamWriter(PathToFile);
            writer.Write(json);
            writer.Close();
        }

        public static void AddKill()
        {
            jsonData.SetKills(jsonData.kills + 1);
            Save();
        }

        public static void AddDeath()
        {
            jsonData.SetDeaths(jsonData.deaths + 1);
            Save();
        }

        public static void AddWin()
        {
            jsonData.SetWins(jsonData.wins + 1);
            Save();
        }

        public static void AddLose()
        {
            jsonData.SetLoses(jsonData.loses + 1);
            Save();
        }

        public static void SetWeaponScope(int weaponIndex, Scope scope)
        {
            jsonData.SetWeaponData(weaponIndex, scope);
            Save();
        }
    }
}