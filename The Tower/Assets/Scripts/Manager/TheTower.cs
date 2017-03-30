using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;

public class TheTower : MonoBehaviour {
	const float BASE_TOWER_HEIGHT = 1.5f;
	const float BASE_TOWER_WIDTH = 1f;
	//range
	const float RANGE_HEIGHT_GAIN=0.05f;
	//hit point
	const float HITPOINT_WIDTH_GAIN=0.025f;
	const float REGEN_STICK = 0.5f;

	public static TheTower ins{ set; get;}
	public BootsMoraleAbility bootsMorale{ set; get;}
	public int StartingLevel{ set; get;}
	public Difficulty ChoseDifficulty{ set; get;}
	public int[] TowerStats{ set; get;}
	public int[] Currencies{ set; get;}
	public int[] Loots{ set; get;}
	public float[] Researchs{ set; get;}
	public float[] Abilities{ set; get;}
	public GameObject projectilePer;
	public Material towerMaterial;

	public float hitpoint{ set; get;}
	public float lastSave{ set; get;}
	public float playtimesinceSave{ set; get;}
	public float totalplayTime{get{ return (TimeManager.timer - lastSave) + playtimesinceSave;}}

	public int HightestWaveCompleted{ set; get;}
	public bool saveLoaded{ set; get;}
	bool isInGame;
	float lastRegen;
	float lastAttack;


	// Use this for initialization
	void Start  () {
		GooglePlayGames.BasicApi.PlayGamesClientConfiguration config = new GooglePlayGames.BasicApi.PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
		PlayGamesPlatform.InitializeInstance (config);
		PlayGamesPlatform.Activate ();
		PlayGamesPlatform.DebugLogEnabled = true;
		if (ins == null) 
			ins = this;
		else 
			Destroy (gameObject);
		DontDestroyOnLoad (gameObject);
		//Create all my array
		StartingLevel=10;
		TowerStats = new int[Enum.GetNames(typeof(Stat)).Length];
		Currencies = new int[Enum.GetNames (typeof(Currency)).Length];
		Loots = new int[Enum.GetNames (typeof(Loot)).Length];
		Researchs = new float[Enum.GetNames (typeof(Research)).Length];
		Abilities = new float[Enum.GetNames (typeof(Ability)).Length];

		bootsMorale = GetComponent<BootsMoraleAbility> ();
		CreateTowerMesh ();
		//LoadLocal ();
		hitpoint = StatsHelper.ins.GetStatsValue (Stat.HitPoint);
		AbilityManager.ins.Init ();

		SceneManager.LoadScene ("Menu");
	}

	void Update(){
		//Update our own timescale
		TimeManager.UpdateTime();
		if (isInGame) {
			RegenerationHitPoint ();
			//Looking for enemies
			if(TimeManager.timer-lastAttack>StatsHelper.ins.GetStatsValue(Stat.Speed)){
				Collider[] col=Physics.OverlapSphere(transform.position,StatsHelper.ins.GetStatsValue(Stat.Range),LayerMask.GetMask("Enemy"));
				if (col.Length != 0) {
					//Find the closest one 
					int closestIndex=0;
					float dist = Vector3.SqrMagnitude (col [closestIndex].transform.position - transform.position);
					for (int i = 0; i < col.Length; i++) {
						float newDistance = Vector3.SqrMagnitude (col [i].transform.position - transform.position);
						if (newDistance < dist) {
							dist = newDistance;
							closestIndex = i;
						}
					}
					col [closestIndex].GetComponent<EnemyController> ().RemoveEnemy ();
					//Shoot closest enemy
					ShootEnemy(col[closestIndex].transform);
					lastAttack = TimeManager.timer;
				}
			}
		}
	}
	public void IsInTheGame(bool b){
		isInGame = b;

	}
	void ShootEnemy(Transform target){
		Vector3 projectileSpawnPoint = target.position.normalized;
		projectileSpawnPoint.y = GetTowerHeight ();
		GameObject a=Instantiate (projectilePer,projectileSpawnPoint, Quaternion.identity)as GameObject;
		Projective p = a.GetComponent<Projective> ();
		//calculate crit damage?
		float dmg=StatsHelper.ins.GetStatsValue(Stat.Damage);

		//bootstMorale modifer

		if (UnityEngine.Random.value <= StatsHelper.ins.GetStatsValue (Stat.CritChange)) {
			//Crit
			dmg += dmg * StatsHelper.ins.GetStatsValue (Stat.CritDamage) / 100;
			p.LauchProjectile (target,dmg,true);
		} else {
			p.LauchProjectile (target,dmg,false);
		}

	}
	public void TakeDamage(float amount){
		hitpoint -= (int)amount;
		GameUI.ins.UpdateHealthBar ();
		if (hitpoint <= 0) {
			GameManager.ins.OnTowerDeath ();
		}
	}


