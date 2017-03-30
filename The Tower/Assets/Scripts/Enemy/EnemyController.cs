using UnityEngine;
using System.Collections;

public enum EnemyType{
	Tiny=0,
	Fast,
	Thought,
}
public class EnemyController : MonoBehaviour {
	public EnemyType type;
	public float hitpoint=20;
	public float attackPersecond=1.5f;
	public float damage=1;
	public float timeToTower=7.5f;
	Vector3 startPos;
	Vector3 endPos;

	float baseHitpoint;
	float baseDamage;
	float transition;
	float lastHit;
	public bool isAlive=false;
	// Use this for initialization
	void Awake () {
		baseHitpoint = hitpoint;
		baseDamage = damage;
	}
	public void LauchEnemy(){
		startPos = transform.position;
		endPos = startPos.normalized * (TheTower.ins.GetTowerWidth ()*0.3f);
		gameObject.SetActive (true);
		isAlive = true;
	}
	public void RemoveEnemy(){
		if (Random.value <= StatsHelper.ins.GetStatsValue (Stat.Luck)) {
			LootsManager.ins.DropItem ();
		}
		gameObject.SetActive (false);
		isAlive = false;
	}
	public void SetDifficulty(Difficulty diff){
		int waveDifficulty = StatsHelper.ins.GetWaveDifficultyValue (GameManager.ins.currentWave);
		hitpoint = baseHitpoint *waveDifficulty*(int)diff;
		damage = baseDamage * waveDifficulty * (int)diff;
	}
	public void TakeDamage(float amount,bool critical){
		hitpoint -= amount;
		CombatTextManager.ins.Show (amount.ToString(),32,(critical)? Color.red:Color.yellow,transform.position,Vector3.up,0.5f);
		if (hitpoint <= 0)
			RemoveEnemy ();
	}
	public void Reset(){
		transition = 0;
		lastHit = 0;
	}
	// Update is called once per frame
	void Update () {
		if (isAlive) {
			transition += TimeManager.DeltaTime* 1/timeToTower;
			transform.position = Vector3.Lerp (startPos, endPos, transition);
			if (transition > 1) {
				if (TimeManager.timer - lastHit > 3 / attackPersecond) {
					lastHit = TimeManager.timer;
					TheTower.ins.TakeDamage (damage);
				}
			}
		}
	}
}
