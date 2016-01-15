using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using JsonFx.Json;
using System;
using AssemblyCSharp;
using System.Collections.Generic;
using System.Linq;

public class RemoteVideoController : MonoBehaviour {

	public MediaPlayerCtrl scrMedia;
	private List<VRAction> actions;
	private bool playVideo;
    private bool loadVideo;
    private GUIStyle guiStyle = new GUIStyle();
    List<int> processedActions;

    // Use this for initialization
    void Start()
    {
        playVideo = false;
        loadVideo = false;
        actions = new List<VRAction> ();
        processedActions = new List<int>();
        processedActions.Add(0);
        fetchActionInterval ();
    }

     
	void fetchActionInterval()
	{
		new Thread((ThreadStart)delegate()
		{
			while(true)
			{
					string data = new WebClient().DownloadString("http://martindrost.nl/vr/getActions?id=" + string.Join(",", processedActions.Select(x => x.ToString()).ToArray()));
					VRAction[] receivedActions = JsonReader.Deserialize<VRAction[]>(data);
					
					foreach(VRAction action in receivedActions)
					{
						actions.Add(action);
                        processedActions.Add(Int32.Parse(action.id));
					}

					Thread.Sleep(5000);
			}
		}).Start();
	}

    // Update is called once per frame
    void Update () {
		for(int i = 0; i < actions.Count; i++)
		{
			VRAction action = actions [i];

			DateTime targetDate = Convert.ToDateTime(action.time);
			DateTime currentDate = DateTime.Now;

			int timeout = 0;
			if(targetDate > currentDate)
			{
				TimeSpan timeSpan = targetDate - currentDate;
				timeout = (int) timeSpan.TotalMilliseconds;
			}

			if (timeout != 0)
				continue;
			
			switch(action.action)
			{
				case "load":
					scrMedia.Load(action.details);
                    loadVideo = true;
					break;
				case"play":
					playVideo = true;
					break;
				case"pause":
					scrMedia.Pause();
					break;
				case"stop":
					scrMedia.Stop();
					break;
				case "seek":
					scrMedia.SeekTo(Int32.Parse(action.details));
					break;
			}
			actions.RemoveAt (i);
			i--;
		}

		if (scrMedia.GetCurrentState() != MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY) {
            if (loadVideo)
            {
                if (scrMedia.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING && scrMedia.GetSeekPosition() > 10)
                {
                    scrMedia.Pause();
                }
                else if (scrMedia.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED)
                {
                    loadVideo = false;
                }
            }
            else if (playVideo)
            {
                scrMedia.Play(0);
                playVideo = false;
            }
        }
    }

    void OnGUI()
    {
        return;

        guiStyle.fontSize = 24;
        guiStyle.normal.textColor = Color.green;
        GUI.Label(new Rect(new Vector2(0, Screen.height-40), new Vector2(Screen.width, Screen.height)), scrMedia.GetSeekPosition() + " ms played", guiStyle);

        DateTime currentDate = DateTime.Now;
        GUI.Label(new Rect(new Vector2(0, Screen.height-80), new Vector2(Screen.width, Screen.height)), currentDate.Hour + ":" + currentDate.Minute + ":" + currentDate.Second, guiStyle);

        GUI.Label(new Rect(new Vector2(0, Screen.height - 120), new Vector2(Screen.width, Screen.height)), "Status: " + scrMedia.GetCurrentState(), guiStyle);
        
    }
}