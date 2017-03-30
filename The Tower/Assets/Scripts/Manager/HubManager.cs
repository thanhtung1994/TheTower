using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class HubManager : MonoBehaviour {
	public static HubManager ins;

	public Button backBtn;
	//Camera
	public Transform defaultCameraWayPoint;
	Transform camTransform;
	Vector3 desiredPosition;
	Quaternion desiredRotation;
	HubObject currenthubObject;
	bool transitionFrame;
	// Use this for initialization
	void Start () {
		backBtn.interactable = false;
		camTransform = Camera.main.transform;
		SetDesiredWayPoint (defaultCameraWayPoint);
		TheTower.ins.SaveClound ();
		TheTower.ins.IsInTheGame (false);
	}
	
	// Update is called once per frame
	void Update () {
		MoveCamera ();
		if (Input.GetMouseButtonUp (0)&&!transitionFrame) {
			RaycastHit hit;
			if(currenthubObject==null)
				if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 75f, LayerMask.GetMask ("HubObject"))) {
					currenthubObject = hit.transform.GetComponent<HubObject> ();
					currenthubObject.FadeMenu (true);
					SetDesiredWayPoint (currenthubObject.waypoint);
					backBtn.interactable = true;
				}

		}
		transitionFrame = false;

	}
	void MoveCamera(){
		camTransform.position = Vector3.Lerp (camTransform.position, desiredPosition, Time.deltaTime);
		camTransform.rotation = Quaternion.Lerp (camTransform.rotation, desiredRotation, Time.deltaTime);
	}
	void SetDesiredWayPoint(Transform waypoint){
		desiredPosition = waypoint.position;
		desiredRotation = waypoint.rotation;
	}
	public void DropMenu(){
		if (currenthubObject == null)
			return;
		currenthubObject.FadeMenu (false);
		currenthubObject = null;
		backBtn.interactable = false;
		transitionFrame = true;
		SetDesiredWayPoint (defaultCameraWayPoint);
	}
	public void StartMissiton(string d){
		Difficulty diff;
		switch (d) {
		case "Easy":
			diff = Difficulty.Easy;
			break;
		case "Medium":
			diff = Difficulty.Medium;
			break;
		case "Hard":
			diff = Difficulty.Hard;
			break;
		case "Insane":
			diff = Difficulty.Insane;
			break;
		default:
			Debug.Log ("wrong difficulty input");
			diff = Difficulty.Easy;
			break;
				}
		TheTower.ins.ChoseDifficulty = diff;
		SceneManager.LoadScene ("Game");
	}
	public void Cheat(){
		for (int i = 0; i < TheTower.ins.Currencies.Length; i++) {
			TheTower.ins.Currencies [i] = 9999;
		}
		for (int i = 0; i < TheTower.ins.Loots.Length; i++) {
			TheTower.ins.Loots [i] = 999;
		}
	}
}
