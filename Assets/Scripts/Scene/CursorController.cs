using UnityEngine;

public class CursorController : MonoBehaviour
{
	[SerializeField] private bool isLockedOnStart = true;
	[SerializeField] private bool isSwitchable = true;

	void Start()
	{
		Cursor.lockState = isLockedOnStart ? CursorLockMode.Locked : CursorLockMode.Confined;
	}

	private void Update()
	{
		if (!isSwitchable)
			return;

		if (Input.GetKeyUp(KeyCode.Escape))
			Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.Confined : CursorLockMode.Locked;
	}
}