using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class LootsContainer : MonoBehaviour {

	public Loot loot;
	float maxLevel;
	Text[] texts;
	// Use this for initialization
	void Start () {
		texts = GetComponentsInChildren<Text> ();
		GetComponentInChildren<Button> ().onClick.AddListener (Sell);
		UpdateText ();

	}

	public void UpdateText(){
		if (texts == null)
			return;
		int amn = TheTower.ins.Loots [(int)loot];
		//0 amount
		//1 title
		//2 BtnSell
		//3 Sell for amount
		texts[0].text="x"+amn.ToString();
		texts [1].text = loot.ToString ();
		texts [3].text =StatsHelper.ins.ToReadAble((amn * LootsManager.ins.GetLootPricePerUnit (loot)));
	}
	void Sell(){
		int amn = TheTower.ins.Loots [(int)loot];
		int price = LootsManager.ins.GetLootPricePerUnit (loot);
		TheTower.ins.Loots [(int)loot] = 0;
		TheTower.ins.Currencies [(int)Currency.Gold] +=amn* price;
		UpdateText ();
		GameUI.ins.UpdateCurrenciesText ();
	}
}
