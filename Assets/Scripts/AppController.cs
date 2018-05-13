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
	[SerializeField] private ScoreCalculator scoreCalculator;
	[SerializeField] private GameObject welcomeUI;
	[SerializeField] private GameObject selectDocumentUI;
	[SerializeField] private GameObject placeObjectUI;
	[SerializeField] private GameObject statistics;
    [SerializeField] private GameObject podiumHint;
    [SerializeField] private GameObject presentationHint;
    [SerializeField] private GameObject scrollableUI;
    [SerializeField] private TapToPlaceParent podium;
	[SerializeField] private TapToPlaceParent projection;
	[SerializeField] private TapToPlaceParent audience;
	private AudienceController audienceController;

	// Use this for initialization
	void Start () {
		ShowWelcome();
		recognizer = new GestureRecognizer();
		recognizer.StartCapturingGestures();
		audienceController = audience.GetComponent<AudienceController>();

        scrollableUI.SetActive(false);
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
		scoreCalculator.StartCalculation(10);
		SpeechRecognition.Instance.StartSpeech();
	}

    private void projection_OnObjectPlaced(object sender, EventArgs e)
    {
        projection.onObjectPlaced -= projection_OnObjectPlaced;
		audience.recognizer = this.recognizer;
		audience.gameObject.SetActive(true);
		audience.SetPlace(true);
        presentationHint.SetActive(false);
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
        podiumHint.SetActive(false);
        scrollableUI.SetActive(true);
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