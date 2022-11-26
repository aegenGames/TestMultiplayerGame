using System.Collections;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(MaterialChanger))]
public class PlayerState : NetworkBehaviour, IDamageHandler
{
	[Tooltip("Time of color change and invulnerability")]
	[SerializeField] private float InvulnerableTime = 3;
	[Tooltip("Color of player, when he take dmg")]
	[SerializeField] private Color changedColor = Color.red;

	[SyncVar] private bool isInvulnerable = false;

	public bool IsInvulnerable { get => isInvulnerable; }

	public bool TakingDamage(float dmg = 0)
	{
		if (isInvulnerable)
			return false;

		StartCoroutine(ChangeState());
		return true;
	}

	public IEnumerator ChangeState()
	{
		ChangeInvulnerableState(true);
		yield return new WaitForSeconds(InvulnerableTime);
		ChangeInvulnerableState(false);
	}

	private void ChangeInvulnerableState(bool setInvulnerability)
	{
		isInvulnerable = setInvulnerability;
		Color color = setInvulnerability ? changedColor : Color.white;
		SetColor(color);
	}

	private void SetColor(Color color)
	{
		if (isServer)
			this.GetComponent<MaterialChanger>().SetColor(color);
		else
			this.GetComponent<MaterialChanger>().CmdSetColor(color);
	}
}