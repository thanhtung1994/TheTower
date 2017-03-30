using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupManager : MonoBehaviour {

	public static PopupManager ins;
	 CanvasGroup canvas;
	public Text titleText;
	public Text descriptionText;
	Transform uiRoot;
	void Start(){
		ins = this;
		canvas = GetComponent<CanvasGroup> ();
		DontDestroyOnLoad (gameObject);
		canvas.alpha = 0;
		canvas.blocksRaycasts = false;
		canvas.interactable = false;

	}
	public void ShowPopUp(string title,string message){
		if (uiRoot == null)
			uiRoot = GameObject.FindGameObjectWithTag ("UIRoot").transform;
		
		transform.SetParent (uiRoot,true);

		canvas.alpha = 1;
		canvas.interactable = true;
		canvas.blocksRaycasts = true;
		GetComponent<RectTransform> ().sizeDelta = Vector2.zero;
		GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;
		transform.localScale = Vector3.one;
		titleText.text = title;
		descriptionText.text = message;
	}
	public void OnClick(){
		canvas.alpha = 0;
		canvas.blocksRaycasts = false;
		canvas.interactable = false;
		transform.SetParent (uiRoot.parent);
	}
}
