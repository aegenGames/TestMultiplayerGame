using UnityEngine;
using Mirror;
using TMPro;

[RequireComponent(typeof(DashSkill))]
public class Player : NetworkBehaviour
{
	[SerializeField] private DashSkill skill;
	[Tooltip("Display player name")]
	[SerializeField] private TextMeshPro nameBoard;
	[SerializeField]
	[SyncVar(hook = nameof(OutputNameOnBoard))]
	private string playerName;
	[SyncVar] private int frags = 0;

	private IDamageHandler playerState;
	private IMoveController moveController;
	private PlayerSettings settings;
	public System.Action<Player> CountFragsIncreased;

	public int Frags { get => frags; set => frags = value; }
	public string PlayerName { get => playerName; set => playerName = value; }
	public bool IsMoveBlocked { get => moveController.IsMoveBlocked; }

	private void OnValidate()
	{
		if (this.GetComponent<IMoveController>() == null)
			Debug.LogError("Error: Player dont find IMoveController component.\n Add IMoveController component");
		if (this.GetComponent<IDamageHandler>() == null)
			Debug.LogError("Error: Player dont find IDamageHandler component.\n Add IDamageHandler component");
	}

	void Start()
	{
		moveController = this.GetComponent<IMoveController>();
		skill.OnAttackBegin += BlockMove;
		skill.OnAttackEnd += UnblockMove;
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
		CmdOutputNameOnBoard(settings.PlayerName);
		nameBoard.gameObject.SetActive(false);
	}

	private void OutputNameOnBoard(string oldName, string newName)
	{
		nameBoard.text = newName;
	}

	[Command]
	public void CmdOutputNameOnBoard(string newName)
	{
		playerName = newName;
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