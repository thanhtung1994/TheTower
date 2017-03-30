using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour {

	//Fade Animation
	public GameObject uiRoot;
	public GameObject resetSaveMenu;
	CanvasGroup uiRootGroup;
	float fadeTrasition;
	bool menuAvaiable;
	bool fadeInTrasition;
	bool menuPoping;
	float idleTimeToFade=5;
	float lastTouchTime;
	//Camera Field
	Transform camTransform;
	Vector3 offset;
	float transition;
	float cameraSpeed=20.5f;
	// Use this for initialization
	void Start () {
		
		if(!Social.localUser.authenticated){
			Social.localUser.Authenticate ((bool success) => {
				if(success){
					TheTower.ins.LoadClound();
				}else{
					PopupManager.ins.ShowPopUp("Cloud error","Unable to connect to the clound, loading lastest game session ");
					TheTower.ins.LoadLocal();
				}
			}
			);
		}
		camTransform = Camera.main.transform;
		 CalculatorCameraOffset ();

		uiRootGroup = uiRoot.GetComponent<CanvasGroup> ();
		uiRootGroup.alpha = 0;
		uiRootGroup.interactable = false;
	}
	
	// Update is called once per frame
	void Update () {
		MoveCamera ();
		UpdateFade ();
		if (Input.GetMouseButtonDown (0))
			OnTouch ();
	}
	void OnTouch(){
		if (!TheTower.ins.saveLoaded)
			return;
		lastTouchTime = Time.time;
		uiRootGroup.interactable = true;
		fadeInTrasition = true;
		menuAvaiable = true;
		menuPoping = true;
		fadeTrasition = Mathf.Clamp (fadeTrasition, 0, 1);
	}
	void UpdateFade(){
		if (Time.time - lastTouchTime > idleTimeToFade) {
			fadeInTrasition = true;
			menuPoping = false;
			menuAvaiable = false;
			uiRootGroup.interactable = false;
		}
		if (!fadeInTrasition)
			return;
		fadeTrasition += menuPoping ? Time.deltaTime : -Time.deltaTime*0.25f;
		uiRootGroup.alpha = fadeTrasition;
		if (fadeTrasition > 1 || fadeTrasition < 0)
			fadeInTrasition = false;
	}
	public void CalculatorCameraOffset(){
		Vector3 r = Vector3.zero;
		r.z =r.y+ TheTower.ins.GetTowerWidth ()+3;
		r.y=TheTower.ins.GetTowerHeight()/2;
		offset= r;
	}
	void MoveCamera(){
		float y = Mathf.Sin (Time.time)*0.25f;
		transition += Time.deltaTime * cameraSpeed;
		Vector3 desiredPos = offset+(Vector3.up*y);
		Quaternion orientation = Quaternion.Euler (0, transition, 0);
		camTransform.position = orientation * desiredPos;
		camTransform.LookAt (Vector3.up * camTransform.position.y);
	}
	public void ToGame(){
		if(menuAvaiable)
		SceneManager.LoadScene ("Hub");
	}
	public void AchievementButton(){
		Debug.Log ("Achievement");
	}
	public void LeaderBoardButton(){
		Debug.Log ("LeaderBoard");
	}
	public void SwapSaveButton(){
		Debug.Log ("SwapSave");
	}
	public void ResetSave(){
		TheTower.ins.saveLoaded=false;
		TheTower.ins.ResetSave();
		resetSaveMenu.SetActive (false);
	}
	public void ResetSaveButton(){
		resetSaveMenu.SetActive (!resetSaveMenu.activeSelf);

	}

}
