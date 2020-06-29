using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Schema;
using UnityEngine;

public static class HFSavingManager
{
    #region Old stuff
    //public static void Save(HFPlayerData inPlayerData)
    //{
    //    BinaryFormatter formatter = new BinaryFormatter();

    //    string path = Application.persistentDataPath + "/player.fun";
    //    Debug.Log("saved in :" + path);

    //    FileStream stream = new FileStream(path, FileMode.Create);

    //    HFPlayerData data = new HFPlayerData(inPlayerData);

    //    formatter.Serialize(stream, data);
    //    stream.Close();
    //}

    /// <summary>
    /// Search in the path if exist a rescue. If it finds the file return him, if not take all levels boolean and save them.
    /// </summary>
    //public static HFPlayerData Load()
    //{
    //    string path = Application.persistentDataPath + "/player.fun";
    //    if (File.Exists(path))
    //    {
    //        BinaryFormatter formatter = new BinaryFormatter();
    //        FileStream stream = new FileStream(path, FileMode.Open);

    //        HFPlayerData data = formatter.Deserialize(stream) as HFPlayerData;
    //        stream.Close();

    //        return data;
    //    }
    //    else
    //    {
    //        HFPlayerData newPlayerData = new HFPlayerData();
    //        HFGameManager.Instance.PlayerData = newPlayerData;
    //        HFGameManager.Instance.PlayerData.SavePlayerData();
    //        return null;
    //    }
    //}
    #endregion

    public static bool SaveGame(HFPlayerData saveGame, string name)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Create))
        {
            try
            {
                formatter.Serialize(stream, saveGame);
            }
            catch (Exception)
            {
                return false;
            }
        }
        return true;
    }

    public static HFPlayerData LoadGame(string name)
    {
        if (!DoesSaveGameExist(name))
        {
            HFPlayerData newPlayerData = new HFPlayerData(name, 1);
        }


        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Open))
        {
            try
            {
                return formatter.Deserialize(stream) as HFPlayerData;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public static bool DeleteSaveGame(string name)
    {
        try
        {
            File.Delete(GetSavePath(name));
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public static bool DoesSaveGameExist(string name)
    {
        return File.Exists(GetSavePath(name));
    }

    private static string GetSavePath(string name)
    {
        return Path.Combine(Application.persistentDataPath, name + ".sav");
    }
}



[System.Serializable]
public class HFPlayerData
{
    public string PlayerName = "NoName";

    private int m_levelsCompletedCounted = 0;
    public int LevelsCompletedCounter
    {
        get
        {
            ClampValue(m_levelsCompletedCounted);
            return m_levelsCompletedCounted;
        }
        set
        {
            m_levelsCompletedCounted = value;
            ClampValue(m_levelsCompletedCounted);
        }
    }

    //Chosen units
    //Chosen perks
    //Other stuff

    private int ClampValue(int inValue)
    {
        inValue = Mathf.Clamp(inValue, 1, HFScenesManager.Instance.LevelContainer.Levels.Count);
        return inValue;
    }

    public HFPlayerData(string inName, int inLevelCompleted)
    {
        PlayerName = inName;
        LevelsCompletedCounter = inLevelCompleted;
        InitSaveData();
    }

    private void InitSaveData()
    {
        HFSavingManager.SaveGame(this, PlayerName);
    }

    public void RefreshPlayerData()
    {
        LevelsCompletedCounter = HFScenesManager.Instance.LevelCompletedCount;
        HFSavingManager.SaveGame(this, PlayerName);
    }

}
