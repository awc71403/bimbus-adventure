using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    #region Editor Variables
    [SerializeField]
    private InputField OAuth;
    [SerializeField]
    private InputField Channel;
    [SerializeField]
    private InputField ChannelTwo;
    [SerializeField]
    private InputField Rounds;
    [SerializeField]
    private InputField RoundDuration;
    [SerializeField]
    private InputField Bots;
    [SerializeField]
    private Button GamemodeButton;
    [SerializeField]
    private Button TwitchButton;
    [SerializeField]
    private Button SettingsButton;
    [SerializeField]
    private Button EndlessButton;
    [SerializeField]
    private Button CreditsButton;
    [SerializeField]
    private Image InfoBox;
    #endregion

    #region Private Variables
    private bool SettingsOpen = false;
    private bool TwitchSettingsOpen = false;
    private bool CreditsOpen = false;
    private int EndlessRounds = 0;
    private int Team = 0;
    private float timeRemainingInCurrentState;

    private int roundsInt = Consts.DEFAULT_ROUNDS;
    private int durationInt = Consts.DEFAULT_DECIDING_DURATION;
    private int botsInt;

    private int[] leftpath = { -20, -4, -4 };
    private int[] rightpath = { -30, -4, -4 };
    private int right;
    #endregion

    #region Initialization
    private void Start()
    {
        try
        {
            OAuth.text = $"{PlayerPrefs.GetString("OAuth")}";
            Channel.text = $"{PlayerPrefs.GetString("Channel")}";
        }
        catch (NullReferenceException)
        {
        }

        Vector2 newPos = this.GetComponentInChildren<Player>().gameObject.transform.position;
        right = UnityEngine.Random.Range(0, 2);
        switch (right) {
            case 0:
                newPos.x = UnityEngine.Random.Range((float)Consts.MENU_LEFTBRIDGE_X_LOWER, (float)Consts.MENU_LEFTBRIDGE_X_UPPER);
                newPos.y = UnityEngine.Random.Range((float)Consts.MENU_LEFTBRIDGE_Y_LOWER, (float)Consts.MENU_LEFTBRIDGE_Y_UPPER);
                this.GetComponentInChildren<Player>().animator.SetBool("Left", false);
                break;
            case 1:
                newPos.x = UnityEngine.Random.Range((float)Consts.MENU_RIGHTBRIDGE_X_LOWER, (float)Consts.MENU_RIGHTBRIDGE_X_UPPER);
                newPos.y = UnityEngine.Random.Range((float)Consts.MENU_RIGHTBRIDGE_Y_LOWER, (float)Consts.MENU_RIGHTBRIDGE_Y_UPPER);
                break;
        }
        this.GetComponentInChildren<Player>().animator.SetBool("Scarf", true);
        this.GetComponentInChildren<Player>().gameObject.transform.position = newPos;

        timeRemainingInCurrentState = 0;
    }
    #endregion

    #region Update
    private void Update() {
        if (timeRemainingInCurrentState > 0) {
            timeRemainingInCurrentState -= Time.deltaTime;
        }
        else {
            timeRemainingInCurrentState = UnityEngine.Random.Range(6, 11);
            if (right == 0) {
                this.GetComponentInChildren<Player>().MoveToLocation(rightpath);
                this.GetComponentInChildren<Player>().animator.SetBool("Left", false);
                right = 1;
            }
            else {
                this.GetComponentInChildren<Player>().MoveToLocation(leftpath);
                this.GetComponentInChildren<Player>().animator.SetBool("Left", true);
                right = 0;
            }
        }
    }
    #endregion

    #region Buttons
    public void StartButton()
    {
        if (OAuth.text == "" || Channel.text == "") {
            if (TwitchSettingsOpen == true) {
                TwitchSettings();
            }
            else if (CreditsOpen == true) {
                Credits();
            }
            else if (SettingsOpen == true) {
                Settings();
            }
            InfoBox.gameObject.SetActive(true);
            InfoBox.GetComponentInChildren<TextMeshProUGUI>().text = "Please insert your Twitch Settings. Find your aouth using twitch chat oauth\n " + " password generator.";
        }
        else {
            PlayerPrefs.SetString("OAuth", OAuth.text);
            PlayerPrefs.SetString("Channel", Channel.text);
            PlayerPrefs.SetString("ChannelTwo", ChannelTwo.text);

            int.TryParse(Bots.GetComponentsInChildren<Text>()[1].text, out botsInt);
            PlayerPrefs.SetInt("Bots", botsInt);
            int.TryParse(Rounds.GetComponentsInChildren<Text>()[1].text, out roundsInt);
            if (roundsInt == 0) {
                roundsInt = Consts.DEFAULT_ROUNDS;
            }
            PlayerPrefs.SetInt("Rounds", roundsInt);
            int.TryParse(RoundDuration.GetComponentsInChildren<Text>()[1].text, out durationInt);
            if (durationInt == 0) {
                durationInt = Consts.DEFAULT_DECIDING_DURATION;
            }
            PlayerPrefs.SetInt("Duration", durationInt);

            PlayerPrefs.SetInt("Endless", EndlessRounds);
            PlayerPrefs.SetInt("Gamemode", Team);

            SceneManager.LoadScene(1);
        }
    }

    public void Gamemode() {
        if (Team == 0) {
            GamemodeButton.GetComponentInChildren<Text>().text = "Teams: On";
            Team = 1;
        }
        else {
            GamemodeButton.GetComponentInChildren<Text>().text = "Teams: Off";
            Team = 0;
        }
    }

    public void Settings() {
        if (!SettingsOpen) {
            if (CreditsOpen == true) {
                Credits();
            }
            if (TwitchSettingsOpen == true) {
                TwitchSettings();
            }
            Rounds.gameObject.SetActive(true);
            RoundDuration.gameObject.SetActive(true);
            Bots.gameObject.SetActive(true);
            GamemodeButton.gameObject.SetActive(false);
            EndlessButton.gameObject.SetActive(false);
            CreditsButton.gameObject.SetActive(false);
            SettingsButton.GetComponentInChildren<Text>().text = "Hide Settings";
            SettingsOpen = true;
            InfoBox.gameObject.SetActive(true);
            InfoBox.GetComponentInChildren<TextMeshProUGUI>().text = "- Default Rounds is 10\n- Default Round timer is 20\n- Default bots is 0";
        }
        else {
            Rounds.gameObject.SetActive(false);
            RoundDuration.gameObject.SetActive(false);
            Bots.gameObject.SetActive(false);
            GamemodeButton.gameObject.SetActive(true);
            EndlessButton.gameObject.SetActive(true);
            CreditsButton.gameObject.SetActive(true);
            SettingsButton.GetComponentInChildren<Text>().text = "Settings";
            SettingsOpen = false;
            InfoBox.gameObject.SetActive(false);
        }
    }

    public void Endless() {
        if (EndlessRounds == 0) {
            EndlessButton.GetComponentInChildren<Text>().text = "Endless: On";
            EndlessRounds = 1;
        }
        else {
            EndlessButton.GetComponentInChildren<Text>().text = "Endless: Off";
            EndlessRounds = 0;
        }
    }

    public void TwitchSettings() {
        if (!TwitchSettingsOpen) {
            if (CreditsOpen == true) {
                Credits();
            }
            if (SettingsOpen == true) {
                Settings();
            }
            OAuth.gameObject.SetActive(true);
            Channel.gameObject.SetActive(true);
            ChannelTwo.gameObject.SetActive(true);
            TwitchButton.GetComponentInChildren<Text>().text = "Hide Settings";
            TwitchSettingsOpen = true;
            InfoBox.gameObject.SetActive(true);
            InfoBox.GetComponentInChildren<TextMeshProUGUI>().text = "- Oauth password generator\n- Channel to connect to\n- Channel #2 is optional\n * Do not show oauth password";
        }
        else {
            OAuth.gameObject.SetActive(false);
            Channel.gameObject.SetActive(false);
            ChannelTwo.gameObject.SetActive(false);
            TwitchButton.GetComponentInChildren<Text>().text = "Twitch Settings";
            TwitchSettingsOpen = false;
            InfoBox.gameObject.SetActive(false);
        }
    }

    public void Credits() {
        if (!CreditsOpen) {
            if (TwitchSettingsOpen == true) {
                TwitchSettings();
            }
            InfoBox.gameObject.SetActive(true);
            CreditsButton.GetComponentInChildren<Text>().text = "Hide Credits";
            InfoBox.GetComponentInChildren<TextMeshProUGUI>().text = "Made By: Wojteknician\nBimbus Sprite By: SirFudgey\nMusic By: dufeboi\n  Inspired BY: Sykkuno";
            CreditsOpen = true;
        }
        else {
            InfoBox.gameObject.SetActive(false);
            CreditsButton.GetComponentInChildren<Text>().text = "Credits";
            CreditsOpen = false;
        }
    }
    #endregion
}
