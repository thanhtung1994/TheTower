using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum Ability{
	BootsMorale=0,
}
public class AbilityManager : MonoBehaviour {
	public static  AbilityManager ins; 
	public List<BaseAbility>  Abilities=new List<BaseAbility>();
	// Use this for initialization
	void Awake () {
		ins = this;
		foreach (BaseAbility b in GetComponents<BaseAbility>()) {
			Abilities.Add (b);
		}
	}
	public void Init(){
		foreach (BaseAbility b in Abilities) {
			b.Init ();
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
