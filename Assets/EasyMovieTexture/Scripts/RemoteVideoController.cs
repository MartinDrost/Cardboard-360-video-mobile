using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using JsonFx.Json;
using System;
using AssemblyCSharp;

public class RemoteVideoController : MonoBehaviour {

    public MediaPlayerCtrl scrMedia;
    
    // Use this for initialization
    void Start()
    {
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
		scrMedia.Load("http://martindrost.nl/rock360.mp4");
		new Thread((ThreadStart)delegate()
		{
			while(true)
			{
					continue;
					string data = new WebClient().DownloadString("http://martindrost.nl/vr/getActions?id=" + lastProcessedAction);
					VRAction[] actions = JsonReader.Deserialize<VRAction[]>(data);
					
					foreach(VRAction action in actions)
					{
						Debug.Log("New action: " + action.action);
						DateTime targetDate = Convert.ToDateTime(action.time);
						DateTime currentDate = DateTime.Now;

						int timeout = 0;
						if(targetDate > currentDate)
						{
							TimeSpan timeSpan = targetDate - currentDate;
							timeout = (int) timeSpan.TotalMilliseconds;
						}

						switch(action.action)
						{
							case "load":
								new Thread((ThreadStart)delegate()
								{
									Thread.Sleep(timeout);
									scrMedia.Load(action.details);
								}).Start();
								break;
							case"play":
								new Thread((ThreadStart)delegate()
								{
									Thread.Sleep(timeout);
									scrMedia.Play();
								}).Start();
								break;
							case"pause":
								new Thread((ThreadStart)delegate()
								{
									Thread.Sleep(timeout);
									scrMedia.Pause();
								}).Start();
								break;
							case"stop":
								new Thread((ThreadStart)delegate()
								{
									Thread.Sleep(timeout);
									scrMedia.Stop();
								});
								break;
							case "seek":
								new Thread((ThreadStart)delegate()
								{
									Thread.Sleep(timeout);
									scrMedia.SeekTo(Int32.Parse(action.details));
								}).Start();
								break;
						}

						lastProcessedAction = Int32.Parse(action.id);
					}

					Thread.Sleep(5000);
			}
		}).Start();
	}

    // Update is called once per frame
    void Update () {

    }

    void OnGUI()
    {
        GUI.Label(new Rect(new Vector2(0, 0), new Vector2(Screen.width, Screen.height)), " url's called");
	}
}