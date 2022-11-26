using UnityEngine;
using Mirror;

public class MaterialChanger : NetworkBehaviour
{
	[SerializeField] private GameObject mesh;
	private NetworkIdentity objNetId;
	[SerializeField]
	[SyncVar(hook = nameof(SyncColor))]
	private Color color = Color.red;

	public void SyncColor(Color _, Color newColor)
	{
		MeshRenderer mesh = this.transform.GetChild(0).Find("body").GetComponent<MeshRenderer>();
		mesh.material.color = newColor;
	}

	[Command]
	public void CmdSetColor(Color newColor)
	{
		SetColor(newColor);
	}

	[Server]
	public void SetColor(Color newColor)
	{
		color = newColor;
	}
}