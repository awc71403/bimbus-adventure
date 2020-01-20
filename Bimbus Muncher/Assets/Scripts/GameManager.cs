using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    #region Events and Delegates
    public delegate void PlayerMove(string username, string gotoLocation);
    public static event PlayerMove PlayerMoveEvent;

    public delegate void Points(int[] locationPoints);
    public static event Points PointsEvent;
    #endregion

    #region GameStates
    public enum GameState
    {
        Join,
        Deciding,
        Randomizer,
        Winner
    }
    #endregion

    #region Editor Variables
    [SerializeField]
    private Button pauseButton;
    [SerializeField]
    private Button playButton;
    [SerializeField]
    private float joinDuration = Consts.DEFAULT_JOIN_DURATION;
    [SerializeField]
    private float decidingDuration = Consts.DEFAULT_DECIDING_DURATION;
    [SerializeField]
    private float randomizerDuration = Consts.DEFAULT_RANDOMIZER_DURATION;
    [SerializeField]
    private int totalrounds = Consts.DEFAULT_ROUNDS;
    [SerializeField]
    private TextMeshProUGUI countdownText;
    [SerializeField]
    private TextMeshProUGUI roundText;
    [SerializeField]
    private Image InfoBox;
    [SerializeField]
    private Image Leaderboard;

    [SerializeField]
    private Image Eat;
    [SerializeField]
    private Image Rest;
    [SerializeField]
    private Image Play;
    [SerializeField]
    private Image Fish;
    [SerializeField]
    private Image Red;
    [SerializeField]
    private Image Blue;

    [SerializeField]
    private TMP_InputField roundInput;
    #endregion

    #region Private Variables
    private float timeRemainingInCurrentState;
    private bool paused = false;
    private int[] locationPoints = new int[4];
    private int[] setlocationPoints = new int[4];
    private int currround = 0;
    private int bots = 0;
    private int gamemode = 0;
    private Sprite[] buttonsprites;

    private List<string> placementPrefix = new List<string>();
    private List<string> runMessages = new List<string>();

    private PlayerManager playerManager;
    #endregion

    #region Accessor
    public GameState CurrentGameState { get; private set; }
    #endregion

    #region Initialization
    private void Awake() {
        gamemode = PlayerPrefs.GetInt("Gamemode", 0);
        totalrounds = PlayerPrefs.GetInt("Rounds", Consts.DEFAULT_ROUNDS);
        decidingDuration = PlayerPrefs.GetInt("Duration", Consts.DEFAULT_DECIDING_DURATION);


        if (PlayerPrefs.GetInt("Endless") == 1) {
            totalrounds = 99;
            roundText.text = "Endless";
        }

        SetState(GameState.Join);
        playerManager = FindObjectOfType<PlayerManager>();
        pauseButton.gameObject.SetActive(true);

        // Eat
        locationPoints[0] = 0;
        // Rest
        locationPoints[1] = 15;
        // Play
        locationPoints[2] = 30;
        // Fish
        locationPoints[3] = 45;

        for (int i = 0; i < Consts.DEFAULT_LEADERBOARD_AMOUNT; i++) {
            int number = i + 1;
            if (number % 10 == 1) {
                placementPrefix.Add(string.Format("{0}st:", number));
            }
            else if (number % 10 == 2) {
                placementPrefix.Add(string.Format("{0}nd:", number));
            }
            else if (number % 10 == 3) {
                placementPrefix.Add(string.Format("{0}rd:", number));
            }
            else {
                placementPrefix.Add(string.Format("{0}th:", number));
            }
        }
    }
    #endregion

    #region Update
    private void Update()
    {
        if (paused == false) {
            if (CurrentGameState != GameState.Winner) {
                if (timeRemainingInCurrentState > 0) {
                    timeRemainingInCurrentState -= Time.deltaTime;
                }
                if (CurrentGameState != GameState.Randomizer) {
                    countdownText.SetText(timeRemainingInCurrentState.ToString("N0"));
                }
                if (timeRemainingInCurrentState <= 0) {
                    MoveToNextState();
                }
            }
        }
    }
    #endregion

    #region Randomizer
    public void Shuffle(int[] list) {
        System.Random rnd = new System.Random();
        for (int i = 0; i < list.Length; i++) {
            int k = rnd.Next(0, i);
            int value = list[k];
            list[k] = list[i];
            list[i] = value;
        }
        Eat.GetComponentInChildren<TextMeshProUGUI>().text = "Eat " + locationPoints[0] + "-" + (100 - locationPoints[0]);
        Rest.GetComponentInChildren<TextMeshProUGUI>().text = "Rest " + locationPoints[1] + "-" + (100 - locationPoints[1]);
        Play.GetComponentInChildren<TextMeshProUGUI>().text = "Play " + locationPoints[2] + "-" + (100 - locationPoints[2]);
        Fish.GetComponentInChildren<TextMeshProUGUI>().text = "Fish " + locationPoints[3] + "-" + (100 - locationPoints[3]);
    }

    public void RandomizeLocationPoints() {
        for (int i = 0; i < locationPoints.Length; i++) {
            setlocationPoints[i] = UnityEngine.Random.Range(locationPoints[i], 100 - locationPoints[i] + 1);
        }
        Eat.GetComponentInChildren<TextMeshProUGUI>().text = "Eat " + setlocationPoints[0];
        Rest.GetComponentInChildren<TextMeshProUGUI>().text = "Rest " + setlocationPoints[1];
        Play.GetComponentInChildren<TextMeshProUGUI>().text = "Play " + setlocationPoints[2];
        Fish.GetComponentInChildren<TextMeshProUGUI>().text = "Fish " + setlocationPoints[3];
        PointsEvent(setlocationPoints);
    }
    #endregion

    #region States
    private void SetState(GameState state) {
        CurrentGameState = state;
        switch (CurrentGameState) {
            case GameState.Join:
                timeRemainingInCurrentState = joinDuration;
                InfoBox.gameObject.SetActive(true);
                if (gamemode == 1) {
                    InfoBox.GetComponentInChildren<TextMeshProUGUI>().text = "Type \"!join <color>\"\nColors:\nRed\nBlue";
                }
                else {
                    InfoBox.GetComponentInChildren<TextMeshProUGUI>().text = "Type \"!join\"";
                }
                break;
            case GameState.Deciding:
                timeRemainingInCurrentState = decidingDuration;
                currround++;
                Shuffle(locationPoints);
                foreach (var bot in playerManager.GetBots()) {
                    SetLocation(bot.Username, playerManager.GetLocations()[Random.Range(0, 4)]);
                }
                if (PlayerPrefs.GetInt("Endless") != 1) {
                    roundText.text = "Round:" + currround + "/";
                    if (currround == 1) {
                        roundInput.gameObject.SetActive(true);
                        roundInput.GetComponentsInChildren<TextMeshProUGUI>()[1].text = "" + totalrounds;
                    }
                }
                break;
            case GameState.Randomizer:
                timeRemainingInCurrentState = randomizerDuration;
                RandomizeLocationPoints();
                UpdateLeaderboard();
                break;
            case GameState.Winner:
                EndGame();
                break;
        }
    }

    private void MoveToNextState() {
        switch (CurrentGameState) {
            case GameState.Join:
                if (playerManager.getPlayerSize() == 0) {
                    SetState(GameState.Winner);
                }
                else {
                    InfoBox.gameObject.SetActive(false);
                    Leaderboard.gameObject.SetActive(true);
                    if (gamemode == 1) {
                        Red.gameObject.SetActive(true);
                        Blue.gameObject.SetActive(true);
                    }
                    SetState(GameState.Deciding);
                }
                break;
            case GameState.Deciding:
                if (playerManager.getPlayersMoving() == 0) {
                    if (currround == 1 || PlayerPrefs.GetInt("Endless") == 1) {
                        bool playeratbridge = false;
                        foreach (var player in playerManager.GetPlayers()) {
                            if (player.currLocation == -1) {
                                SetLocation(player.Username, playerManager.GetLocations()[Random.Range(0, 4)]);
                                playeratbridge = true;
                            }
                        }
                        if (playeratbridge) {
                            return;
                        }
                    }
                    SetState(GameState.Randomizer);
                }
                break;
            case GameState.Randomizer:
                if (currround == totalrounds) {
                    SetState(GameState.Winner);
                }
                else {
                    SetState(GameState.Deciding);
                }
                break;
        }
    }
    #endregion


    #region UI Methods
    public void Pause() {
        if (paused == false) {
            paused = true;
            pauseButton.image.sprite = Resources.Load("Play", typeof(Sprite)) as Sprite;
        }
        else {
            paused = false;
            pauseButton.image.sprite = Resources.Load("Pause", typeof(Sprite)) as Sprite;
        }
    }

    public void PlayAgain() {
        SceneManager.LoadScene(0);
    }

    public void ChangeRounds(string newrounds) {
        int newroundsInt = int.Parse(newrounds);
        if (currround <= newroundsInt) {
            totalrounds = newroundsInt;
            roundInput.text = "" + totalrounds;
        }
        else {
            roundInput.text = "" + totalrounds;
        }
    }
    #endregion

    #region Player
    public void SetLocation(string username, string gotoLocation) {
        if (CurrentGameState == GameState.Deciding || CurrentGameState == GameState.Join) {
            if (CurrentGameState != GameState.Randomizer) {
                PlayerMoveEvent(username, gotoLocation);
            }
        }
    }
    #endregion

    #region Win
    private void EndGame() {
        if (playerManager.getPlayerSize() == 0) {
            InfoBox.GetComponentInChildren<TextMeshProUGUI>().text = "There are no players.\nIf you couldn't join, check your OAuth password.";
        }
        else {
            if (gamemode == 1) {
                int red = playerManager.GetRedPoints();
                int blue = playerManager.GetBluePoints();
                int redplayers = playerManager.GetRedPlayers();
                int blueplayers = playerManager.GetBluePlayers();
                int redfinal = red / redplayers;
                int bluefinal = blue / blueplayers;
                if (redfinal > bluefinal) {
                    InfoBox.GetComponentInChildren<TextMeshProUGUI>().text = "Red team wins!\n\nRed / players = " + redfinal + "\nBlue / Players = " + bluefinal;
                }
                else if (redfinal == bluefinal) {
                    InfoBox.GetComponentInChildren<TextMeshProUGUI>().text = "It's a tie!\n\nRed / players = " + redfinal + "\nBlue / Players = " + bluefinal;
                }
                else {
                    InfoBox.GetComponentInChildren<TextMeshProUGUI>().text = "Blue team wins!\n\nRed / players = " + redfinal + "\nBlue / Players = " + bluefinal;
                }
            }
            else {
                List<Player> winners = new List<Player>();
                int highestpoints = 0;
                foreach (var player in playerManager.GetPlayers()) {
                    if (player.Points > highestpoints) {
                        winners.Add(player);
                        highestpoints = player.Points;
                    }
                    else if (player.Points == highestpoints) {
                        winners.Add(player);
                    }
                    else {
                        break;
                    }
                }
                if (winners.Count == 1) {
                    InfoBox.GetComponentInChildren<TextMeshProUGUI>().text = $"{winners[0].Username} win!";
                }
                else if (winners.Count == 2) {
                    InfoBox.GetComponentInChildren<TextMeshProUGUI>().text = $"{winners[0].Username} and {winners[1].Username} win!";
                }
                else {
                    string winUsers = "";
                    for (int i = 0; i < winners.Count - 1; i++) {
                        winUsers += $"{winners[i].Username}, ";
                    }
                    InfoBox.GetComponentInChildren<TextMeshProUGUI>().text = winUsers + $"and {winners[winners.Count - 1].Username} win!";
                }
            }
        }
        playButton.gameObject.SetActive(true);
        InfoBox.gameObject.SetActive(true);
    }
    #endregion

    #region Leaderboard
    public void UpdateLeaderboard() {
        List<Player> players = playerManager.GetPlayers();

        for (int i = 0; i < Mathf.Min(playerManager.getPlayerSize(), Consts.DEFAULT_LEADERBOARD_AMOUNT); i++) {
            if (players[i] == null) {
                break;
            }
            runMessages.Add(string.Format("{0}\n{1} {2}", placementPrefix[i], players[i].Username, players[i].Points));
        }

        StringBuilder runString = new StringBuilder();
        foreach (var runMessage in runMessages) {
            runString.AppendLine(runMessage);
        }
        Leaderboard.GetComponentsInChildren<TextMeshProUGUI>()[1].SetText(runString);
        runMessages.Clear();

        if (gamemode == 1) {
            Red.GetComponentInChildren<TextMeshProUGUI>().text = "Red:" + playerManager.GetRedPoints();
            Blue.GetComponentInChildren<TextMeshProUGUI>().text = "Blue:" + playerManager.GetBluePoints();
        }
    }
    #endregion
}

