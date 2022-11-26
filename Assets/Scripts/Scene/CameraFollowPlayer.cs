using UnityEngine;
using Mirror;

public class CameraFollowPlayer : NetworkBehaviour
{
	[Tooltip("Camera obstacle layer")]
	[SerializeField] private LayerMask obstaclesLayer;
	[Tooltip("Distanse between camera and player")]
	[SerializeField] private Vector3 cameraDistanse = new Vector3(0, 0.2f, -1f);

	[Header("Rotation speed")]
	[Tooltip("Horizontal rotation speed")]
	[SerializeField] private float speedX = 1;
	[Tooltip("Vertical rotation speed")]
	[SerializeField] private float speedY = 1;

	[Header("Clamp")]
	[SerializeField] private float topClamp = 70;
	[SerializeField] private float bottomClamp = 0;

	private Transform target;
	private Vector3 previousTargetPos;
	private float currentRotationY;

	public Transform Target
	{
		get => target;
		set
		{
			target = value;
			SetFollowPos();
		}
	}

	void LateUpdate()
	{
		if (Cursor.lockState != CursorLockMode.Locked || !target)
			return;

		UpdatePosition();
		RotateCamera();
		CheckObstacle();
	}

	private void UpdatePosition()
	{
		Vector3 currentTargetPos = target.position;
		this.transform.position += target.position - previousTargetPos;
		previousTargetPos = currentTargetPos;
	}

	private void SetFollowPos()
	{
		previousTargetPos = target.position;
		this.transform.position = previousTargetPos + cameraDistanse;
		this.transform.eulerAngles = new Vector3(0f, 0f, 0f);
	}

	private void RotateCamera()
	{
		float rotationY = Input.GetAxis("Mouse Y");
		if(rotationY != 0)
		{
			rotationY *= speedY * Time.deltaTime;
			float newRotationY = Mathf.Clamp(currentRotationY + rotationY, -topClamp, -bottomClamp);
			if (newRotationY != currentRotationY)
			{
				float angle = newRotationY - currentRotationY;
				this.transform.RotateAround(target.position, -transform.right, angle);
				currentRotationY = newRotationY;
			}
		}

		float rotationX = Input.GetAxis("Mouse X");
		if (rotationX != 0)
		{
			rotationX *= speedX * Time.deltaTime;
			this.transform.RotateAround(target.position, Vector3.up, rotationX);
		}
	}

	private void CheckObstacle()
	{
		Vector3 curPosition = this.transform.position;
		float distance = Vector3.Distance(curPosition, target.position);

		RaycastHit hit;
		if (Physics.Raycast(target.position, curPosition - target.position, out hit, Mathf.Abs(cameraDistanse.z), obstaclesLayer))
		{
			curPosition = hit.point;
		}
		else if (distance < Mathf.Abs(cameraDistanse.z) && !Physics.Raycast(curPosition, -this.transform.forward, 0.1f, obstaclesLayer))
		{
			curPosition -= this.transform.forward * 0.05f;
		}
		
		this.transform.position = curPosition;
	}
}