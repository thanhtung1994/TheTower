using UnityEngine;
using System.Collections;

public class HubObject : MonoBehaviour {
	public Transform waypoint;
	public GameObject menu;

	CanvasGroup cGroup;
	float transition;
	bool inTransition;
	bool isPoping;

	void Start () {
		cGroup = menu.GetComponent<CanvasGroup> ();
		cGroup.alpha = 0;
		cGroup.interactable = false;
		cGroup.blocksRaycasts = false;
	}
	

	void Update () {
		if (transition < 0 || transition > 1)
			inTransition = false;
		if (!inTransition)
			return;
		transition += (isPoping) ? Time.deltaTime : -Time.deltaTime;
		cGroup.alpha = transition;
	}
	public void FadeMenu(bool show){
		if (show)
			OnShow ();
		isPoping = show;
		cGroup.interactable = show;
		cGroup.blocksRaycasts = show;
		inTransition = true;
		transition = Mathf.Clamp (transition, 0, 1);
	}
	public virtual void OnShow(){
		
	}
}
