using UnityEngine;
using Mirror;

public class MaterialChanger : NetworkBehaviour
{
	[SerializeField] private MeshRenderer mesh;
	[SerializeField]
	[SyncVar(hook = nameof(SyncColor))]
	private Color color = Color.red;

	public void SyncColor(Color _, Color newColor)
	{
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