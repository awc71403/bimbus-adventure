using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using cakeslice;

public class PlayerManager : MonoBehaviour {

    #region Private Variables
    private List<Player> playerslist = new List<Player>();
    private List<Player> botslist = new List<Player>();

    private List<Player> previousleaders = new List<Player>();

    private int playeramount = 0;
    private int playersmoving = 0;

    public GameObject playerPrefab;

    private int bots = 0;
    private int gamemode;

    private string[] colors = { "red", "blue" , "black" };
    private int[] teampoints = { 0, 0 };
    private int redplayers = 0;
    private int blueplayers = 0;
    private List<string> listofcolors;

    private string[] locations = { "eat", "rest", "play", "fish" };
    private List<string> listoflocations;

    private Dictionary<string, int[]> pathway;

    private bool[] boolpicker = { true, false };
    #endregion

    #region Initialization
    private void Start() {
        pathway = new Dictionary<string, int[]>();
        bots = PlayerPrefs.GetInt("Bots");
        gamemode = PlayerPrefs.GetInt("Gamemode", 0);
        listofcolors = colors.ToList<string>();
        listoflocations = locations.ToList<string>();
        for (int i = 0; i < bots; i++)
        {
            CreatePlayer($"Bot{i}", colors[UnityEngine.Random.Range(0, 2)], false, true);
        }

        int[] bridgeeat = { -3, 0, -4 };
        pathway.Add("bridgeeat", bridgeeat);

        int[] bridgerest = { -2, 1, -4 };
        pathway.Add("bridgerest", bridgerest);

        int[] bridgeplay = { -2, 2, -4 };
        pathway.Add("bridgeplay", bridgeplay);

        int[] bridgefish = { -3, 3, -4 };
        pathway.Add("bridgefish", bridgefish);

        int[] eatrest = { -3, -2, 1 };
        pathway.Add("eatrest", eatrest);

        int[] eatplay = { -3, -2, 2 };
        pathway.Add("eatplay", eatplay);

        int[] eatfish = { 3, -4, -4 };
        pathway.Add("eatfish", eatfish);

        int[] resteat = { -2, -3, 0 };
        pathway.Add("resteat", resteat);

        int[] restplay = { 2, -4, -4 };
        pathway.Add("restplay", restplay);

        int[] restfish = { -2, -3, 3 };
        pathway.Add("restfish", restfish);

        int[] playeat = { -2, -3, 0 };
        pathway.Add("playeat", playeat);

        int[] playrest = { 1, -4, -4 };
        pathway.Add("playrest", playrest);

        int[] playfish = { -2, -3, 3 };
        pathway.Add("playfish", playfish);

        int[] fisheat = { 0, -4, -4 };
        pathway.Add("fisheat", fisheat);

        int[] fishrest = { -3, -2, 1 };
        pathway.Add("fishrest", fishrest);

        int[] fishplay = { -3, -2, 2 };
        pathway.Add("fishplay", fishplay);

        int[] lbridgeeat = { -3, 0, -4 };
        pathway.Add("lbridgeeat", lbridgeeat);

        int[] lbridgerest = { 1, -4, -4 };
        pathway.Add("lbridgerest", lbridgerest);

        int[] lbridgeplay = { 2, -4, -4 };
        pathway.Add("lbridgeplay", lbridgeplay);

        int[] lbridgefish = { -3, 3, -4 };
        pathway.Add("lbridgefish", lbridgefish);

        int[] rbridgeeat = { 0, -4, -4 };
        pathway.Add("rbridgeeat", rbridgeeat);

        int[] rbridgerest = { -2, 1, -4 };
        pathway.Add("rbridgerest", rbridgerest);

        int[] rbridgeplay = { -2, 2, -4 };
        pathway.Add("rbridgeplay", rbridgeplay);

        int[] rbridgefish = { 3, -4, -4 };
        pathway.Add("rbridgefish", rbridgefish);
    }

    //-1 is Bridge
    //-2 is Left Bridge
    //-3 is Right Bridge
    //0 is Eat
    //1 is Rest
    //2 is Play
    //3 is Fish
    //-4 is Nothing

