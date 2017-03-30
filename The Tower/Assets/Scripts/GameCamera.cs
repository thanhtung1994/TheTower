using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {
	const float SCREEN_SIDE_BUTTONS=50.0f;
	Vector3 offset=new Vector3(0,10,20);
	float currentX=0;
	float sensivity=180f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (0)) {
			Vector3 moupos = Input.mousePosition;
			if (moupos.x < SCREEN_SIDE_BUTTONS)
				currentX -= Time.deltaTime * sensivity;
			else if (moupos.x > Screen.width - SCREEN_SIDE_BUTTONS)
				currentX += Time.deltaTime * sensivity;
		}
		Vector3 dir = offset;
		Quaternion rotation = Quaternion.Euler (0, currentX, 0);
		transform.position = rotation * dir;
		transform.LookAt (Vector3.up * (TheTower.ins.GetTowerHeight () / 2));
	}
}
