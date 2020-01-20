using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour {

    #region Public Variables
    public static AudioClip points0, points1, points2, points3;
    static AudioSource audioSrc;
    public static AudioClip[] pointsclips;
    #endregion

    #region Initialization
    // Start is called before the first frame update
    void Start() {
        points0 = Resources.Load<AudioClip>("Bounce 8");
        points1 = Resources.Load<AudioClip>("Bounce 15");
        points2 = Resources.Load<AudioClip>("Button 10");
        points3 = Resources.Load<AudioClip>("Button 25");

        audioSrc = GetComponent<AudioSource>();

        pointsclips = new AudioClip[4];
        pointsclips[0] = points0;
        pointsclips[1] = points1;
        pointsclips[2] = points2;
        pointsclips[3] = points3;
    }
    #endregion

    #region Sound
    public static void PlaySound() {
        audioSrc.PlayOneShot(pointsclips[Random.Range(0, 4)]);
    }
    #endregion
}