    private void OnEnable() {
        GameManager.PlayerMoveEvent += gotoLocation;
        GameManager.PointsEvent += CalculatePoints;
        Player.MovementEndEvent += DecrementPlayersMoving;
    }
    #endregion

    #region Accessors
    internal Player GetPlayer(string username)
    {
        var player = playerslist.FirstOrDefault(t => t.Username == username);
        return player;
    }

    internal List<Player> GetPlayers() {
        return playerslist;
    }

    internal List<Player> GetBots() {
        return botslist;
    }

    internal string[] GetLocations() {
        return locations;
    }

    internal int GetRedPoints() {
        return teampoints[0];
    }

    internal int GetRedPlayers() {
        return redplayers;
    }

    internal int GetBluePoints() {
        return teampoints[1];
    }

    internal int GetBluePlayers() {
        return blueplayers;
    }

    internal int getPlayerSize() {
        return playeramount;
    }

    internal int getPlayersMoving() {
        return playersmoving;
    }
    #endregion

    #region Player Creation
    internal void CreatePlayer(string username, string color, bool scarf = false, bool bot = false) {
        if (listofcolors.Contains(color)) {
            var player = playerslist.FirstOrDefault(t => t.Username == username);
            if (player == null) {
                GameObject playerObject = Instantiate(playerPrefab);
                player = playerObject.GetComponent<Player>();
                player.Username = username;
                player.Points = 0;
                player.currLocation = -1;
                player.endLocation = -1;
                player.stringcurrLocation = "bridge";
                player.Color = color;

                //playerObject.GetComponentInChildren<TextMeshProUGUI>().text = username;

                //playerObject.GetComponentInChildren<Canvas>().gameObject.GetComponentInChildren<TextMeshProUGUI>().text = username;
                playerslist.Add(player);
                if (bot) {
                    botslist.Add(player);
                }
                if (scarf) {
                    player.animator.SetBool("Scarf", true);
                }
                player.animator.SetBool("Rest", true);
                player.animator.SetBool("Sit", boolpicker[UnityEngine.Random.Range(0, 2)]);
                player.animator.SetBool("PlaceLeft", boolpicker[UnityEngine.Random.Range(0, 2)]);
                playeramount++;

                if (gamemode == 1) {
                    switch (color) {
                        case "red":
                            redplayers++;
                            player.GetComponent<cakeslice.Outline>().color = 0;
                            break;
                        case "blue":
                            blueplayers++;
                            player.GetComponent<cakeslice.Outline>().color = 1;
                            break;
                    }
                }

                if (playeramount == 9) {
                    foreach (var p in playerslist) {
                        p.SetLeader(false);
                        p.GetComponentInChildren<TextMeshPro>().text = "";
                    }
                }
                else if (playeramount > 9) {
                    player.SetLeader(false);
                }
                else {
                    player.GetComponentInChildren<TextMeshPro>().text = username;
                }
            }
        }
    }
    #endregion

    #region Sort Leaders
    internal void SortLeaders() {
        int leadingscore = playerslist[0].Points;
        int sykPogCounter = 0;
        for (int i = 0; i < Mathf.Min(playeramount, Consts.DEFAULT_LEADERBOARD_AMOUNT); i++) {
            if (playerslist[i].Points == leadingscore) {
                Emote(playerslist[i], Resources.Load("sykPog", typeof(Sprite)) as Sprite);
                sykPogCounter++;
                if (playeramount > 8) {
                    if (!playerslist[i].GetLeader()) {
                        playerslist[i].SetLeader(true);
                    }
                }
            }
            else {
                break;
            }
        }
        if (playeramount > 1) {
            List<Player> leaders = playerslist.GetRange(sykPogCounter, Mathf.Min(playeramount, Consts.DEFAULT_LEADERBOARD_AMOUNT) - sykPogCounter);
            if (previousleaders.Count == 0) {
                foreach (var player in leaders) {
                    Emote(player, Resources.Load("sykShock", typeof(Sprite)) as Sprite);
                    if (playeramount > 8) {
                        player.SetLeader(true);
                    }
                }
            }
            else {
                for (int i = 0; i < Mathf.Min(previousleaders.Count, Consts.DEFAULT_LEADERBOARD_AMOUNT); i++) {
                    if (i < Mathf.Min(playeramount, Consts.DEFAULT_LEADERBOARD_AMOUNT) - sykPogCounter && !Contains(leaders[i], previousleaders)) {
                        //New leaders
                        Emote(leaders[i], Resources.Load("sykShock", typeof(Sprite)) as Sprite);
                        if (playeramount > 8) {
                            leaders[i].SetLeader(true);
                        }
                    }
                    if (!Contains(previousleaders[i], leaders) && i < previousleaders.Count) {
                        //Leaving leaders
                        if (previousleaders[i].Points != leadingscore) {
                            Emote(previousleaders[i], Resources.Load("sykS", typeof(Sprite)) as Sprite);
                            if (playeramount > 8) {
                                previousleaders[i].SetLeader(false);
                            }
                        }
                    }
                    else if (i < previousleaders.Count) {
                        //Staying leaders
                        Emote(previousleaders[i], Resources.Load("sykBimbus", typeof(Sprite)) as Sprite);
                    }
                }
            }
        }
        previousleaders = playerslist.GetRange(0, Mathf.Min(playeramount, Consts.DEFAULT_LEADERBOARD_AMOUNT));
    }

