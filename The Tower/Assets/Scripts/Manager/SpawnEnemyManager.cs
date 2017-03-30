using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public struct SpawnInformation{
	public EnemyType type;
	public float time;
	public Difficulty diff;
}
public class SpawnEnemyManager : MonoBehaviour {
	const float ZONE_ANGLE=72;
	const float SPAWN_DISTANCE=20;
	const int AMN_ZONES=5;

	public static SpawnEnemyManager ins;
	public GameObject[] enemyPretabs;

	int zoneIndex;
	List<EnemyController> enemiesPool=new List<EnemyController>();
	// Use this for initialization
	void Start () {
		if (ins == null)
			ins = this;
		else
			Destroy (gameObject);
	}

	public void SpawnEnemy(EnemyType type,Difficulty diff){
		float zonePostion = Random.Range (0, ZONE_ANGLE);
		zonePostion += zoneIndex * ZONE_ANGLE;
		Vector3 dir = Vector3.forward * SPAWN_DISTANCE;
		Quaternion angle = Quaternion.Euler (0, zonePostion, 0);
		EnemyController e = GetEnemy (type);
		e.Reset ();
		e.SetDifficulty (diff);
		e.transform.position = angle*dir;
		e.LauchEnemy ();
		zoneIndex++;
		if (zoneIndex >= AMN_ZONES)
			zoneIndex = 0;
	}
	EnemyController GetEnemy(EnemyType type){
		EnemyController e = enemiesPool.Find (x => !x.isAlive && x.type == type);
		if (e == null) {
			e = (Instantiate (enemyPretabs [(int)type], Vector3.zero, Quaternion.identity)as GameObject).GetComponent<EnemyController>();
			e.type = type;
			enemiesPool.Add (e);
		}
		return e;
	}
	public bool isAnyEnemyAlive(){
		return ((enemiesPool.Find (x => x.isAlive)) == null);
	}
}
