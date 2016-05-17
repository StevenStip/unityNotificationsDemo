using UnityEngine;
using System.Collections;
using DeltaDNA; 

public class MainBehaviourScript : MonoBehaviour
{
	public const string COLLECT_URL = "http://collect8634ntynt.deltadna.net/collect/api";
	public const string ENGAGE_URL = "http://engage8634ntynt.deltadna.net";
	public const string ENVIRONMENT_KEY = "77068300723337397081646175814611";

	Camera camera;

	// Use this for initialization
	void Start ()
	{
		camera = GetComponent<Camera> ();
		camera.clearFlags = CameraClearFlags.SolidColor;

		Debug.Log ("hi");
		DDNA.Instance.SetLoggingLevel (DeltaDNA.Logger.Level.DEBUG);
		DDNA.Instance.ClientVersion = "0.1";
		DDNA.Instance.StartSDK (ENVIRONMENT_KEY, COLLECT_URL, ENGAGE_URL);

		// iOS push notifications
		DDNA.Instance.IosNotifications.OnDidRegisterForPushNotifications += (string n) => {
			Debug.Log ("Got an iOS push token: " + n);
		};
		DDNA.Instance.IosNotifications.OnDidReceivePushNotification += (string n) => {
			Debug.Log ("Got an iOS push notification! " + n);
		};
		DDNA.Instance.IosNotifications.RegisterForPushNotifications ();


		// Android push notifications:
		DDNA.Instance.AndroidNotifications.OnDidRegisterForPushNotifications += (string n) => {
			Debug.Log ("Got an Android registration token: " + n);
		};
		DDNA.Instance.AndroidNotifications.OnDidFailToRegisterForPushNotifications += (string n) => {
			Debug.Log ("Failed getting an Android registration token: " + n);
		};
		DDNA.Instance.AndroidNotifications.RegisterForPushNotifications ();


	}


	void OnGUI ()
	{
		GUI.skin.textField.wordWrap = true;
		GUI.skin.button.fontSize = 36;

		int xOffset = 0;
		int yOffset = 0;

		if (GUI.Button (new Rect (yOffset+=10, xOffset+=10, 300, 100), "do engage stuff")) {
			Debug.Log ("do engage stuff");

			var engagement = new Engagement("startMission")
				.AddParam("missionID", "tutorial1");

			DDNA.Instance.RequestEngagement(engagement, (System.Collections.Generic.Dictionary<string, object> response) => {
				Debug.Log("engagement");
				foreach (System.Collections.Generic.KeyValuePair<string, object> temp in response){
					Debug.Log(temp.Key);
				}
					

				Debug.Log(response.Values.ToString());

				System.Collections.Generic.Dictionary<string, object> parameters = (System.Collections.Generic.Dictionary<string, object>)response["parameters"];
				if (parameters.ContainsKey("isTutorial")){
					Debug.Log("the parameter: ");
					if (System.Convert.ToBoolean(parameters["isTutorial"])){
						camera.backgroundColor = Color.green;
					}else{
						camera.backgroundColor = Color.red;
					}
					Debug.Log(parameters["isTutorial"].ToString());
				
				}else{
					Debug.Log("Key not found");
				}

			});
		}
		if (GUI.Button (new Rect (yOffset, xOffset+=110, 300, 100), "image engage")) {
			var engagement = new Engagement ("openMenu");

			DDNA.Instance.RequestEngagement (engagement, (response) => {

				ImageMessage imageMessage = ImageMessage.Create (response);

				// Check we got an engagement with a valid image message.
				if (imageMessage != null) {   
					// Download the image message resources.
					Debug.Log("Image received");
					imageMessage.FetchResources ();
					imageMessage.OnDidReceiveResources += () => {
						Debug.Log("Image Message loaded resources.");
						imageMessage.Show();
					};				
				} else {
					Debug.Log("No image message returned");
				}
			}, (exception) => {
				Debug.Log ("Engage reported an error: " + exception.Message);
			});

		}

		if (GUI.Button (new Rect (yOffset, xOffset+=110, 300, 100), "Clear persistent data")) {
			DDNA.Instance.ClearPersistentData ();
		}

		if (GUI.Button (new Rect (yOffset, xOffset+=110, 300, 100), "record event")) {
			// Build a game event with a couple of event parameters
			GameEvent optionsEvent = new GameEvent ("options")
				.AddParam ("option", "Music")
				.AddParam ("action", "Disabled");

			// Record the event
			DDNA.Instance.RecordEvent (optionsEvent);
		}
	}
}