using UnityEngine;
using Mirror;
using TMPro;

public class Player : NetworkBehaviour
{
	[Tooltip("Display player name")]
	[SerializeField] private TextMeshPro nameBoard;
	[SyncVar(hook = nameof(ChangeNameOnBoard))]
	private string playerName;
	[SyncVar(hook = nameof(ChangeFragsOnBoard))]
	private int frags = 0;

	private ISkill skill;
	private IDamageHandler playerState;
	private IMoveController moveController;
	private PlayerSettings settings;
	public System.Action<Player> CountFragsIncreased;

	public int Frags { get => frags; set => frags = value; }
	public string PlayerName { get => playerName; set => playerName = value; }
	public bool IsMoveBlocked { get => moveController.IsMoveBlocked; }

	private void OnValidate()
	{
		if (this.GetComponent<ISkill>() == null)
			Debug.LogError("Error: Player dont find ISkill component.\n Add ISkill component");
		if (this.GetComponent<IMoveController>() == null)
			Debug.LogError("Error: Player dont find IMoveController component.\n Add IMoveController component");
		if (this.GetComponent<IDamageHandler>() == null)
			Debug.LogError("Error: Player dont find IDamageHandler component.\n Add IDamageHandler component");
	}

	void Start()
	{
		moveController = this.GetComponent<IMoveController>();
		skill = this.GetComponent<ISkill>();
		skill.OnHittingEnemy += CmdHitEnemy;
	}

	private void Update()
	{
		if (Input.GetMouseButtonUp(0) && !IsMoveBlocked)
			skill.StartSkill();
	}

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();
		settings = FindObjectOfType<PlayerSettings>();
		CmdSetName(settings.PlayerName);
	}

	private void ChangeNameOnBoard(string oldName, string newName)
	{
		if (isLocalPlayer)
			nameBoard.text = frags.ToString();
		else
			nameBoard.text = $"{newName}\n{frags}";
	}

	[Command]
	public void CmdSetName(string newName)
	{
		playerName = newName;
	}

	private void ChangeFragsOnBoard(int oldFrags, int newFrags)
    {
		if(isLocalPlayer)
			nameBoard.text = newFrags.ToString();
		else
			nameBoard.text = $"{playerName}\n{newFrags}";
	}

	public void BlockMove()
	{
		moveController.IsMoveBlocked = true;
	}

	public void UnblockMove()
	{
		moveController.IsMoveBlocked = false;
	}

	[ClientRpc]
	public void RpcSetPosition(Vector3 pos)
	{
		this.transform.position = pos;
	}

	[Command]
	private void CmdHitEnemy(GameObject go)
	{
		IDamageHandler enemyPlayerState = go.GetComponent<IDamageHandler>();
		if (!enemyPlayerState.TakingDamage())
			return;

		++frags;
		CountFragsIncreased(this);
	}
}