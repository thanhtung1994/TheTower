using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TowerStatContainer : MonoBehaviour {

	public Stat stat;
	public Text levelText;
	public Text goldCostText;
	public Text panelGoldText;
	public Image statIcon;
	public Button levelUpButton;
	void Start(){
		UpdateContainer ();
		levelUpButton.onClick.AddListener (LevelUp);
	}
	void UpdateContainer(){
		panelGoldText.text =StatsHelper.ins.ToReadAble(TheTower.ins.Currencies [(int)Currency.Gold]);
		int level = TheTower.ins.TowerStats [(int)stat];
		bool maxed = level >= StatsHelper.ins.GetStatsMaxValue (stat);
		levelUpButton.interactable = !maxed;
		levelText.text = "Lv:" + level.ToString();
		levelText.color = (maxed) ? Color.green : Color.black;
		goldCostText.text = (maxed) ? "Maxed" : StatsHelper.ins.GetPrice (level).ToString();
	}
	void LevelUp(){
		int price = StatsHelper.ins.GetPrice (TheTower.ins.TowerStats [(int)stat]);
		if (TheTower.ins.Currencies [(int)Currency.Gold] >= price) {
			TheTower.ins.Currencies [(int)Currency.Gold] -= price;
			TheTower.ins.TowerStats [(int)stat]++;
			UpdateContainer ();
			if (stat == Stat.HitPoint || stat == Stat.Damage) {
				TheTower.ins.RescaleTower ();
			}
		} else {
			Debug.Log ("You don't have money");
		}
	}
}
