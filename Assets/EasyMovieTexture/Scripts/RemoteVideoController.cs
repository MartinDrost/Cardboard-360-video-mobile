using UnityEngine;
using System.Collections;
using System.Threading;

public class RemoteVideoController : MonoBehaviour {

    public MediaPlayerCtrl scrMedia;
    
    // Use this for initialization
    void Start()
    {
        /*
        scrMedia.Load("http://martindrost.nl/rock360.mp4");
        scrMedia.Play();
        scrMedia.Stop();
        scrMedia.Pause();
        scrMedia.UnLoad();
        */
    }

    // Update is called once per frame
    int count = 0;
    void Update () {

    }

    void OnGUI()
    {
        GUI.Label(new Rect(new Vector2(0, 0), new Vector2(Screen.width, Screen.height)), count + "");
    }
}
