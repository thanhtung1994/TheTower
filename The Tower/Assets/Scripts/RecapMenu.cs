using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class RecapMenu : MonoBehaviour {
	public void ToMenu(){
		SceneManager.LoadScene ("Hub");
	}
}
