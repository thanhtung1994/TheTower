using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class AbilityContainer : MonoBehaviour {
	const float REFRESH_RATE=1;
	public Ability ability;
	BaseAbility baseAbility;
	Button button;
	Image currentImg;
	Text[] texts;
	float lastRefesh;
	// Use this for initialization
	void Start () {
		baseAbility = AbilityManager.ins.Abilities.Find (x => x.ability == ability);
		texts = GetComponentsInChildren<Text> ();
		button = GetComponentInChildren<Button> ();
		button.onClick.AddListener (OnCast);
		currentImg = GetComponentInChildren<Image> ();
		UpdateContainer ();
	}
	void Update(){
		if (TimeManager.timer- lastRefesh > REFRESH_RATE) {
			UpdateContainer ();
		}
	}
	void UpdateContainer(){
		//text
		//0 title
		//1 description
		//2 cost amount
		//3 button message
		//4 duration
		bool ready=baseAbility.AbilityReady();
		button.interactable = ready;

		if (ready) {
			if (baseAbility.cost == 0) {
				button.transform.GetChild (0).gameObject.SetActive (false);
				button.transform.GetChild (1).gameObject.SetActive (true);
				texts [3].text = "Use!";
			} else {
				button.transform.GetChild (0).gameObject.SetActive (true);
				button.transform.GetChild (1).gameObject.SetActive (false);
				texts [2].text = "x" + baseAbility.cost.ToString ();
			}

			texts [4].text = StatsHelper.ins.GetTimeStringFromFloat (baseAbility.cooldown,true);
			texts [4].color = Color.black;
		} else {
			button.transform.GetChild (0).gameObject.SetActive (false);
			button.transform.GetChild (1).gameObject.SetActive (true);
			if (baseAbility.AbilityActive()) {
				texts [3].text = "Active!";
				texts [4].text = StatsHelper.ins.GetTimeStringFromFloat (baseAbility.DurationLeft, true);
				texts [4].color = Color.red;
			} else {
				texts [3].text = "On cooldown...";
				texts [4].text = StatsHelper.ins.GetTimeStringFromFloat (baseAbility.CoolDownLeft, true);
				texts [4].color = Color.blue;
			}
		}
		lastRefesh=TimeManager.timer;

	}
	void OnCast(){
		baseAbility.Cast ();
		UpdateContainer ();
	}
}
