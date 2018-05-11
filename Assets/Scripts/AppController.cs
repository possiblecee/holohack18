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
	[SerializeField] private GameObject statistics;
	[SerializeField] private TapToPlaceParent podium;
	[SerializeField] private TapToPlaceParent projection;
	[SerializeField] private TapToPlaceParent audience;

	// Use this for initialization
	void Start () {
		ShowWelcome();
		recognizer = new GestureRecognizer();
		recognizer.StartCapturingGestures();
	}

	void OnEnable()
	{
		podium.onObjectPlaced += podium_onObjectPlaced;
		projection.onObjectPlaced += projection_OnObjectPlaced;
		audience.onObjectPlaced += audience_OnObjectPlaced;
	}

    private void audience_OnObjectPlaced(object sender, EventArgs e)
    {
        audience.onObjectPlaced -= audience_OnObjectPlaced;
		OnPressentationStarted();
    }

	// After all object placed, this method will be executed
	public void OnPressentationStarted() {
		// Use this to start the pressentation
	}

    private void projection_OnObjectPlaced(object sender, EventArgs e)
    {
        projection.onObjectPlaced -= projection_OnObjectPlaced;
		audience.recognizer = this.recognizer;
		audience.gameObject.SetActive(true);
		audience.SetPlace(true);
    }

    void OnDisable()
	{
		podium.onObjectPlaced -= podium_onObjectPlaced;
		projection.onObjectPlaced -= projection_OnObjectPlaced;
		audience.onObjectPlaced -= audience_OnObjectPlaced;
	}

    private void podium_onObjectPlaced(object sender, EventArgs e)
    {
        podium.onObjectPlaced -= podium_onObjectPlaced;
		projection.gameObject.SetActive(true);
		projection.SetPlace(true);
		projection.recognizer = recognizer;
    }

    // Update is called once per frame
    void Update () {
	}

	public void RestartApplication() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	// If the pressentation ended, call this method
	public void EndPresentation(){
		statistics.SetActive(true);
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