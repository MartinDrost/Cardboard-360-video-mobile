using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using JsonFx.Json;
using System;
using AssemblyCSharp;
using System.Collections.Generic;

public class RemoteVideoController : MonoBehaviour {

	public MediaPlayerCtrl scrMedia;
	private List<VRAction> actions;
	private bool playVideo;
    
    // Use this for initialization
    void Start()
    {
		playVideo = false;
		actions = new List<VRAction> ();
		fetchActionInterval ();
        /*
        scrMedia.Load("http://martindrost.nl/rock360.mp4");
        scrMedia.Play();
        scrMedia.Stop();
        scrMedia.Pause();
        scrMedia.UnLoad();
        */
    }

	int lastProcessedAction = 0;
	void fetchActionInterval()
	{
		new Thread((ThreadStart)delegate()
		{
			while(true)
			{
					string data = new WebClient().DownloadString("http://martindrost.nl/vr/getActions?id=" + lastProcessedAction);
					VRAction[] receivedActions = JsonReader.Deserialize<VRAction[]>(data);
					
					foreach(VRAction action in receivedActions)
					{
						actions.Add(action);
						lastProcessedAction = Int32.Parse(action.id);
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

		if (playVideo && scrMedia.GetCurrentState() != MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY) {
			scrMedia.Play ();
			playVideo = false;
		}
    }

    void OnGUI()
    {
        //GUI.Label(new Rect(new Vector2(0, 0), new Vector2(Screen.width, Screen.height)), " url's called");
	}
}