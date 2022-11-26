using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkManager))]
public class ArenaNetworkManagerHUD : MonoBehaviour
{
	[SerializeField] private PlayerSettings playerSettings;

	private ArenaNetworkManager manager;
	private bool showWarningWindow = false;

	void Awake()
	{
		manager = GetComponent<ArenaNetworkManager>();
	}

	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(10, 40, 215, 9999));
		if (!NetworkClient.isConnected && !NetworkServer.active)
		{
			StartButtons();
		}
		else
		{
			StatusLabels();
		}

		if (NetworkClient.isConnected && !NetworkClient.ready)
		{
			if (GUILayout.Button("Client Ready"))
			{
				NetworkClient.Ready();
				if (NetworkClient.localPlayer == null)
				{
					NetworkClient.AddPlayer();
				}
			}
		}

		StopButtons();

		GUILayout.EndArea();
	}

	void StartButtons()
	{
		if (!NetworkClient.active)
		{
			playerSettings.PlayerName = GUILayout.TextField(playerSettings.PlayerName);

			if (Application.platform != RuntimePlatform.WebGLPlayer)
			{
				if (GUILayout.Button("Host (Server + Client)"))
				{
					if (playerSettings.PlayerName.Equals("Input name"))
						showWarningWindow = true;
					else
						manager.StartHost();
				}
			}

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Client"))
			{
				if(playerSettings.PlayerName.Equals("Input name"))
					showWarningWindow = true;
				else
					manager.StartClient();
			}

			if(showWarningWindow)
				GUILayout.Window(0, new Rect(150, 200, 200, 50), x => { if (GUILayout.Button("ok")) showWarningWindow = false; }, "Enter name for start");
			
			manager.networkAddress = GUILayout.TextField(manager.networkAddress);
			GUILayout.EndHorizontal();

			if (Application.platform == RuntimePlatform.WebGLPlayer)
			{
				GUILayout.Box("(  WebGL cannot be server  )");
			}
			else
			{
				if (GUILayout.Button("Server Only")) manager.StartServer();
			}
		}
		else
		{
			GUILayout.Label($"Connecting to {manager.networkAddress}..");
			if (GUILayout.Button("Cancel Connection Attempt"))
			{
				manager.StopClient();
			}
		}
	}

	void StatusLabels()
	{
		if (Cursor.lockState == CursorLockMode.Locked)
			return;

		if (NetworkServer.active && NetworkClient.active)
		{
			GUILayout.Label($"<b>Host</b>: running via {Transport.active}");
		}

		else if (NetworkServer.active)
		{
			GUILayout.Label($"<b>Server</b>: running via {Transport.active}");
		}

		else if (NetworkClient.isConnected)
		{
			GUILayout.Label($"<b>Client</b>: connected to {manager.networkAddress} via {Transport.active}");
		}
	}

	void StopButtons()
	{
		if (Cursor.lockState == CursorLockMode.Locked)
			return;

		if (NetworkServer.active && NetworkClient.isConnected)
		{
			if (GUILayout.Button("Stop Host"))
			{
				manager.StopHost();
			}
		}

		else if (NetworkClient.isConnected)
		{
			if (GUILayout.Button("Stop Client"))
			{
				manager.StopClient();
			}
		}

		else if (NetworkServer.active)
		{
			if (GUILayout.Button("Stop Server"))
			{
				manager.StopServer();
			}
		}

		if (GUILayout.Button("Exit Game"))
		{
			Application.Quit();
		}
	}
}