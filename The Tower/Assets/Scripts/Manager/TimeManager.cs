using UnityEngine;
using System.Collections;

public static  class TimeManager  {

	public static float timer;


	public static float TimeScale;
	public static void UpdateTime(){
		timer += DeltaTime;
	}
	public static float DeltaTime{
		get{ return Time.deltaTime * TimeScale;}
	}
}
