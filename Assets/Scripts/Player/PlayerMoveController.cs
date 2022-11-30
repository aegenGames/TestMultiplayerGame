using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))]
public class PlayerMoveController : NetworkBehaviour, IMoveController
{
	[SerializeField] private CharacterController characterController;
	[Header("Speed")]
	[SerializeField] private float moveSpeed = 1;
	[SerializeField] private float rotateSpeed = 1;
	[SerializeField]
	[SyncVar] private bool isMoveBlocked = false;
	private Animator anim;

	private CameraFollowPlayer followCamera;
	public bool IsMoveBlocked { get => isMoveBlocked; set => isMoveBlocked = value; }

	void OnValidate()
	{
		if (characterController == null)
			characterController = GetComponent<CharacterController>();
	}

	private void Awake()
	{
		anim = GetComponent<Animator>();
	}

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();
		followCamera = Camera.main.GetComponent<CameraFollowPlayer>();
		if (!followCamera)
			followCamera = Camera.main.gameObject.AddComponent<CameraFollowPlayer>();

		Transform target = this.transform.Find("TargetForCamera");
		followCamera.Target = target.transform;
		isMoveBlocked = false;
	}

	private void Update()
	{
		anim.SetBool("isMoving", false);
		if (!isMoveBlocked)
			Move();
	}

	private void Move()
	{
		if (!isLocalPlayer || characterController == null || !characterController.enabled
			|| Cursor.lockState != CursorLockMode.Locked)
			return;

		Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		if (direction.Equals(Vector3.zero))
			return;

		anim.SetBool("isMoving", true);
		if (direction.z >= 0)
			anim.SetFloat("moveSpeed", moveSpeed / 2);
		else
			anim.SetFloat("moveSpeed", -moveSpeed / 2);

		Quaternion rotate = followCamera.transform.rotation;
		rotate.x = 0; rotate.z = 0;
		this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotate, Time.deltaTime * rotateSpeed);

		direction = transform.TransformDirection(direction);
		direction *= moveSpeed * Time.deltaTime;
		characterController.Move(direction);
	}
}