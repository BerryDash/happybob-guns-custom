using System.IO;
using System.Numerics;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class BazookaManager : MonoBehaviour
{
    public static BazookaManager Instance;
    private bool firstLoadDone = false;
    public JObject saveFile = new()
    {
        ["version"] = "0"
    };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (!firstLoadDone)
            {
                firstLoadDone = true;
                Load();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save();
        }
    }

    public void Load()
    {
        string path = Path.Join(Application.persistentDataPath, "save.json");
        if (!File.Exists(path))
        {
            File.Create(path).Dispose();
        }
        else
        {
            try
            {
                var tempSaveFile = JObject.Parse(Encoding.UTF8.GetString(File.ReadAllBytes(path)));
                if (tempSaveFile != null) saveFile = tempSaveFile;
            }
            catch
            {
                Debug.LogWarning("Failed to load save file");
            }
        }
    }

    public void Save()
    {
#if UNITY_EDITOR
        return;
#else
        string path = Path.Join(Application.persistentDataPath, "save.json");
        var encoded = Encoding.UTF8.GetBytes(saveFile.ToString(Newtonsoft.Json.Formatting.None));
        using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        fileStream.Write(encoded, 0, encoded.Length);
        fileStream.Flush(true);
#endif
    }

    //Settings stuff

    public void SetSettingFullScreen(bool value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        saveFile["settings"]["fullScreen"] = value;
    }

    public bool GetSettingFullScreen()
    {
        if (saveFile["settings"] == null) return true;
        if (saveFile["settings"]["fullScreen"] == null) return true;
        return bool.Parse(saveFile["settings"]["fullScreen"].ToString());
    }

    public void SetSettingVsync(bool value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        saveFile["settings"]["vsync"] = value;
    }

    public bool GetSettingVsync()
    {
        if (saveFile["settings"] == null) return true;
        if (saveFile["settings"]["vsync"] == null) return true;
        return bool.Parse(saveFile["settings"]["vsync"].ToString());
    }

    public void SetSettingRandomMusic(bool value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        saveFile["settings"]["randomMusic"] = value;
    }

    public bool GetSettingRandomMusic()
    {
        if (saveFile["settings"] == null) return true;
        if (saveFile["settings"]["randomMusic"] == null) return true;
        return bool.Parse(saveFile["settings"]["randomMusic"].ToString());
    }

    public void SetSettingMusicVolume(float value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        saveFile["settings"]["musicVolume"] = value;
    }

    public float GetSettingMusicVolume()
    {
        if (saveFile["settings"] == null) return 1f;
        if (saveFile["settings"]["musicVolume"] == null) return 1f;
        return float.Parse(saveFile["settings"]["musicVolume"].ToString());
    }

    public void SetSettingSFXVolume(float value)
    {
        if (saveFile["settings"] == null) saveFile["settings"] = new JObject();
        saveFile["settings"]["sfxVolume"] = value;
    }

    public float GetSettingSFXVolume()
    {
        if (saveFile["settings"] == null) return 1f;
        if (saveFile["settings"]["sfxVolume"] == null) return 1f;
        return float.Parse(saveFile["settings"]["sfxVolume"].ToString());
    }

    //Game store stuff

    public void SetGameStoreHighScore(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["highScore"] = value.ToString();
    }

    public BigInteger GetGameStoreHighScore()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["highScore"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["highScore"].ToString());
    }

    public void SetGameStoreTotalAttepts(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalAttempts"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalAttepts()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalAttempts"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalAttempts"].ToString());
    }

    public void SetGameStoreTotalNormalBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalNormalBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalNormalBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalNormalBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalNormalBerries"].ToString());
    }

    public void SetGameStoreTotalPoisonBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalPoisonBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalPoisonBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalPoisonBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalPoisonBerries"].ToString());
    }

    public void SetGameStoreTotalSlowBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalSlowBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalSlowBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalSlowBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalSlowBerries"].ToString());
    }

    public void SetGameStoreTotalUltraBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalUltraBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalUltraBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalUltraBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalUltraBerries"].ToString());
    }

    public void SetGameStoreTotalSpeedyBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalSpeedyBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalSpeedyBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalSpeedyBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalSpeedyBerries"].ToString());
    }

    public void SetGameStoreTotalRandomBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalRandomBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalRandomBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalRandomBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalRandomBerries"].ToString());
    }

    public void SetGameStoreTotalAntiBerries(BigInteger value)
    {
        if (saveFile["gameStore"] == null) saveFile["gameStore"] = new JObject();
        saveFile["gameStore"]["totalAntiBerries"] = value.ToString();
    }

    public BigInteger GetGameStoreTotalAntiBerries()
    {
        if (saveFile["gameStore"] == null) return 0;
        if (saveFile["gameStore"]["totalAntiBerries"] == null) return 0;
        return BigInteger.Parse(saveFile["gameStore"]["totalAntiBerries"].ToString());
    }
}
