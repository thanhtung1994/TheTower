using UnityEngine;
using System.Collections;
using System;
public enum Loot{
	Rook=0,
	Log,
	Silk,
	Plank,
	Brick,
	IronIngot,
	GoldIngot,
	Saphhire,
	Emerald,
	Ruby,
	Cat,
	Fox,
	Unicorn,
}
public enum LootPrice{
	Rook=50,
	Log=75,
	Silk=100,
	Plank=225,
	Brick=300,
	IronIngot=500,
	GoldIngot=1000,
	Saphhire=2000,
	Emerald=5000,
	Ruby=8000,
	Cat=30000,
	Fox=40000,
	Unicorn=100000,
}
public class LootsManager : MonoBehaviour {
	
	public static LootsManager ins;
	public BitArray unlockedLoots;
	public GameObject lootContainer;
	// Use this for initialization
	void Awake () {
		ins = this;
		unlockedLoots = new BitArray (Enum.GetNames (typeof(Loot)).Length);
	}
	
	public int  GetLootPricePerUnit(Loot loot){
		int price = 0;
		//Get the price
		switch (loot) {
		case Loot.Rook:
			price = (int)LootPrice.Rook;
			break;
		case Loot.Log:
			price = (int)LootPrice.Log;
			break;
		case Loot.Silk:
			price = (int)LootPrice.Silk;
			break;
		case Loot.Plank:
			price = (int)LootPrice.Plank;
			break;
		case Loot.Brick:
			price = (int)LootPrice.Brick;
			break;
		case Loot.IronIngot:
			price = (int)LootPrice.IronIngot;
			break;
		case Loot.GoldIngot:
			price = (int)LootPrice.GoldIngot;
			break;
		case Loot.Saphhire:
			price = (int)LootPrice.Saphhire;
			break;
		case Loot.Emerald:
			price =(int) LootPrice.Emerald;
			break;
		case Loot.Ruby:
			price = (int)LootPrice.Ruby;
			break;
		case Loot.Cat:
			price = (int)LootPrice.Cat;
			break;
		case Loot.Fox:
			price = (int)LootPrice.Fox;
			break;
		case Loot.Unicorn:
			price = (int)LootPrice.Unicorn;
			break;
		}
		if (ReseachManager.ins.IsResearchUnLocked (Research.Tradesman))
			price += (int)(price * 0.1f);
		return price;
	}
	public int GetUnlockLootAmount(){
		int amn = 0;
		foreach (bool  b in unlockedLoots) {
			if (b)
				amn++;
		}
		return amn;
	}
	public void UnLookRoot(int lootindex){
		
		unlockedLoots.Set (lootindex, true);

		lootContainer.transform.GetChild (lootindex).gameObject.SetActive (true);
	}
	public void DropItem(){
		int amn = GetUnlockLootAmount ();
		int lootIndex = UnityEngine.Random.Range (0, amn);
		TheTower.ins.Loots [lootIndex]++;
		GameUI.ins.UpdateLootContainer (lootIndex);
		CombatTextManager.ins.Show (new Sprite (), (TheTower.ins.GetTowerHeight() + 0.5f) * Vector3.up, Vector3.up, 1.25f);
	}

}