    private bool Contains(Player player, List<Player> list) {
        foreach (var p in list) {
            if (p.Username == player.Username) {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region UI Emote
    internal void Emote(Player player, Sprite emote) {
        player.GetComponentsInChildren<SpriteRenderer>(true)[1].gameObject.SetActive(true);
        player.GetComponentsInChildren<SpriteRenderer>()[1].sprite = emote;
        player.GetComponentsInChildren<SpriteRenderer>()[1].color = new Color(1, 1, 1, 1);
        StartCoroutine(FadeOut(player, Consts.DEFAULT_FADE_TIME));

    }

    IEnumerator FadeOut(Player player, float lerpTime) {
        float timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while (true) {
            timeSinceStarted = Time.time - timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(1, 0, percentageComplete);

            player.GetComponentsInChildren<SpriteRenderer>(true)[1].color = new Color(1, 1, 1, currentValue);

            if (percentageComplete >= 1) {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
        player.GetComponentsInChildren<SpriteRenderer>()[1].gameObject.SetActive(false);
    }
    #endregion

    #region Points
    internal void CalculatePoints(int[] setlocationPoints) {
        foreach (Player player in playerslist) {
            player.Points += setlocationPoints[player.endLocation];
            if (gamemode == 1) {
                switch (player.Color) {
                    case "red":
                        teampoints[0] += setlocationPoints[player.endLocation];
                        break;
                    case "blue":
                        teampoints[1] += setlocationPoints[player.endLocation];
                        break;
                }
            }
        }
        playerslist.Sort(ComparePlayerDistance);
        SortLeaders();
        Sound.PlaySound();
    }

    internal int ComparePlayerDistance(Player x, Player y) {
        if (x == null) {
            if (y == null) {
                return 0;
            }
            else {
                return 1;
            }
        }
        else {
            if (y == null) {
                return -1;
            }
            else {
                if (x.Points > y.Points) {
                    return -1;
                }
                else if (y.Points > x.Points) {
                    return 1;
                }
                else {
                    return 0;
                }
            }
        }
    }
    #endregion

    #region Player Movement
    public void gotoLocation(string username, string gotoLocation) {
        Player player = GetPlayer(username);
        if (listoflocations.Contains(gotoLocation)) {
            if (player != null) {
                if (player.stringcurrLocation != gotoLocation) {
                    if (!PlayerMoving(player)) {
                        playersmoving++;
                    }
                    player.MoveToLocation(pathway[player.stringcurrLocation + gotoLocation]);
                }
            }
        }
    }

    public bool PlayerMoving(Player player) {
        if (player.GetProgress() == 3) {
            return false;
        }
        else {
            return true;
        }
    }

    public void DecrementPlayersMoving() {
        playersmoving--;
    }
    #endregion

    #region Disable
    private void OnDisable() {
        GameManager.PlayerMoveEvent -= gotoLocation;
        GameManager.PointsEvent -= CalculatePoints;
        Player.MovementEndEvent -= DecrementPlayersMoving;
    }
    #endregion
}