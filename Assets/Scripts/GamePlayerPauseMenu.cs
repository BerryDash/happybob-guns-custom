using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayerPauseMenu : MonoBehaviour
{
    public static GamePlayerPauseMenu Instance;
    public Button continueButton;
    public Button statsButton;
    public Button exitButton;
    public TMP_Text versionText;

    public GameObject statsMenu;
    public Button statsMenuExitButton;
    public TMP_Text statsText;

    public Toggle settingFullscreenToggle;
    public Toggle settingVSyncToggle;
    public Toggle settingRandomMusicToggle;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Awake()
    {
        if (Application.isMobilePlatform || Application.isEditor)
        {
            exitButton.gameObject.SetActive(false);
            continueButton.gameObject.transform.localPosition = new(-175, 0, 0);
            statsButton.gameObject.transform.localPosition = new(175, 0, 0);
        }
        else
        {
            exitButton.onClick.AddListener(() => Application.Quit());
        }
        Instance = this;
        continueButton.onClick.AddListener(GamePlayer.instance.DisablePause);
        statsButton.onClick.AddListener(() =>
        {
            statsMenu.SetActive(true);
            var text = new StringBuilder();
            text.AppendLine("High Score: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreHighScore()));
            text.AppendLine("Total Normal Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalNormalBerries()));
            text.AppendLine("Total Poison Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalPoisonBerries()));
            text.AppendLine("Total Slow Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalSlowBerries()));
            text.AppendLine("Total Ultra Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalUltraBerries()));
            text.AppendLine("Total Speedy Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalSpeedyBerries()));
            text.AppendLine("Total Random Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalRandomBerries()));
            text.AppendLine("Total Anti Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalAntiBerries()));
            text.AppendLine("Total Attempts: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalAttepts()));
            statsText.text = text.ToString();
        });
        statsMenuExitButton.onClick.AddListener(() =>
        {
            statsMenu.SetActive(false);
            statsText.text = string.Empty;
        });
        versionText.text = Application.version;

        musicSlider.value = BazookaManager.Instance.GetSettingMusicVolume();
        sfxSlider.value = BazookaManager.Instance.GetSettingSFXVolume();
        if (!Application.isMobilePlatform)
        {
            settingFullscreenToggle.isOn = BazookaManager.Instance.GetSettingFullScreen() == true;
            settingVSyncToggle.isOn = BazookaManager.Instance.GetSettingVsync() == true;
            settingRandomMusicToggle.isOn = BazookaManager.Instance.GetSettingRandomMusic() == true;
            settingFullscreenToggle.onValueChanged.AddListener(value =>
            {
                BazookaManager.Instance.SetSettingFullScreen(value);
                var width = Display.main.systemWidth;
                var height = Display.main.systemHeight;
                Screen.SetResolution(width, height, value);
            });
            settingVSyncToggle.onValueChanged.AddListener(value =>
            {
                BazookaManager.Instance.SetSettingVsync(value);
                QualitySettings.vSyncCount = value ? 1 : -1;
            });
        }
        else
        {
            settingFullscreenToggle.interactable = false;
            settingVSyncToggle.interactable = false;
            settingRandomMusicToggle.isOn = BazookaManager.Instance.GetSettingRandomMusic() == true;
        }
        settingRandomMusicToggle.onValueChanged.AddListener(value => BazookaManager.Instance.SetSettingRandomMusic(value));
        musicSlider.onValueChanged.AddListener(value =>
        {
            BazookaManager.Instance.SetSettingMusicVolume(value);
            GamePlayer.instance.backgroundMusic.volume = value;
        });
        sfxSlider.onValueChanged.AddListener(value => BazookaManager.Instance.SetSettingSFXVolume(value));
    }
}