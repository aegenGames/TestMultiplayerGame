using UnityEngine;

public interface ISkill
{
	public void StartSkill();

	public delegate void HitHandler(GameObject go);
	public event HitHandler OnHittingEnemy;
}
