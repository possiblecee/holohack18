using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.WSA.Input;

public class AppController : Singleton<AppController> {

	private AppState applicationState;
	private GestureRecognizer recognizer;
	[SerializeField] private GameObject welcomeUI;
	[SerializeField] private GameObject selectDocumentUI;
	[SerializeField] private PlaceObjectUIController placeObjectUI;
	[SerializeField] private TapToPlaceParent podium;
	[SerializeField] private GameObject projection;
	[SerializeField] private GameObject audience;

	// Use this for initialization
	void Start () {
		ShowWelcome();
		recognizer = new GestureRecognizer();
		recognizer.StartCapturingGestures();
	}

	void OnEnable()
	{
		podium.onObjectPlaced += podium_onObjectPlaced;
	}

	void OnDisable()
	{
		podium.onObjectPlaced -= podium_onObjectPlaced;
	}

    private void podium_onObjectPlaced(object sender, EventArgs e)
    {
        
    }

    // Update is called once per frame
    void Update () {
	}

	public void RestartApplication() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void EndPresentation(){
		
	}

	public void OnPresentationSelected() {
		selectDocumentUI.gameObject.SetActive(false);
		placeObjectUI.gameObject.SetActive(true);
		podium.gameObject.SetActive(true);
		podium.recognizer = this.recognizer;
		podium.SetPlace(true);
	}

	public void OnWelcomeNextClick(){
		welcomeUI.gameObject.SetActive(false);
		selectDocumentUI.gameObject.SetActive(true);
	}

	public void ShowStatistics(){
		
	}

	public void ShowWelcome(){
		welcomeUI.gameObject.SetActive(true);
	}
}

public enum AppState {

	WELCOME,
	SELECT_DOCUMENT,
	PLACE_PODIUM,
	PLACE_PROJECTOR,
	PLACE_AUDIENCE,
	PRESENTATION_RUNNING,
	PRESENTATION_END,
	PRESENTATION_STATISTICS
}