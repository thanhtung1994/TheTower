using UnityEngine;
using System.Collections;

public class BaseAbility : MonoBehaviour {

	public Ability ability;
	public Currency currency;
	public int cost;

	public float cooldown=300;
	public float duration = 30;
	public float CoolDownLeft{get{return Mathf.Abs (TheTower.ins.totalplayTime - (lastCast + cooldown));}}
	public float DurationLeft{get{return Mathf.Abs (TheTower.ins.totalplayTime - (lastCast + duration));}}
	float lastCast;
	public void Init(){
		lastCast = TheTower.ins.Abilities [(int)ability];
	}
	public void Cast(){
		if (!ChargeCost ())
			return;
		lastCast = TheTower.ins.totalplayTime;
		TheTower.ins.Abilities [(int)ability] = lastCast;
		Action ();
	}
	public bool ChargeCost(){
		//charge the player money
		if(cost<=TheTower.ins.Currencies[(int)currency]){
			TheTower.ins.Currencies [(int)currency] -= cost;
			if (GameUI.ins != null)
				GameUI.ins.UpdateCurrenciesText ();
			return true;
		}
		PopupManager.ins.ShowPopUp ("Too poor!!!", "You  don't have good");
		return false;
	}
	protected virtual  void Action(){
		Debug.Log ("Action is not implement in" + this.ToString ());
	}
	public bool AbilityActive(){
		if (lastCast==0)
			return false;
		return (lastCast + duration >= TheTower.ins.totalplayTime);
	}
	public bool AbilityReady(){
		if (lastCast == 0)
			return true;
		return TheTower.ins.totalplayTime - lastCast >= cooldown;
	}
}
