using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class HFSavingManager
{
    /// <summary>
    /// Save the gamemanager in one new PlayerDataNew into a file in a prefixed location in the memory.
    /// </summary>
    public static void Save(HFPlayerData player)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/player.fun";

        FileStream stream = new FileStream(path, FileMode.Create);

        HFPlayerData data = new HFPlayerData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    /// <summary>
    /// Search in the path if exist a rescue. If it finds the file return him, if not take all levels boolean and save them.
    /// </summary>
    public static HFPlayerData Load()
    {
        string path = Application.persistentDataPath + "/player.fun";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            HFPlayerData data = formatter.Deserialize(stream) as HFPlayerData;
            stream.Close();

            return data;
        }
        else
        {
            //GameManager.Instance.SetWorldBooleans();
            //Save(GameManager.Instance);
            return null;
        }
    }
}


[System.Serializable]
public class HFPlayerData
{
    public string PlayerName = "";

    public int LevelsCompletedCounter = 0;

    //Chosen units
    //Chosen perks
    //Other stuff


    public HFPlayerData(HFPlayerData player)
    {

    }


    public void SavePlayerData()
    {
        HFSavingManager.Save(this);
    }

}
