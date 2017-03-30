using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ResearchContainer : MonoBehaviour {
	const float REFRESH_RATE = 1;
	public Research research;
	public Image imgProgress;
	public GameObject buttonContainer;
	ReseachInfo info;
	Text[] texts;
	float lastRefresh;
	bool isResearchAlive;

	// Use this for initialization
	void Start () {
		info = ReseachManager.ins.GetReseachInfo (research);
		texts = GetComponentsInChildren<Text> ();
		GetComponentInChildren<Button> ().onClick.AddListener (StartResearch);
		UpdateContainer ();

	}
	void Update(){
		if (isResearchAlive &&TimeManager.timer - lastRefresh > REFRESH_RATE) {
			UpdateContainer ();
		}
	}
	void UpdateContainer(){
		float startTime = TheTower.ins.Researchs [(int)research];
		//texts
		//0 descriptiontext
		//1 durationtext
		//2 gold cost 
		//3 loot cost
		//4 button message
		//5 progress 
		if (startTime == 0) {
			//research has not been started yet
			//show the research cost in the button
			Transform cost1 = buttonContainer.transform.GetChild (0).GetChild(0);
			Transform cost2 = buttonContainer.transform.GetChild (0).GetChild(1);

			buttonContainer.transform.GetChild(0).gameObject.SetActive(true);
			buttonContainer.transform.GetChild (1).gameObject.SetActive (false);

			//cost1.GetComponentInChildren<Image>().sprite=
			cost1.GetComponentInChildren<Text>().text="x"+info.goldCost;
			//cost1.GetComponentInChildren<Image>().sprite=
			cost2.GetComponentInChildren<Text>().text="x"+info.lootCost;
			if (info.lootCost == 0) {
				buttonContainer.transform.GetChild (0).GetChild (1).gameObject.SetActive (false);
			}

			texts[4].text="Start";
			texts [1].text = StatsHelper.ins.GetTimeStringFromFloat(info.researchTime,true);
			texts [1].color = Color.black;
			texts [5].text = "0%";
			imgProgress.rectTransform.localScale = new Vector3(0,1,1);

		} else if (TheTower.ins.totalplayTime - startTime<info.researchTime) {
			//Is current in progress
			buttonContainer.transform.GetChild(0).gameObject.SetActive(false);
			buttonContainer.transform.GetChild (1).gameObject.SetActive (true);

			GetComponentInChildren<Button> ().interactable = false;
			float ratio = TheTower.ins.totalplayTime - startTime;
			ratio = ratio / info.researchTime;
			texts[4].text="In progress";
			texts [1].text = StatsHelper.ins.GetTimeStringFromFloat(info.researchTime-(info.researchTime*ratio),true);
			texts [1].color = Color.blue;
			texts [5].text = (ratio*100).ToString("0")+"%";
			isResearchAlive = true;
			imgProgress.rectTransform.localScale = new Vector3(ratio,1,1);
		}else{
			//is finish
			buttonContainer.transform.GetChild(0).gameObject.SetActive(false);
			GetComponentInChildren<Button> ().interactable = false;
			texts[4].text="Completed";
			texts [1].text = "0";
			texts [1].color = Color.black;
			texts [5].text = "100%";
			if (isResearchAlive) {
				ReseachManager.ins.UpdateReseachBitArray ();
				isResearchAlive = false;
			}
			imgProgress.rectTransform.localScale = new Vector3(1,1,1);
		}
		lastRefresh = TimeManager.timer;
	}
	void StartResearch(){
		if (info.goldCost <= TheTower.ins.Currencies [(int)Currency.Gold]) {
			if (info.lootCost <= TheTower.ins.Loots [(int)info.loot]) {
				TheTower.ins.Currencies [(int)Currency.Gold] -= info.goldCost;
				TheTower.ins.Loots [(int)info.loot] -= info.lootCost;

				TheTower.ins.Researchs [(int)research] = TheTower.ins.totalplayTime;
				UpdateContainer ();
				if (GameUI.ins != null) {
					GameUI.ins.UpdateCurrenciesText ();
					GameUI.ins.UpdateLootContainer ((int)info.loot);
				}
				return;
			}
			PopupManager.ins.ShowPopUp ("Too poor!!!", "You  don't have loot");
			return;
		}
		//Not Enough loot pop-up
		PopupManager.ins.ShowPopUp ("Too poor!!!", "You  don't have good");
	}
}
