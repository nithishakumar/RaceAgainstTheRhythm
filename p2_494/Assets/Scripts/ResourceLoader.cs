using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader
{
    private static Dictionary<string, Sprite> loadedSprites = new();
    private static Dictionary<string, AudioClip> loadedAudio = new();
    private static Dictionary<string, GameObject> loadedPrefabs = new();
    private static Dictionary<string, string> filename2Key = new();
    private static string mainPath = "Rhythm/";

    static ResourceLoader()
    {
        //filename2Key["link_sprites_0"] = "downwardWalk";

        LoadAllSprites();
        LoadAudio();
        LoadPrefabs();
    }

    private static void LoadAllSprites()
    {
        //LoadSpritesFromSheet("link_sprites");

        string spritePath = mainPath + "Sprites/";

        loadedSprites["obstacle1"] = Resources.Load<Sprite>(spritePath + "obstacle1");
        loadedSprites["obstacle2"] = Resources.Load<Sprite>(spritePath + "obstacle2");
        loadedSprites["obstacle3"] = Resources.Load<Sprite>(spritePath + "obstacle3");
        loadedSprites["obstacle4"] = Resources.Load<Sprite>(spritePath + "obstacle4");
    }

    private static void LoadSpritesFromSheet(string sheet)
    {
        Sprite[] subSprites = Resources.LoadAll<Sprite>(mainPath + sheet);

        for (int i = 0; i < subSprites.Length; i++)
        {
            if (filename2Key.ContainsKey(subSprites[i].name))
            {
                string key = filename2Key[subSprites[i].name];
                loadedSprites[key] = subSprites[i];
            }
        }
    }

    private static void LoadAudio()
    {
        //string audioPath = mainPath + "Audio/";
        //loadedAudio["rupeeSfx"] = Resources.Load<AudioClip>(audioPath + "Sound Effect (14)");
    }

    private static void LoadPrefabs()
    {
        string prefabPath = mainPath + "Prefabs/";
        //loadedPrefabs["musicNote1"] = Resources.Load<GameObject>(prefabPath + "musicNote1");
    }

    public static Sprite GetSprite(string spriteName)
    {
        if (loadedSprites.ContainsKey(spriteName))
        {
            return loadedSprites[spriteName];
        }
        else
        {
            Debug.LogError("Sprite not found: " + spriteName);
            return null;
        }
    }

    public static AudioClip GetAudioClip(string audioClipName)
    {
        if (loadedAudio.ContainsKey(audioClipName))
        {
            return loadedAudio[audioClipName];
        }
        else
        {
            Debug.LogError("AudioClip not found: " + audioClipName);
            return null;
        }
    }

    public static GameObject GetPrefab(string prefabName)
    {
        if (loadedPrefabs.ContainsKey(prefabName))
        {
            return loadedPrefabs[prefabName];
        }
        else
        {
            Debug.LogError("Prefab not found: " + prefabName);
            return null;
        }
    }


}
