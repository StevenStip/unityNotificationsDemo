using UnityEngine;
using System.Collections;
using DeltaDNA;

public class MainBehaviourScript : MonoBehaviour
{
	public const string COLLECT_URL = "http://collect8634ntynt.deltadna.net/collect/api";
	public const string ENGAGE_URL = "http://engage8634ntynt.deltadna.net";
	public const string ENVIRONMENT_KEY = "77068300723337397081646175814611";

	// Use this for initialization
	void Start ()
	{
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

		if (GUI.Button (new Rect (10, 10, 300, 100), "Button Event")) {
			Debug.Log ("Button pressed");
		}

	}
}