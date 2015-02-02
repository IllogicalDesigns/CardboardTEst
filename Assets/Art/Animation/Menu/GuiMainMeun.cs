using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GuiMainMeun : MonoBehaviour
{
		public string lvl2Load = "FIX_ME";
		public Animator[] myMainMenuAnimators;
		public Canvas[] myMainCanvas;				//Main = 1 //Options = 2 //RaceSetup = 3 //Extras = 4
		public Text[] myOptionsText;
		public Slider[] mySliders;
		public static int mySensitivity = 5;
		public static float myVolume = 0.5f;
		public static int myGraphics = 5;
		public string[] names;
		// Use this for initialization
		void Start ()
		{
				QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("qualityLevel"));
		mySliders[2].value = PlayerPrefs.GetInt("qualityLevel");
				foreach (Canvas tempCan in myMainCanvas) {
						tempCan.enabled = false;
				}
				myMainCanvas [0].enabled = true;
				names = QualitySettings.names;
				mySliders [2].maxValue = (names.Length - 1);
		}

		public void LoadLevel ()
		{
				Application.LoadLevel (lvl2Load);
				Debug.Log (lvl2Load);
		}

		public void QuitGame ()
		{
				Application.Quit ();
		}

		public void SwitchToOptions ()
		{
				foreach (Canvas tempCan in myMainCanvas) {
						tempCan.enabled = false;
				}
				myMainCanvas [1].enabled = true;
		}

		public void SwitchToMainMeun ()
		{
				foreach (Canvas tempCan in myMainCanvas) {
						tempCan.enabled = false;
				}
				myMainCanvas [0].enabled = true;
		}

		public void OptionsUpdateSensetivity ()
		{
				int tmpInt = Mathf.RoundToInt (mySliders [0].value);
				myOptionsText [0].text = (myOptionsText [0].name + " : " + Mathf.RoundToInt (mySliders [0].value).ToString ());
				mySensitivity = tmpInt;
		}

		public void OptionsUpdateVolume ()
		{
				int tmpInt = Mathf.RoundToInt (mySliders [1].value);
				myOptionsText [1].text = (myOptionsText [1].name + " : " + Mathf.RoundToInt (mySliders [1].value).ToString ());
				//myVolume = (Mathf.RoundToInt (mySliders [1].value) );
		}

		public void OptionsUpdateGraphics ()
		{
				int tmpInt = Mathf.RoundToInt (mySliders [2].value);
				if (tmpInt < 0)
						tmpInt = 0;

				myOptionsText [2].text = (myOptionsText [2].name + " : " + names [tmpInt]);
		}
		public void SaveAndReturnOptions ()
		{
		QualitySettings.SetQualityLevel (Mathf.RoundToInt(mySliders [2].value));
		PlayerPrefs.SetInt ("qualityLevel", Mathf.RoundToInt(mySliders [2].value));
		}

		public void CloseMainMenu ()
		{
				foreach (Animator tempAnimat in myMainMenuAnimators) {
						Button tmpButton = tempAnimat.gameObject.GetComponent<Button> ();
						tmpButton.interactable = false;
						tempAnimat.SetTrigger ("CloseMainMenu");
				}
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
}
