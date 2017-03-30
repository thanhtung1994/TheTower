using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class CombatText{
	public bool isActive;
	public GameObject go;
	public Text txt;
	public Image lootImg;
	public Vector3 motion;
	public float duration=1;
	public float lastShow;
	public void Hide(){
		isActive = false;
		go.SetActive (false);
	}
	public void Show(){
		isActive = true;
		lastShow = Time.time;
		go.SetActive (true);
	}
	public void UpdateCombatText(){
		if (!isActive)
			return;
		if (Time.time - lastShow > duration)
			Hide ();
		go.transform.position += motion * Time.deltaTime;
		go.transform.rotation = Quaternion.LookRotation (go.transform.position-Camera.main.transform.position);
	}
}
public class CombatTextManager : MonoBehaviour {
	public static CombatTextManager ins;
	public GameObject combatTextContainer;
	public GameObject combatTextPertabs;

	List<CombatText> combatTexts=new List<CombatText>();
	// Use this for initialization
	void Start () {
		ins = this;
	}
	void Update(){
		foreach (CombatText c in combatTexts)
			c.UpdateCombatText ();
	}
	public void Show(string text,int fontsize, Color color,Vector3 pos,Vector3 motion,float duration){
		CombatText cmb = GetCombatText ();
		cmb.txt.text = text;
		cmb.txt.fontSize = fontsize;
		cmb.txt.color = color;
		//swap 
		cmb.txt.gameObject.SetActive(true);
		cmb.lootImg.gameObject.SetActive(false);


		cmb.go.transform.position = pos;
		cmb.motion = motion;

		cmb.duration = duration;
		cmb.Show ();
		Debug.Log ("Show projective");
	}
	public void Show(Sprite img,Vector3 pos,Vector3 motion,float duration){
		CombatText cmb = GetCombatText ();


		//swap 
		cmb.txt.gameObject.SetActive(false);
		cmb.lootImg.gameObject.SetActive(true);
		cmb.lootImg.sprite = img;

		cmb.go.transform.position = pos;
		cmb.motion = motion;

		cmb.duration = duration;
		cmb.Show ();
	}
	CombatText GetCombatText(){
		CombatText cmb = combatTexts.Find (c => !c.isActive);
		if (cmb == null) {
			cmb = new CombatText ();
			cmb.go = Instantiate (combatTextPertabs);
			cmb.go.transform.SetParent (combatTextContainer.transform);
			cmb.txt = cmb.go.transform.GetChild(0).GetComponent<Text> ();
			cmb.lootImg = cmb.go.transform.GetChild (1).GetComponent<Image> ();
			combatTexts.Add (cmb);
		}
		return cmb;
	}
}
