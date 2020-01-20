using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour {

    #region Events and Delegates
    public delegate void MovementEnd();
    public static event MovementEnd MovementEndEvent;
    #endregion

    #region Public Variables
    public string Username;
    public int Points;
    public int currLocation;
    public int endLocation;
    public string stringcurrLocation;
    public string Color;

    public Animator animator;
   

    float ElapsedTime;
    float FinishTime;
    Vector2 StartPosition;
    Vector2 Target;
    #endregion

    #region Private Variables
    private int[] pathway = new int[3];
    private int progress = 3;
    private bool left = true;
    private bool leader = true;
    private bool[] boolpicker = { true, false };

    private float speed = 4;
    #endregion

    #region Initialization
    void Start()
    {
        Vector2 newPos = gameObject.transform.position;
        newPos.x = UnityEngine.Random.Range((float)Consts.BRIDGE_X_LOWER, (float)Consts.BRIDGE_X_UPPER);
        newPos.y = UnityEngine.Random.Range((float)Consts.BRIDGE_Y_LOWER, (float)Consts.BRIDGE_Y_UPPER);
        gameObject.transform.position = newPos;

        StartPosition = gameObject.transform.position;
        Target = gameObject.transform.position;
    }
    #endregion

    #region Update
    void Update()
    {
        if (progress < 3) {
            animator.SetBool("Eat", false);
            animator.SetBool("Rest", false);
            animator.SetBool("Play", false);
            animator.SetBool("Fish", false);
            if (gameObject.transform.position.x == Target.x && gameObject.transform.position.y == Target.y) {
                currLocation = pathway[progress];
                SetLocationString(pathway[progress]);
                StartPosition = Target;
                progress += 1;
                if (progress < 3 && pathway[progress] != -4) {
                    LocationRandomizer(pathway[progress]);
                }
                else {
                    progress = 3;
                    animator.SetBool("PlaceLeft", boolpicker[UnityEngine.Random.Range(0, 2)]);
                    switch (endLocation) {
                        case 0:
                            animator.SetBool("Eat", true);
                            break;
                        case 1:
                            animator.SetBool("Rest", true);
                            animator.SetBool("Sit", boolpicker[UnityEngine.Random.Range(0, 2)]);
                            break;
                        case 2:
                            animator.SetBool("Play", true);
                            break;
                        case 3:
                            animator.SetBool("Fish", true);
                            break;
                        case -20:
                            switch (UnityEngine.Random.Range(0, 4)) {
                                case 0:
                                    animator.SetBool("Eat", true);
                                    break;
                                case 1:
                                    animator.SetBool("Rest", true);
                                    animator.SetBool("Sit", boolpicker[UnityEngine.Random.Range(0, 2)]);
                                    break;
                                case 2:
                                    animator.SetBool("Play", true);
                                    break;
                                case 3:
                                    animator.SetBool("Fish", true);
                                    break;
                            }
                            break;
                        case -30:
                            switch (UnityEngine.Random.Range(0, 4)) {
                                case 0:
                                    animator.SetBool("Eat", true);
                                    break;
                                case 1:
                                    animator.SetBool("Rest", true);
                                    animator.SetBool("Sit", boolpicker[UnityEngine.Random.Range(0, 2)]);
                                    break;
                                case 2:
                                    animator.SetBool("Play", true);
                                    break;
                                case 3:
                                    animator.SetBool("Fish", true);
                                    break;
                            }
                            break;
                    }
                    try {
                        MovementEndEvent();
                    }
                    catch (NullReferenceException menu) {
                    }
                }
            }
            gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, Target, speed * Time.deltaTime);
        }
    }
    #endregion

    #region Accessor/Setter
    internal int GetProgress() {
        return progress;
    }

    internal bool GetLeader() {
        return leader;
    }

    internal void SetLeader(bool value) {
        leader = value;
        animator.SetBool("Leader", leader);
        if (leader) {
            this.gameObject.GetComponentInChildren<TextMeshPro>().text = Username;
            this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 11;
        }
        else {
            this.gameObject.GetComponentInChildren<TextMeshPro>().text = "";
            this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 10;
        }
    }
    #endregion

    #region Location/Moving
    internal void MoveToLocation(int[] chosenpath) {
        pathway = chosenpath;
        for (int i = 2; i >= 0; i--) {
            if (pathway[i] != -4) {
                endLocation = pathway[i];
                break;
            }
        }
        if (left == true && (pathway[0] == 0 || pathway[0] == 3)) {
            pathway[1] = pathway[0];
            pathway[0] = -3;
        } else if ((left == false && (pathway[0] == 1 || pathway[0] == 2))) {
            pathway[1] = pathway[0];
            pathway[0] = -2;
        }

        switch (endLocation) {
            case 0:
                left = false;
                break;
            case 1:
                left = true;
                break;
            case 2:
                left = true;
                break;
            case 3:
                left = false;
                break;
        }
        animator.SetBool("Left", left);

        progress = 0;
        LocationRandomizer(pathway[progress]);
    }

    internal void SetLocationString(int location) {
        switch (location) {
            case 0:
                stringcurrLocation = "eat";
                break;
            case 1:
                stringcurrLocation = "rest";
                break;
            case 2:
                stringcurrLocation = "play";
                break;
            case 3:
                stringcurrLocation = "fish";
                break;
            case -2:
                stringcurrLocation = "lbridge";
                break;
            case -3:
                stringcurrLocation = "rbridge";
                break;
        }
    }

    internal void LocationRandomizer(int location) {
        switch (location) {
            case 0:
                Target.x = UnityEngine.Random.Range((float)Consts.EAT_X_LOWER, (float)Consts.EAT_X_UPPER);
                Target.y = UnityEngine.Random.Range((float)Consts.EAT_Y_LOWER, (float)Consts.EAT_Y_UPPER);
                break;
            case 1:
                Target.x = UnityEngine.Random.Range((float)Consts.REST_X_LOWER, (float)Consts.REST_X_UPPER);
                Target.y = UnityEngine.Random.Range((float)Consts.REST_Y_LOWER, (float)Consts.REST_Y_UPPER);
                break;
            case 2:
                Target.x = UnityEngine.Random.Range((float)Consts.PLAY_X_LOWER, (float)Consts.PLAY_X_UPPER);
                Target.y = UnityEngine.Random.Range((float)Consts.PLAY_Y_LOWER, (float)Consts.PLAY_Y_UPPER);
                break;
            case 3:
                Target.x = UnityEngine.Random.Range((float)Consts.FISH_X_LOWER, (float)Consts.FISH_X_UPPER);
                Target.y = UnityEngine.Random.Range((float)Consts.FISH_Y_LOWER, (float)Consts.FISH_Y_UPPER);
                break;
            case -2:
                Target.x = UnityEngine.Random.Range((float)Consts.LEFTBRIDGE_X_LOWER, (float)Consts.LEFTBRIDGE_X_UPPER);
                Target.y = UnityEngine.Random.Range((float)Consts.LEFTBRIDGE_Y_LOWER, (float)Consts.LEFTBRIDGE_Y_UPPER);
                break;
            case -3:
                Target.x = UnityEngine.Random.Range((float)Consts.RIGHTBRIDGE_X_LOWER, (float)Consts.RIGHTBRIDGE_X_UPPER);
                Target.y = UnityEngine.Random.Range((float)Consts.RIGHTBRIDGE_Y_LOWER, (float)Consts.RIGHTBRIDGE_Y_UPPER);
                break;
            case -20:
                Target.x = UnityEngine.Random.Range((float)Consts.MENU_LEFTBRIDGE_X_LOWER, (float)Consts.MENU_LEFTBRIDGE_X_UPPER);
                Target.y = UnityEngine.Random.Range((float)Consts.MENU_LEFTBRIDGE_Y_LOWER, (float)Consts.MENU_LEFTBRIDGE_Y_UPPER);
                break;
            case -30:
                Target.x = UnityEngine.Random.Range((float)Consts.MENU_RIGHTBRIDGE_X_LOWER, (float)Consts.MENU_RIGHTBRIDGE_X_UPPER);
                Target.y = UnityEngine.Random.Range((float)Consts.MENU_RIGHTBRIDGE_Y_LOWER, (float)Consts.MENU_RIGHTBRIDGE_Y_UPPER);
                break;
        }
    }
    #endregion
}