	private	void CreateTowerMesh(){
		float w = 0.5f;
		Vector3[] verts = new Vector3[] {
			new Vector3 (-w, 0, -w),
			new Vector3 (w, 0, -w),
			new Vector3 (w, 0, w),
			new Vector3 (-w, 0, w),
			new Vector3 (-w, w*2, -w),
			new Vector3 (w, w*2, -w),
			new Vector3 (w, w*2, w),
			new Vector3 (-w, w*2, w),
		};
		int[] tris = new int[] {
			0, 4, 1,//font
			4, 5, 1,
			1, 5, 2,//right
			5, 6, 2,
			2, 6, 3,//back
			6, 7, 3,
			3, 7, 0,//left
			7, 4, 0,
			4, 7, 5,//top
			7, 6, 5
		};
		Mesh m = new Mesh ();
		m.vertices = verts;
		m.triangles = tris;
		m.RecalculateBounds ();
		m.RecalculateNormals ();
		gameObject.AddComponent<MeshFilter> ().mesh = m;
		gameObject.AddComponent<MeshRenderer> ().material = towerMaterial;
	}
	public	void RescaleTower(){
		Vector3 newScale = Vector3.zero;
		newScale.x = BASE_TOWER_WIDTH + TowerStats [(int)Stat.HitPoint] * HITPOINT_WIDTH_GAIN;
		newScale.z = newScale.x;
		newScale.y = BASE_TOWER_HEIGHT + TowerStats [(int)Stat.Range] * RANGE_HEIGHT_GAIN;
		transform.localScale = newScale;
	}
	void RegenerationHitPoint(){ //regenation heath 
		if (TimeManager.timer - lastRegen > REGEN_STICK) {
			hitpoint += (StatsHelper.ins.GetStatsValue (Stat.Regen) / 5) * REGEN_STICK;
			if(hitpoint>StatsHelper.ins.GetStatsValue(Stat.HitPoint))
				hitpoint=StatsHelper.ins.GetStatsValue(Stat.HitPoint);

			GameUI.ins.UpdateHealthBar ();
			lastRegen += lastRegen;
				}
	}
	public float GetTowerHeight(){
		return transform.localScale.y;
	}
	public float GetTowerWidth(){
		return transform.localScale.x;
	}
	private bool isSaving;
	private bool hasBeenWarnedLocalSave;

