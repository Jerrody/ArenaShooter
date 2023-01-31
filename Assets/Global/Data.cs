using System.IO;
using UnityEngine;

namespace Game.Global.Data
{
    public enum Scopes : uint
    {
        NoScope,
        Scope,
    }

    public struct WeaponData
    {
        public Scopes CurrentScope;

        public bool isScopeSet => CurrentScope == Scopes.Scope;
    }

    public struct JsonData
    {
        public uint kills { get; private set; }
        public uint deaths { get; private set; }
        public uint wins { get; private set; }
        public uint loses { get; private set; }
        public WeaponData ak47Data { get; private set; }
        public WeaponData shotgunData { get; private set; }
        public WeaponData smgData { get; private set; }
        public bool IsOpenedScope;

        public void SetKills(uint kill)
        {
            kills += kill;
        }

        public void SetDeaths(uint deaths)
        {
            this.deaths += deaths;
        }

        public void SetWins(uint wins)
        {
            this.wins += wins;
        }

        public void SetLoses(uint loses)
        {
            this.loses += loses;
        }

        public void SetAk47Data(WeaponData ak47Data)
        {
            this.ak47Data = ak47Data;
        }

        public void SetShotgunData(WeaponData shotgunData)
        {
            this.shotgunData = shotgunData;
        }

        public void SetSmgData(WeaponData smgData)
        {
            this.smgData = smgData;
        }
    }

    public static class Data
    {
#if UNITY_STANDALONE
        private static readonly string PathToData =
            Application.persistentDataPath + Path.AltDirectorySeparatorChar + "data.json";
#else
        private static readonly string PathToData = Application.dataPath + System.IO.Path.AltDirectorySeparatorChar +
                                                    "Data/data.json";
#endif

        public static JsonData jsonData { get; private set; }

        public static void Save()
        {
            var savePath = PathToData;
            var json = JsonUtility.ToJson(jsonData);

            var writer = new StreamWriter(savePath);
            writer.Write(json);
        }

        public static void Load()
        {
            var reader = new StreamReader(PathToData);
            if (!File.Exists(PathToData))
                Save();

            var json = reader.ReadToEnd();
            jsonData = JsonUtility.FromJson<JsonData>(json);
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
    }
}