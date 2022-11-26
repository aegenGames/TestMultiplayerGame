using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
	private string playerName = "Input name";

	public string PlayerName { get => playerName; set => playerName = value; }
}
