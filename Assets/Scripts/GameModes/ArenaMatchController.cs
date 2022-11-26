using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class ArenaMatchController : NetworkBehaviour
{
	[SerializeField] private int countFragsForWin = 3;
	[SerializeField] private int timeBeforeNewRound = 5;
	[Tooltip("GameObject, for display after win")]
	[SerializeField] private GameObject endRoundObject;
	[SerializeField] private Transform startPositionsParent;

	private TextMeshProUGUI winnerNameTMP;
	private TextMeshProUGUI countdownTimeTMP;

	private List<Player> players = new List<Player>();
	private Transform[] startPositions;

	private void Start()
	{
		winnerNameTMP = endRoundObject.transform.Find("WinnerName").GetComponent<TextMeshProUGUI>();
		countdownTimeTMP = endRoundObject.transform.Find("CountdownTime").GetComponent<TextMeshProUGUI>();

		startPositions = new Transform[startPositionsParent.childCount];
		for (int i = 0; i < startPositions.Length; ++i)
			startPositions[i] = startPositionsParent.GetChild(i);
	}

	public List<Player> Players { get => players; set => players = value; }

	public void AddPlayer(Player player)
	{
		player.CountFragsIncreased += CheckNumberFrags;
		players.Add(player);
	}

	/// <summary>
	/// If one of the players is near position return true
	/// </summary>
	public bool CheckNearPlayers(Vector3 position)
	{
		if (players.Count == 0)
			return false;

		float minDistance = players[0].GetComponent<Collider>().bounds.size.x * players[0].transform.localScale.x;
		foreach(Player player in players)
		{
			if (Vector3.Distance(position, player.transform.position) < minDistance)
				return true;
		}
		return false;
	}

	/// <summary>
	/// If 
	/// </summary>
	/// <param name="player"></param>
	public void CheckNumberFrags(Player player)
	{
		if (player.Frags == countFragsForWin)
		{
			RpcEndRound(player.PlayerName);
		}
	}

	[ClientRpc]
	public void RpcEndRound(string newName)
	{
		endRoundObject.SetActive(true);
		winnerNameTMP.text = newName + "\nWIN";
		foreach (Player player in players)
		{
			player.Frags = 0;
			player.BlockMove();
		}
		StartCoroutine(Countdown());
	}

	private IEnumerator Countdown()
	{
		int time = timeBeforeNewRound;
		while (time >= 0)
		{
			countdownTimeTMP.text = time.ToString();
			yield return new WaitForSeconds(1);
			--time;
		}
		endRoundObject.SetActive(false);
		RestartMatch();
		yield return new WaitForSeconds(1f);
	}

	[ServerCallback]
	private void RestartMatch()
	{
		GeneralAlgoritms.ShuffleArray(startPositions);
		int i = 0;
		foreach (Player player in players)
		{
			player.UnblockMove();
			player.RpcSetPosition(startPositions[i++].position);
		}
	}
}