	public void LoadLocal(){
		/*
		playtimesinceSave = PlayerPrefs.GetFloat ("TotalPlayTime");
		HightestWaveCompleted = PlayerPrefs.GetInt ("HightesWaveCompleted", 0);
		TowerStats = new int[Enum.GetNames(typeof(Stat)).Length];
		TowerStats[(int)Stat.Damage]=PlayerPrefs.GetInt("StatDamage",1);
		TowerStats[(int)Stat.HitPoint]=PlayerPrefs.GetInt("StatHitPoint",1);
		TowerStats[(int)Stat.Range]=PlayerPrefs.GetInt("StatRange",5);
		TowerStats[(int)Stat.Speed]=PlayerPrefs.GetInt("StatSpeed",1);
		TowerStats[(int)Stat.Regen]=PlayerPrefs.GetInt("StatRegen",1);
		TowerStats[(int)Stat.CritChange]=PlayerPrefs.GetInt("StatCritChange",1);
		TowerStats[(int)Stat.CritDamage]=PlayerPrefs.GetInt("StatCritDamage",1);
		TowerStats[(int)Stat.Luck]=PlayerPrefs.GetInt("StatLuck",1);

		Currencies = new int[Enum.GetNames (typeof(Currency)).Length];
		Currencies[(int)Currency.Diamond]=PlayerPrefs.GetInt("CurrencyDiamond",1);
		Currencies[(int)Currency.Gold]=PlayerPrefs.GetInt("CurrencyGold",1);
		Currencies[(int)Currency.MagicBrick]=PlayerPrefs.GetInt("CurrencyMagicBrick",1);

		Loots = new int[Enum.GetNames (typeof(Loot)).Length];
		Loots [(int)Loot.Rook] = PlayerPrefs.GetInt ("LootRock", 1);
		Loots [(int)Loot.Log] = PlayerPrefs.GetInt ("LootLog", 1);
		Loots [(int)Loot.Silk] = PlayerPrefs.GetInt ("LootSilk", 1);
		Loots [(int)Loot.Plank] = PlayerPrefs.GetInt ("LootPlank", 1);
		Loots [(int)Loot.Brick] = PlayerPrefs.GetInt ("LootBrick", 1);
		Loots [(int)Loot.IronIngot] = PlayerPrefs.GetInt ("LootIronIngot", 1);
		Loots [(int)Loot.GoldIngot] = PlayerPrefs.GetInt ("LootGoldIngot", 1);
		Loots [(int)Loot.Saphhire] = PlayerPrefs.GetInt ("LootSaphhire", 1);
		Loots [(int)Loot.Emerald] = PlayerPrefs.GetInt ("LootEmerald", 1);
		Loots [(int)Loot.Ruby] = PlayerPrefs.GetInt ("LootRuby", 1);
		Loots [(int)Loot.Cat] = PlayerPrefs.GetInt ("LootCat", 1);
		Loots [(int)Loot.Fox] = PlayerPrefs.GetInt ("LootFox", 1);
		Loots [(int)Loot.Unicorn] = PlayerPrefs.GetInt ("LootUnicorn", 1);

		Researchs = new float[Enum.GetNames (typeof(Research)).Length];
		for (int i = 0; i < Researchs.Length; i++) {
			string r = Enum.GetValues (typeof(Research)).GetValue (i).ToString();
			Researchs [i] = PlayerPrefs.GetFloat (r, 0);
		}

		Abilities = new float[Enum.GetNames (typeof(Ability)).Length];
		for (int i = 0; i <Abilities.Length; i++) {
			string r = Enum.GetValues (typeof(Ability)).GetValue (i).ToString();
			Abilities [i] = PlayerPrefs.GetFloat (r, 0);
		}
		*/
		LoadFromString (PlayerPrefs.GetString ("SaveData", ""));
	}
	public void LoadClound(){
		isSaving = false;
		((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution
		("TheTowerSave",
				GooglePlayGames.BasicApi.DataSource.ReadCacheOrNetwork,
				ConflictResolutionStrategy.UseLongestPlaytime,
				SaveGameOpened);
	}
	public void SaveLocal(){
		/*
		PlayerPrefs.SetFloat ("TotalPlayTime", totalplayTime);
		PlayerPrefs.SetInt ("HightesWaveCompleted", HightestWaveCompleted);
		lastSave = Time.time;
		PlayerPrefs.SetInt ("StatDamage", TowerStats[(int)Stat.Damage]);
		PlayerPrefs.SetInt ("StatHitPoint", TowerStats[(int)Stat.HitPoint]);
		PlayerPrefs.SetInt ("StatRange", TowerStats[(int)Stat.Range]);
		PlayerPrefs.SetInt ("StatSpeed", TowerStats[(int)Stat.Speed]);
		PlayerPrefs.SetInt ("StatRegen", TowerStats[(int)Stat.Regen]);
		PlayerPrefs.SetInt ("StatCritChange", TowerStats[(int)Stat.CritChange]);
		PlayerPrefs.SetInt ("StatCritDamage", TowerStats[(int)Stat.CritDamage]);
		PlayerPrefs.SetInt ("StatLuck", TowerStats[(int)Stat.Luck]);

		PlayerPrefs.SetInt ("CurrencyDiamond", TowerStats[(int)Currency.Diamond]);
		PlayerPrefs.SetInt ("CurrencyGold", TowerStats[(int)Currency.Gold]);
		PlayerPrefs.SetInt ("CurrencyMagicBrick", TowerStats[(int)Currency.MagicBrick]);

		PlayerPrefs.SetInt ("LootRock", Loots [(int)Loot.Rook]);
		PlayerPrefs.SetInt ("LootLog", Loots [(int)Loot.Log]);
		PlayerPrefs.SetInt ("LootSilk",Loots [(int)Loot.Silk]);
		PlayerPrefs.SetInt ("LootPlank", Loots [(int)Loot.Plank]);
		PlayerPrefs.SetInt ("LootBrick", Loots [(int)Loot.Brick]);
		PlayerPrefs.SetInt ("LootIronIngot", Loots [(int)Loot.IronIngot]);
		PlayerPrefs.SetInt ("LootGoldIngot",	Loots [(int)Loot.GoldIngot]);
		PlayerPrefs.SetInt ("LootSaphhire", Loots [(int)Loot.Saphhire]);
		PlayerPrefs.SetInt ("LootEmerald", Loots [(int)Loot.Emerald]);
		PlayerPrefs.SetInt ("LootRuby", Loots [(int)Loot.Ruby] );
		PlayerPrefs.SetInt ("LootCat", Loots [(int)Loot.Cat]);
		PlayerPrefs.SetInt ("LootFox", Loots [(int)Loot.Fox]);
		PlayerPrefs.SetInt ("LootUnicorn", Loots [(int)Loot.Unicorn]);

		for (int i = 0; i < Researchs.Length; i++) {
			string r = Enum.GetValues (typeof(Research)).GetValue (i).ToString();
			PlayerPrefs.SetFloat (r, Researchs [i]);
		}

		for (int i = 0; i < Abilities.Length; i++) {
			string r = Enum.GetValues (typeof(Ability)).GetValue (i).ToString();
			PlayerPrefs.SetFloat (r, Abilities [i]);
		}
		*/
		PlayerPrefs.SetString ("SaveData", GetSaveString());
	}
	public void SaveClound(){
		if (Social.localUser.authenticated) {
			isSaving = true;
			hasBeenWarnedLocalSave = false;
			SaveLocal ();
			((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution
			("TheTowerSave",
				GooglePlayGames.BasicApi.DataSource.ReadCacheOrNetwork,
				ConflictResolutionStrategy.UseLongestPlaytime,
				SaveGameOpened);
		} else {
			if (!hasBeenWarnedLocalSave)
				PopupManager.ins.ShowPopUp ("Cloud error!", "Unable to reach the cloud,saving localy");
			hasBeenWarnedLocalSave = true;
			SaveLocal ();
		}
	}
	public void SaveGameOpened(SavedGameRequestStatus status,ISavedGameMetadata game){
		if (status == SavedGameRequestStatus.Success) {
			if (isSaving) {// Writting data
				byte[] data=System.Text.ASCIIEncoding.ASCII.GetBytes(GetSaveString());
				TimeSpan playedTime = TimeSpan.FromSeconds (totalplayTime);
				SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder ()
					.WithUpdatedPlayedTime (playedTime)
					.WithUpdatedDescription("Saved Game At "+DateTime.Now);
				SavedGameMetadataUpdate update = builder.Build ();
				((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate (game, update, data, SaveGameWritten);
			} else {//Reading Data
				((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game,SaveGameLoaded);
			}
		} else {
			PopupManager.ins.ShowPopUp ("Save State", "Error opening game:" + status);
		}
	}
	public void SaveGameLoaded(SavedGameRequestStatus status,byte[] data){
		if (status == SavedGameRequestStatus.Success) {
			string cloudSave = System.Text.ASCIIEncoding.ASCII.GetString (data);
			string localSave = PlayerPrefs.GetString ("SaveData", "");

			ulong cloudTime,localTime;
			ulong.TryParse (cloudSave.Split('%')[0],out cloudTime);
			ulong.TryParse (localSave.Split('%')[0],out localTime);

			if (cloudTime == 0 || localTime == 0) {
				if (cloudTime != 0)
					LoadFromString (cloudSave);
				else if (localTime != 0)
					LoadFromString (localSave);
				else {
					LoadFromString (GetEmptySaveString ());
				}
				return;
			}else
			LoadFromString ((localTime>cloudTime)?localSave:cloudSave);
		} else {
			Debug.Log ("Error reading game:" + status);
		}
	}
	public void SaveGameWritten(SavedGameRequestStatus status,ISavedGameMetadata game){
		Debug.Log (status);
	}
	private void LoadFromString(string savedData){
		
		if (savedData == "") {
			LoadFromString (GetEmptySaveString ());
			return;
		}
		string[] data = savedData.Split ('|');
		// timestamp and totaltime and highestwave and stardingwave
		string[] misc=data[0].Split('%');
		playtimesinceSave =float.Parse(misc[1]);
		StartingLevel = int.Parse (misc [2]);
		HightestWaveCompleted =int.Parse(misc[3]);
		//stats
		string[] stats=data[1].Split('%');
		TowerStats[(int)Stat.Damage]=int.Parse(stats[0]);
		TowerStats[(int)Stat.HitPoint]=int.Parse(stats[1]);
		TowerStats[(int)Stat.Range]=int.Parse(stats[2]);
		TowerStats[(int)Stat.Speed]=int.Parse(stats[3]);
		TowerStats[(int)Stat.Regen]=int.Parse(stats[4]);
		TowerStats[(int)Stat.CritChange]=int.Parse(stats[5]);
		TowerStats[(int)Stat.CritDamage]=int.Parse(stats[6]);
		TowerStats[(int)Stat.Luck]=int.Parse(stats[7]);
		//currency
		string[] currencies=data[2].Split('%');
		Currencies [(int)Currency.Diamond] = int.Parse (currencies [0]);
		Currencies[(int)Currency.Gold]=int.Parse (currencies [1]);
		Currencies[(int)Currency.MagicBrick]=int.Parse (currencies [2]);

		//loots
		string[] loots=data[3].Split('%');
		Loots [(int)Loot.Rook] =  int.Parse (loots [0]);
		Loots [(int)Loot.Log] =int.Parse (loots [1]);
		Loots [(int)Loot.Silk] = int.Parse (loots [2]);
		Loots [(int)Loot.Plank] = int.Parse (loots [3]);
		Loots [(int)Loot.Brick] =int.Parse (loots [4]);
		Loots [(int)Loot.IronIngot] =int.Parse (loots [5]);
		Loots [(int)Loot.GoldIngot] = int.Parse (loots [6]);
		Loots [(int)Loot.Saphhire] =int.Parse (loots [7]);
		Loots [(int)Loot.Emerald] =int.Parse (loots [8]);
		Loots [(int)Loot.Ruby] = int.Parse (loots [9]);
		Loots [(int)Loot.Cat] = int.Parse (loots [10]);
		Loots [(int)Loot.Fox] =int.Parse (loots [11]);
		Loots [(int)Loot.Unicorn] =int.Parse (loots [12]);
		//Researchs
		string[]researchs=data[4].Split('%');
		Researchs[(int)Research.Tradesman] =float.Parse(researchs[0]);
		ReseachManager.ins.UpdateReseachBitArray ();
		//Abilities
		string[]abilities=data[5].Split('%');
		Abilities [(int)Ability.BootsMorale] =float.Parse(abilities[0]);
		RescaleTower ();
		saveLoaded = true;
	}
	private string GetSaveString(){
		string saveData = "";
		//timestamp&&totaltime && highestwave&&currentWave
		saveData+=
			DateTime.Now.Ticks.ToString()+'%'
			+totalplayTime.ToString()+'%'
			+StartingLevel.ToString()+'%'
			+HightestWaveCompleted.ToString()+'|';
		lastSave =TimeManager.timer;
		playtimesinceSave = totalplayTime;
		//Stats
		saveData += 
		TowerStats [(int)Stat.Damage].ToString () + '%' +
		TowerStats [(int)Stat.HitPoint].ToString () + '%' +
		TowerStats [(int)Stat.Range].ToString () + '%' +
		TowerStats [(int)Stat.Speed].ToString () + '%' +
		TowerStats [(int)Stat.Regen].ToString() + '%' +
		TowerStats [(int)Stat.CritChange].ToString () + '%' +
		TowerStats [(int)Stat.CritDamage].ToString () + '%' +
		TowerStats [(int)Stat.Luck].ToString () + '|';


		//Currencies
		saveData+=
			TowerStats[(int)Currency.Diamond].ToString()+'%'+
			TowerStats[(int)Currency.Gold].ToString()+'%'+
			TowerStats[(int)Currency.MagicBrick].ToString()+'|';
		//Loots
		saveData +=
		Loots [(int)Loot.Rook].ToString () + '%' +
		Loots [(int)Loot.Log].ToString () + '%' +
		Loots [(int)Loot.Silk].ToString () + '%' +
		Loots [(int)Loot.Plank].ToString () + '%' +
		Loots [(int)Loot.Brick].ToString () + '%' +
		Loots [(int)Loot.IronIngot].ToString () + '%' +
		Loots [(int)Loot.GoldIngot].ToString () + '%' +
		Loots [(int)Loot.Saphhire].ToString () + '%' +
		Loots [(int)Loot.Emerald].ToString () + '%' +
		Loots [(int)Loot.Ruby].ToString () + '%' +
		Loots [(int)Loot.Cat].ToString () + '%' +
		Loots [(int)Loot.Fox].ToString () + '%' +
		Loots [(int)Loot.Unicorn].ToString () + '|';

		//Research
		saveData +=
			Researchs [(int)Research.Tradesman].ToString () + '|';

		//Abilities
		saveData+=
			Abilities[(int)Ability.BootsMorale].ToString();
		
		return saveData;
	}
	private string GetEmptySaveString ()
	{
		string savestring = GetSaveString ();
		string newssave = "";
		string[] data = savestring.Split ('|');
		for (int i = 0; i < data.Length; i++) {
			string[] data2 = data [i].Split ('%');
			for (int j = 0; j < data2.Length; j++) {
				if (i == 0 && j == 0) {
					newssave += DateTime.Now.Ticks.ToString ();
					continue;
				}
				if (j == 0)
					newssave += "0";
				else
					newssave += "%0";
			}
			if (i != data.Length - 1)
				newssave += '|';
		}
	
		return newssave;
	}
	public void ResetSave(){
		isSaving = true;
		LoadFromString (GetEmptySaveString ());
		SaveClound ();
	}
	void OnApplicationQuit(){
		SaveClound ();
	}
}