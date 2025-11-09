using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMenu : MonoBehaviour
{
    public TMP_Text text;
    public Button updateButton;
    public Button continueButton;

    async void Start()
    {
        if (!Application.isMobilePlatform)
        {
            var width = Display.main.systemWidth;
            var height = Display.main.systemHeight;
            Screen.SetResolution(width, height, BazookaManager.Instance.GetSettingFullScreen());
            QualitySettings.vSyncCount = BazookaManager.Instance.GetSettingVsync() ? 1 : -1;
        }
        else
        {
            Application.targetFrameRate = 360;
            QualitySettings.vSyncCount = 0;
        }
        PlayerPrefs.SetString("latestVersion", Application.version);
        updateButton.onClick.AddListener(() =>
        {
            Application.OpenURL("https://games.lncvrt.xyz/download");
        });
        await SceneManager.LoadSceneAsync("GamePlayer");
    }
}