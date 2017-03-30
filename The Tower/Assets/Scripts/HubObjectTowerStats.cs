using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class HubObjectTowerStats : HubObject {
	public Text goldText;
	public override void OnShow ()
	{
		goldText.text = TheTower.ins.Currencies [(int)Currency.Gold].ToString ();
	}

}
