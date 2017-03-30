using UnityEngine;
using System.Collections;
public enum Currency{
	Gold=0,
	MagicBrick,
	Diamond,
}
public enum Stat{
	Damage=0,
	Range,
	Speed,
	HitPoint,
	Regen,
	CritChange,
	CritDamage,
	Luck
}
public class StatsHelper : MonoBehaviour {
	#region Stat Const
	const float BASE_DAMAGE=1;
	const float MAX_DAMAGE = 20;

	const float BASE_RANGE=7.5f;//7.5m in radius
	const float GAIN_RANGE=0.25f;
	const float MAX_RANGE = 10;

	const float BASE_SPEED=1.5f; //1.5 attack fer second
	const float GAIN_SPEED=0.025f;
	const float MAX_SPEED = 10;

	const float BASE_HITPOINT=10;// max hit point
	const float MAX_HITPOINT = 30;

	const float BASE_REGEN=1;//1 hitpoint every 5 second
	const float MAX_REGEN = 10;

	const float BASE_CRITCHACE=0.01f;// 1/100 crit
	const float GAIN_CRITCHACE=0.01f;
	const float MAX_CRITCHACE = 5;

	const float BASE_CRITDAMAGE=50;//+50% of damage
	const float GAIN_CRITDAMAGE=5;
	const float MAX_CRITDAMAGE = 5;

	const float BASE_LUCK=0.25f;//25% chance 
	const float GAIN_LUCK=0.0015f;
	const float MAX_LUCK = 20;
	#endregion

	public static StatsHelper ins;
	const int MAX_SKILL_LEVEL=50;
	const int LEVEL_VALUES_SCALE = 50;
	const int PRICE_VALUES_SCALE = 50;
	const int MAX_WAVE_LEVEL=100;
	const int WAVE_VALUES_SCALE = 50;

	int[] priceValueArray = new int[PRICE_VALUES_SCALE];
	int[] levelvaluesArray=	new int[MAX_SKILL_LEVEL];
	int[] waveDifficultyArray=new int[MAX_WAVE_LEVEL];

	//Abilites modifier

	// Use this for initialization
	void Awake () {
		ins = this;
		GenerateLevelValuesArray ();
		GeneratePriceValuesArray ();
		GenerateWaveDifficultyArray ();

		Debug.Log (GetTimeStringFromFloat (4000, true));
	}
	void GenerateLevelValuesArray(){
		float points = 0;
		for (int i = 0; i < MAX_SKILL_LEVEL; i++) {
			points += Mathf.Floor ((i+1) * Mathf.Pow (2, i / LEVEL_VALUES_SCALE));
			levelvaluesArray [i] = (int)Mathf.Floor (points / 4);
		}
	}
	void GenerateWaveDifficultyArray(){
		float points = 0;
		for (int i = 0; i < MAX_WAVE_LEVEL; i++) {
			points += Mathf.Floor ((i+1) * Mathf.Pow (2, i / WAVE_VALUES_SCALE));
			waveDifficultyArray [i] = (int)Mathf.Floor (points / 4);
			if (waveDifficultyArray [i] == 0)
				waveDifficultyArray [i] = 1;
		}
	}
	void GeneratePriceValuesArray(){
		float points = 0;
		for (int i = 0; i < MAX_SKILL_LEVEL; i++) {
			points += Mathf.Floor ((i+1) * Mathf.Pow (2, i / PRICE_VALUES_SCALE));
			priceValueArray [i] = (int)Mathf.Floor (points / 4);

		}
	}

	public int GetPrice(int levelIndex){
		return priceValueArray [levelIndex];
	}
	public float  GetStatsValue(Stat stat,int statLevel=-1){
		int level =(statLevel==-1)?TheTower.ins.TowerStats [(int)stat]:statLevel;
		switch (stat) {
		case Stat.Damage:
			float modiferDamage = (TheTower.ins.bootsMorale.AbilityActive ()) ?TheTower.ins.bootsMorale.damageModifer : 0;
			return BASE_DAMAGE + levelvaluesArray [level]+modiferDamage;
		case Stat.HitPoint:
			return BASE_HITPOINT + levelvaluesArray [level];
		case Stat.Regen:
			return BASE_REGEN + levelvaluesArray [level];
		case Stat.CritDamage:
			return BASE_CRITDAMAGE + level * GAIN_CRITDAMAGE;
		case Stat.CritChange:
			return BASE_CRITCHACE + level * GAIN_CRITCHACE;
		case Stat.Luck:
			return BASE_LUCK + level * GAIN_LUCK;
		case Stat.Speed:
			float modiferSpeed = (TheTower.ins.bootsMorale.AbilityActive ()) ?TheTower.ins.bootsMorale.attackSpeedModifer : 0;
			return (BASE_SPEED + level * -GAIN_SPEED)-modiferSpeed;
		case Stat.Range:
			return BASE_RANGE + level * GAIN_RANGE;
		}
		return -1f;
	}
	public float  GetStatsMaxValue(Stat stat){
		switch (stat) {
		case Stat.Damage:
			return MAX_DAMAGE;
		case Stat.HitPoint:
			return MAX_HITPOINT;
		case Stat.Regen:
			return MAX_REGEN;
		case Stat.CritDamage:
			return MAX_CRITDAMAGE;
		case Stat.CritChange:
			return MAX_CRITCHACE;
		case Stat.Luck:
			return MAX_LUCK;
		case Stat.Speed:
			return MAX_SPEED;
		case Stat.Range:
			return MAX_RANGE;
		}
		return -1f;
	}
	public int GetWaveDifficultyValue(int waveIndex){
		if (waveIndex >= MAX_WAVE_LEVEL)
			return waveDifficultyArray [MAX_WAVE_LEVEL - 1];
		
		return waveDifficultyArray [waveIndex];
	}
	public string GetTimeStringFromFloat(float time,bool includeSecond){
		string r = "";
		float remain = time;
		//hours
		r+=((int)remain/3600).ToString()+'h';
		remain -= ((int)remain / 3600) * 3600;
		//minutes
		r+=((int)remain/60).ToString("00")+'m';
		//secondes
		if (includeSecond)
			r += (time % 60).ToString ("00") + 's';
		return r;
	}
	public string ToReadAble(int value){
		string s = value.ToString ();
		if (value > 999999) {
			s = s.Insert (s.Length - 6, ".");
			s = s.Remove (s.Length - 4);
			s += 'm';
		} else if (value > 9999) {
			s = s.Remove (s.Length - 3);
			s+='k';
		}
		return s;

	}
}
