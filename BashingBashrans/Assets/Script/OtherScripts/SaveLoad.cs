using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad{

    public static Game savedGame;

    public static void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedGame.igs");
        bf.Serialize(file, SaveLoad.savedGame);
        file.Close();
    }

    public static void Load()
    {
        if (!File.Exists(Application.persistentDataPath + "/savedGame.igs"))
        {
            savedGame = new Game();
            Save();
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/savedGame.igs", FileMode.Open);
        SaveLoad.savedGame = (Game)bf.Deserialize(file);
        file.Close();
    }
}
