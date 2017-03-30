using UnityEngine;
using System.Collections;
public enum Research{
	Tradesman=0,

}
[System.Serializable]
public struct ReseachInfo{
	public Research reseach;
	public int goldCost;
	public int lootCost;
	public Loot loot;
	public float researchTime;
}

public class ReseachManager : MonoBehaviour {
	public static ReseachManager ins;
	public ReseachInfo[] reseachInfos;
	BitArray unlockedReseach;

	// Use this for initialization
	void Awake () {
		ins = this;
	}
	
	public void UpdateReseachBitArray(){
		unlockedReseach = new BitArray (TheTower.ins.Researchs.Length);
		for (int i = 0; i < unlockedReseach.Length; i++) {
			if (TheTower.ins.Researchs [i] == 0) {
				unlockedReseach.Set (i, false);
			}
			else {
				if (TheTower.ins.totalplayTime - TheTower.ins.Researchs [i] > reseachInfos [i].researchTime) {
					unlockedReseach.Set (i, true);
				} else {
					unlockedReseach.Set (i, false);
				}
			}
		}
	}
	public ReseachInfo GetReseachInfo(Research r){
		return reseachInfos [(int)r];
	}
	public bool IsResearchUnLocked(Research r){
		return unlockedReseach.Get ((int)r);
	}
}
