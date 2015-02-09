using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GuiMainMeun : MonoBehaviour
{
		public Rigidbody myCar;
		public float myCarEngineForce;
		public bool isCarIdle = true;
		public string lvl2Load = "FIX_ME";
		public Animator[] myMainMenuAnimators;
		public Canvas[] myMainCanvas;				//Main = 1 //Options = 2 //RaceSetup = 3 //Extras = 4
		public Text[] myOptionsText;				//sense = 1 //Volume = 2 //Grapx = 3	 //ControlText = 4			
		public Slider[] mySliders;					//sense = 1 //Volume = 2 //Grapx = 3
		public static int mySensitivity = 5;
		public static float myVolume = 0.5f;
		public string[] names;
		public int currControls = 1;
		// Use this for initialization
		void Start ()
		{
				//get our quality names
				names = QualitySettings.names;
				//set the graphxQ slider length to be that of out Q levels
				mySliders [2].maxValue = (names.Length - 1);
				//load our sensetivity setting then update the menu
				mySliders [0].value = PlayerPrefs.GetInt ("mySensitivity");
				OptionsUpdateSensetivity ();
				//load our voulume setting then update the menu
				mySliders [1].value = PlayerPrefs.GetInt ("myVolume");
				OptionsUpdateVolume ();
				//load our Quality setting then update the menu
				QualitySettings.SetQualityLevel (PlayerPrefs.GetInt ("qualityLevel"));
				mySliders [2].value = PlayerPrefs.GetInt ("qualityLevel");
				OptionsUpdateGraphics ();
				//load our Control Settings
				currControls = PlayerPrefs.GetInt ("ControlType");
				currControls = currControls - 1;
				OptionsUpdateControlType ();
				//disable all canvas then enable the menu canvas
				foreach (Canvas tempCan in myMainCanvas) {
						tempCan.enabled = false;
				}
				myMainCanvas [0].enabled = true;
				//set our gameVolume to the right volume
				AudioListener.volume = myVolume;
				//start our animations
				StartCoroutine(carEngineIdle());
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
				myOptionsText [1].text = (myOptionsText [1].name + " : " + Mathf.RoundToInt (mySliders [1].value).ToString ());
				float tmpFloat = mySliders [1].value;
				tmpFloat = tmpFloat / 10f;
				tmpFloat = tmpFloat / 10f;
				myVolume = tmpFloat;
				AudioListener.volume = myVolume;
		}

		public void OptionsUpdateGraphics ()
		{
				int tmpInt = Mathf.RoundToInt (mySliders [2].value);

				myOptionsText [2].text = (myOptionsText [2].name + " : " + names [tmpInt]);
		}

		public void OptionsUpdateControlType ()
		{
				currControls++;
				if (currControls < 0)
						currControls = 1;
				if (currControls > 3)
						currControls = 1;
				if (currControls == 1) {
						myOptionsText [3].text = "Gamepad";
						PlayerMove.myControlType = PlayerMove.controlType.KeyboardGamepad;
				}
				if (currControls == 2) {
						myOptionsText [3].text = "VR FIX_ME";
						PlayerMove.myControlType = PlayerMove.controlType.VirtualReality;
				}
				if (currControls == 3) {
						myOptionsText [3].text = "Acclerometer";
						PlayerMove.myControlType = PlayerMove.controlType.Mobile;
				}
		}

		public void SaveAndReturnOptions ()
		{
				//save our settings and update our quality setting
				PlayerPrefs.SetInt ("mySensitivity", Mathf.RoundToInt (mySliders [0].value));
				PlayerPrefs.SetInt ("myVolume", Mathf.RoundToInt (mySliders [1].value));
				QualitySettings.SetQualityLevel (Mathf.RoundToInt (mySliders [2].value));
				PlayerPrefs.SetInt ("qualityLevel", Mathf.RoundToInt (mySliders [2].value));
				PlayerPrefs.SetInt ("ControlType", currControls);
		}

		public void CloseMainMenu ()
		{
				foreach (Animator tempAnimat in myMainMenuAnimators) {
						Button tmpButton = tempAnimat.gameObject.GetComponent<Button> ();
						tmpButton.interactable = false;
						tempAnimat.SetTrigger ("CloseMainMenu");
				}
		}
		IEnumerator carEngineIdle()
		{
		do {
						Random.Range(myCarEngineForce - 50, myCarEngineForce + 100);
						Vector3 tempVec = new Vector3 (myCarEngineForce, 0, 0);
						yield return new WaitForSeconds (0.1f);
						myCar.AddRelativeTorque (tempVec);
						yield return new WaitForSeconds (0.1f);
						myCar.AddRelativeTorque (-tempVec);
				} while(isCarIdle);
		}
	
		// Update is called once per frame
		void Update ()
		{
		}
}
