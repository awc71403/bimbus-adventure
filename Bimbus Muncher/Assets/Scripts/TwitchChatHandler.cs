using TwitchLib.Client.Models;
using TwitchLib.Unity;
using UnityEngine;

public class TwitchChatHandler : MonoBehaviour {
    [SerializeField] //[SerializeField] Allows the private field to show up in Unity's inspector. Way better than just making it public
    public string _channel;
    [SerializeField]
    public string _oauth;

    private string _channeltwo;
    private GameManager gameManager;
    private PlayerManager playerManager;

    private int gamemode;

    private Client _client;

    private void Start() {

        _oauth = PlayerPrefs.GetString("OAuth");
        _channel = PlayerPrefs.GetString("Channel");
        _channeltwo = PlayerPrefs.GetString("ChannelTwo", "");

        gamemode = PlayerPrefs.GetInt("Gamemode", 0);

        gameManager = FindObjectOfType<GameManager>();
        playerManager = FindObjectOfType<PlayerManager>();

        // To keep the Unity application active in the background, you can enable "Run In Background" in the player settings:
        // Unity Editor --> Edit --> Project Settings --> Player --> Resolution and Presentation --> Resolution --> Run In Background
        // This option seems to be enabled by default in more recent versions of Unity. An aditional, less recommended option is to set it in code:
        // Application.runInBackground = true;

        //Create Credentials instance
        ConnectionCredentials credentials = new ConnectionCredentials(_channel, _oauth);

        // Create new instance of Chat Client
        _client = new Client();

        // Initialize the client with the credentials instance, and setting a default channel to connect to.
        _client.Initialize(credentials, _channel);

        // Bind callbacks to events
        _client.OnConnected += OnConnected;
        _client.OnMessageReceived += OnMessageReceived;

        // Connect
        _client.Connect();
    }

    private void OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e) {
        if (_channeltwo != "") {
            _client.JoinChannel(_channeltwo);
        }
    }

    private void OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e) {

        string[] words = e.ChatMessage.Message.Split(' ');
        string color = "black";

        if (words[0].Equals("!join")) {
            if (gamemode == 1 && words.Length == 2) {
                color = words[1].ToLower();
            }
            else if (gamemode == 1) {
                return;
            }

            if ((gameManager.CurrentGameState == GameManager.GameState.Join || PlayerPrefs.GetInt("Endless") == 1) && gameManager.CurrentGameState != GameManager.GameState.Winner && words.Length <= 2) {
                if (e.ChatMessage.IsModerator || e.ChatMessage.IsSubscriber || e.ChatMessage.IsMe || e.ChatMessage.IsBroadcaster || e.ChatMessage.Username == "wojteknician" || e.ChatMessage.Username == "Sykkuno") {
                    playerManager.CreatePlayer(e.ChatMessage.Username, color, true);
                }
                else {
                    playerManager.CreatePlayer(e.ChatMessage.Username, color);
                }
            }
            
        }
        else {
            gameManager.SetLocation(e.ChatMessage.Username, e.ChatMessage.Message);
        }
    }
}
