using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class StatsContainer : MonoBehaviour {
	public Stat stat;
	float maxLevel;
	Text[] texts;
	// Use this for initialization
	void Start () {
		texts = GetComponentsInChildren<Text> ();
		GetComponentInChildren<Button> ().onClick.AddListener (LevelUp);
		maxLevel = StatsHelper.ins.GetStatsMaxValue (stat);
		UpdateText ();

	}
	
	void UpdateText(){
		int statLevel = TheTower.ins.TowerStats [(int)stat];

		texts [0].text = stat.ToString ();//title
		if (statLevel < maxLevel) {
			texts [1].text = StatsHelper.ins.GetPrice (statLevel) + "gold\n LevelUp";
		} else {
			GetComponentInChildren<Button> ().interactable = false;
			texts [1].text="Max";
		}
		texts[2].text="Lv:"+statLevel;
		texts [3].text = StatsHelper.ins.GetStatsValue (stat).ToString ();;
	}
	void LevelUp(){
		int price = StatsHelper.ins.GetPrice (TheTower.ins.TowerStats [(int)stat]);
		if (TheTower.ins.Currencies [(int)Currency.Gold] >= price) {
			TheTower.ins.Currencies [(int)Currency.Gold] -= price;
			TheTower.ins.TowerStats [(int)stat]++;
			GameUI.ins.UpdateCurrenciesText ();
			UpdateText ();
			if (stat == Stat.HitPoint || stat == Stat.Damage) {
				TheTower.ins.RescaleTower ();
				GameUI.ins.UpdateHealthBar ();
			}
		} else {
			PopupManager.ins.ShowPopUp ("Too poor!!!", "You  don't have money");
		}
	}
}
