using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem 
{
    public static string GetPath(string name)
    {
        return Application.persistentDataPath + name + ".eden";
    }

    public static void SavePlayer(PlayerManager player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetPath(GameController.currentSaveFileName);
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);
        formatter.Serialize(stream, data);
        stream.Close();

    }

    public static PlayerData LoadPlayer(string name)
    {
        string path = GetPath(name);

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            return data;
        } else
        {
            Debug.LogError("Save file not found in " + path);
            GameUtilities.ShowMessage(MessageLevel.Error, "Save file not found in " + path);
            return null;
        }
    }
}
