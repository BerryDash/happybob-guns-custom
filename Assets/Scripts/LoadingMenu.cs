using System;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMenu : MonoBehaviour
{
    public TMP_Text text;
    public Button updateButton;
    public Button continueButton;

    void Start()
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
            Application.OpenURL("https://berrydashwithguns.lncvrt.xyz/download");
        });
        CheckUpdate();
    }

    async void CheckUpdate()
    {
        using UnityWebRequest request = UnityWebRequest.Get("https://berrydashwithguns.lncvrt.xyz/database/canLoadClient.php");
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            text.text = "Failed to check version";
            return;
        }
        string response = request.downloadHandler.text;
        if (response == "1")
        {
            await SceneManager.LoadSceneAsync("GamePlayer");
        }
        else if (response == "2")
        {
            text.text = "Outdated client! You can still play the game and access the servers, but it isn't recommended.";

            var updateButtonPos = updateButton.transform.localPosition;
            updateButtonPos.x = -135;
            updateButton.transform.localPosition = updateButtonPos;

            updateButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(true);
        }
        else if (response == "3")
        {
            text.text = "Outdated client! You can still load into the game, but online features may not be available.";

            var updateButtonPos = updateButton.transform.localPosition;
            updateButtonPos.x = -135;
            updateButton.transform.localPosition = updateButtonPos;

            updateButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(true);
        }
        else if (response == "4")
        {
            text.text = "You are on a beta version of the game. You can still play the game and access the servers, but it is recommended to use the latest stable version.";
            continueButton.transform.position = updateButton.transform.position;
            continueButton.gameObject.SetActive(true);
        }
        else
        {
            text.text = "Outdated client! Please update your client to play";
            updateButton.gameObject.SetActive(true);
        }
    }
}