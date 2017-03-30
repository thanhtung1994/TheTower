using UnityEngine;
using System.Collections;

public class Projective : MonoBehaviour {
	float damage;
	Transform target;
	bool isLauched;
	bool critial;
	bool hasTarget;
	Vector3 lastTargetPos;
	public void LauchProjectile(Transform target,float damage,bool critical){
		this.target = target;
		this.damage = damage;
		this.critial = critical;
		isLauched = true;
	}
	void Update(){
		if (!isLauched)
			return;
		if (!target.gameObject.activeSelf) {
			hasTarget = false;
			lastTargetPos = target.position;
		}
		float baseSpeed = StatsHelper.ins.GetStatsValue (Stat.Speed, 0);
		float speedRatio = (baseSpeed - StatsHelper.ins.GetStatsValue (Stat.Speed));
		speedRatio = speedRatio / baseSpeed;
		if (speedRatio < 0.4f)
			speedRatio = 0.4f;
		float speed = (speedRatio * 40) * TimeManager.DeltaTime;
		transform.position = Vector3.MoveTowards (transform.position,(hasTarget)?target.position:lastTargetPos,speed);
		transform.LookAt (target);
		if (Vector3.Distance (transform.position,(hasTarget)?target.position:lastTargetPos) < 1f)
			OnArrival ();
	}
	void OnArrival(){
		if(hasTarget)
		target.GetComponent<EnemyController> ().TakeDamage (damage,critial);
		Destroy (gameObject);
	}
}
