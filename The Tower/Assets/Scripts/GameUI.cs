using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GameUI : MonoBehaviour {
	public static GameUI ins;
	public GameObject[] panels;
	public Button[]buttons;
	public Text[] currenciesText;

	//HealthBar fields
	public GameObject hitPointBar;
	public Text hitpointText;
	public Color32 hitpointHealth;
	public Color32 hitpointDieing;
	Image hitpointImage;
	RectTransform hitpointTranform;

	//Current wave & Enemies left to spawn
	public Text currentWaveText;
	public Text enemiesLeftToSpawnText;

	//Loot
	public GameObject lootContainer;
	LootsContainer[] lootContainers;
	void Start(){
		ins = this;
		UpdateCurrenciesText ();
		hitpointTranform = hitPointBar.GetComponent<RectTransform> ();
		hitpointImage = hitPointBar.GetComponent<Image> ();
		UpdateHealthBar ();
		lootContainers = new LootsContainer[lootContainer.transform.childCount];
		for (int i = 0; i < lootContainer.transform.childCount; i++) {
			Transform t = lootContainer.transform.GetChild (i);
			lootContainers [i] = t.GetComponent<LootsContainer> ();
			t.gameObject.SetActive (false);
		}
		LootsManager.ins.UnLookRoot (0);
		LootsManager.ins.UnLookRoot (1);
		LootsManager.ins.UnLookRoot (2);
		NavigateTo (0);
	}
	public void NavigateTo(int menuIndex){
		for (int i = 0; i < panels.Length; i++) {
			if (i == menuIndex) {
				panels [i].SetActive (true);
				buttons [i].Select ();
			} else {
				panels [i].SetActive (false);
			}
				
		}
	}
	public void UpdateCurrenciesText(){
		currenciesText [0].text =StatsHelper.ins.ToReadAble(TheTower.ins.Currencies [(int)Currency.Diamond]);
		currenciesText [1].text =StatsHelper.ins.ToReadAble(TheTower.ins.Currencies [(int)Currency.MagicBrick]);
		currenciesText [2].text =StatsHelper.ins.ToReadAble(TheTower.ins.Currencies [(int)Currency.Gold]);
	}
	public void UpdateHealthBar(){
		float currHP = TheTower.ins.hitpoint;
		float maxHP = StatsHelper.ins.GetStatsValue (Stat.HitPoint);
		float ration = currHP / maxHP;
		if (currHP > 0) {
			hitpointImage.color = Color.Lerp (hitpointHealth, hitpointDieing, ration);
			hitpointText.text = currHP.ToString ("0") + " / " + maxHP.ToString ();
			hitpointTranform.localScale = new Vector3 (ration, 1, 1);
		} else {
			hitpointText.text = "Dead";
			hitpointTranform.localScale = new Vector3 (0, 1, 1);
		}
	}
	public void UpdateCurrentWaveText(int currentWave){
		currentWaveText.text = currentWave.ToString ();
	}
	public void UpdateEnemiesLeftToSpawn(int amn){
		enemiesLeftToSpawnText.text = amn.ToString ();
	}
	public void UpdateLootContainer(int lootIndex){
		lootContainers [lootIndex].UpdateText ();
	}
}
