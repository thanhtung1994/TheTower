using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum Difficulty
{
	Easy = 1,
	Medium = 2,
	Hard = 4,
	Insane = 7,
}

public class GameManager : MonoBehaviour
{
	const float ADDTIONAL_ENEMY_LEVELDELTA = 2.5f;
	const float DELAY_IN_BETWEEN_SPAWN = 0.5f;
	const float TIME_IN_BETWEEN_WAVE = 5;
	const int GOLD_ON_LOSS_PER_WAVE = 100;
	public static GameManager ins;
	public int currentWave;
	public int startingWave;
	public Difficulty currentDifficulty;
	public GameObject recapMenu;
	public GameObject optionMenu;
	public GameObject timerPanel;
	List<SpawnInformation> toSpawn = new List<SpawnInformation> ();
	float lastWaveTime;
	float lastWaveTimeCompleted;
	bool waveActive;
	bool isAlive = true;
	// Use this for initialization
	void Start ()
	{
		ins = this;
		ModTimeScale (1);
		recapMenu.SetActive (false);
		startingWave =TheTower.ins.StartingLevel;
		currentWave = startingWave;
		currentDifficulty = TheTower.ins.ChoseDifficulty;
		GenerateSpawnList (currentWave);
		GameUI.ins.UpdateCurrentWaveText (currentWave);
		for (int i = 0; i < (int)currentWave/5; i++) {
			LootsManager.ins.UnLookRoot (i + 3);
		}
		TheTower.ins.IsInTheGame (true);
	}

	void Update ()
	{
		if (!isAlive)
			return;
		if (waveActive) {
			int c = toSpawn.Count;
			if (c > 0) {
				SpawnInformation si = toSpawn [0];
				if (TimeManager.timer - lastWaveTime > si.time) {
					SpawnEnemyManager.ins.SpawnEnemy (si.type, si.diff);
					toSpawn.Remove (si);

					GameUI.ins.UpdateEnemiesLeftToSpawn (c - 1);
				}
			} else {
				//if there any enemies left on the battleground
				if (SpawnEnemyManager.ins.isAnyEnemyAlive ()) {
					waveActive = false;
					lastWaveTimeCompleted = TimeManager.timer;
				}
			} 
		} else {
			if (TimeManager.timer- lastWaveTimeCompleted > TIME_IN_BETWEEN_WAVE) {
				currentWave++;
				GenerateSpawnList (currentWave);
				if (currentWave % 5==0) {
					int lootIndex = (int)(currentWave / 5);
					LootsManager.ins.UnLookRoot (lootIndex + 3);
				}
			}
		}
			
	}

	void GenerateSpawnList (int waveLevel)
	{
		lastWaveTime = TimeManager.timer;
		toSpawn.Clear ();
		float delay = 2f;
		int amnToSpawn = 10 + (int)(waveLevel / ADDTIONAL_ENEMY_LEVELDELTA);
		for (int i = 0; i < amnToSpawn; i++) {
			SpawnInformation s = new SpawnInformation ();
			s.time = delay += DELAY_IN_BETWEEN_SPAWN + (waveLevel % 5) * -0.1f;
			s.type = EnemyType.Tiny;
			s.diff = currentDifficulty;
			if (Random.value <= 0.05)
				s.diff += 1;
			toSpawn.Add (s);
		}
		waveActive = true;
		GameUI.ins.UpdateEnemiesLeftToSpawn (amnToSpawn);
		GameUI.ins.UpdateCurrentWaveText (waveLevel);
		RewardOnDefeat (currentDifficulty);
	}

	void DoneSpawnWave ()
	{
		Debug.Log ("We don't spawn enemy");
		currentWave++;
		if (currentWave > TheTower.ins.HightestWaveCompleted)
			TheTower.ins.HightestWaveCompleted = currentWave;
		GenerateSpawnList (currentWave);


	}

	public void OnTowerDeath ()
	{
		foreach (Transform item in timerPanel.transform) {
			item.GetComponent<Button> ().interactable = false;
		}
		ModTimeScale (0);
		if (isAlive) {
			isAlive = false;
			recapMenu.SetActive (true);
		}
		optionMenu.SetActive (false);
	}

	public void OnOption ()
	{
		if (!isAlive)
			return;
		int startinggoldreward=(startingWave- 1) * 100 * (int)currentDifficulty;
		int goldReward = ((currentWave - 1) * 100 * (int)currentDifficulty)-startinggoldreward;
		optionMenu.SetActive (!optionMenu.activeSelf);
		optionMenu.GetComponentInChildren<Text> ().text = "Leaving now will reward you:" + goldReward + "gold";
	}
	public void ModTimeScale(float amount){
		TimeManager.TimeScale = amount;
	}
	public void ToHub ()
	{
		int startinggoldreward=(startingWave- 1) * 100 * (int)currentDifficulty;
		int goldReward = ((currentWave - 1) * 100 * (int)currentDifficulty)-startinggoldreward;
		TheTower.ins.Currencies [(int)Currency.Gold] += goldReward;
		ModTimeScale (1);
		TheTower.ins.StartingLevel = currentWave;
		SceneManager.LoadScene ("Hub");
	}
	public void RewardOnDefeat(Difficulty diff){
		int amn = currentWave;
		amn *=GOLD_ON_LOSS_PER_WAVE;
		amn *= (int)diff;

		TheTower.ins.Currencies [(int)Currency.Gold] += amn;
		GameUI.ins.UpdateCurrenciesText ();
	}
}
