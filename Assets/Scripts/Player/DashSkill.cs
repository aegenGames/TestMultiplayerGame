using System.Collections;
using UnityEngine;
using Mirror;

public class DashSkill : NetworkBehaviour, ISkill
{
	[Header("Parameters of attack")]
	[SerializeField] private float dashDistance = 3;
	[Tooltip("Time for overcoming the full dash distance")]
	[SerializeField] private float timeAction = 1;
	[SerializeField] private float cooldown = 3;

	[Header("Animation")]
	[SerializeField] private Animator dashAnimation;

	[Header("Collider")]
	[Tooltip("Radius of Controller in during attack")]
	[SerializeField] private float radiusColliderInAttack;
	[Tooltip("Height of Controller in during attack")]
	[SerializeField] private float heightColliderInAttack;
	[SyncVar(hook = nameof(ChangeCollider))]
	private bool reduceCollider;
	// Radius and height of Controller in open state
	private float radiusColliderInCalm;
	private float heightColliderInCalm;

	private bool isCooldown = false;
	private bool isAttacking = false;

	private IMoveController moveController;
	private CharacterController characterController;
	private CapsuleCollider meshCollider;

    public event ISkill.HitHandler OnHittingEnemy;

    private void Start()
	{
		moveController = this.GetComponent<IMoveController>();
		characterController = this.GetComponent<CharacterController>();
		meshCollider = this.GetComponent<CapsuleCollider>();
		dashAnimation.SetFloat("closingSpeed", 5 / timeAction);
		radiusColliderInCalm = characterController.radius;
		heightColliderInCalm = characterController.height;
	}

	public void StartSkill()
	{
		if (characterController == null || !characterController.enabled ||
			Cursor.lockState != CursorLockMode.Locked || !isLocalPlayer)
			return;

		if (dashAnimation)
		{
			dashAnimation.SetBool("isDashing", true);
		}
		StartCoroutine(StartAttack());
	}

	private void ChangeCollider(bool oldValue, bool newValue)
	{
		if (oldValue == newValue)
			return;

		float diffuseRadius = radiusColliderInCalm - radiusColliderInAttack;
		float heightDiffuse = heightColliderInCalm - heightColliderInAttack;
		Vector3 offsetControllerPos = Vector3.up * (radiusColliderInCalm - radiusColliderInAttack);
		if (newValue)
		{
			diffuseRadius = -diffuseRadius;
			heightDiffuse = -heightDiffuse;
			offsetControllerPos = -offsetControllerPos;
		}
		
		characterController.radius += diffuseRadius;
		characterController.height += heightDiffuse;
		characterController.center += offsetControllerPos;
		meshCollider.radius += diffuseRadius;
		meshCollider.height += heightDiffuse;
		meshCollider.center += offsetControllerPos;
	}

	[Command]
	private void CmdChangeController(bool isReduce)
	{
		reduceCollider = isReduce;
	}

	private IEnumerator StartAttack()
	{
		if (isCooldown)
			yield break;

		CmdChangeController(true);
		moveController.IsMoveBlocked = true;

		isCooldown = true;
		isAttacking = true;
		float deltaTime = Time.fixedDeltaTime;
		float curTime = timeAction;
		Vector3 deltaMove = this.transform.forward * dashDistance / timeAction * deltaTime;

		while (curTime > 0)
		{
			characterController.Move(deltaMove);
			curTime -= deltaTime;
			yield return new WaitForSeconds(deltaTime);
		}
		StartCoroutine(FinishedAttack());
	}

	private IEnumerator FinishedAttack()
	{
		if (dashAnimation)
		{
			dashAnimation.SetBool("isDashing", false);
		}

		CmdChangeController(false);
		moveController.IsMoveBlocked = false;

		isAttacking = false;
		yield return new WaitForSeconds(cooldown);
		isCooldown = false;
	}

	[ClientCallback]
	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		string tag = hit.gameObject.tag;
		if (tag != "Player" && tag != "Environment" || !isAttacking)
			return;

		if (tag == "Player")
		{
			OnHittingEnemy(hit.gameObject);
		}

		StopAllCoroutines();
		StartCoroutine(FinishedAttack());
	}
}