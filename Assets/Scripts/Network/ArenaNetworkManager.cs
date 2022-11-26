using UnityEngine;
using Mirror;

public class ArenaNetworkManager : NetworkManager
{
	[Header("Arena")]
	[SerializeField] private ArenaMatchController matchController;

	public override void OnStartClient()
	{
		base.OnStartClient();
		GameObject go = GameObject.Find("MatchController");
		if(go)
			matchController = go.GetComponent<ArenaMatchController>();
	}

	public override void OnServerAddPlayer(NetworkConnectionToClient conn)
	{
		Transform startPos = GetFreeRandomPosition();
		GameObject player = Instantiate(playerPrefab, startPos.position, startPos.rotation);
		player.name = $"Player [connId={conn.connectionId}]";
		NetworkServer.AddPlayerForConnection(conn, player);

		matchController.AddPlayer(player.GetComponent<Player>());
	}

	public override void OnServerDisconnect(NetworkConnectionToClient conn)
	{
		matchController.Players.Remove(conn.identity.GetComponent<Player>());
		base.OnServerDisconnect(conn);
	}

	private Transform GetFreeRandomPosition()
	{
		if (matchController.Players.Count > startPositions.Count)
			return GetStartPosition();

		Transform startPos;
		while (true)                                                             //When random metod is used, can returned 
		{                                                                        //occupied start point, check for it.
			startPos = GetStartPosition();
			if (!matchController.CheckNearPlayers(startPos.position))
			{
				return startPos;
			}
		}
	}
}