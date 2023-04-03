using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class GameSaver 
{
    private static string Path
    {
        get
        {
            return System.IO.Path.Combine(Application.dataPath, "save.json"); 
            
        }
    }

    public static void SaveGame(GameState gameState)
    {
        var json = JsonConvert.SerializeObject(gameState);
        File.WriteAllText(Path,json);
        
    }

    public static GameState LoadGame()
    {
        if (File.Exists(Path))
        {
            string json = File.ReadAllText(Path);
            GameState state = JsonConvert.DeserializeObject<GameState>(json);
            return state;
        }

        return null;
    }
    
    
}
