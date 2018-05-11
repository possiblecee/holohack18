using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppController : Singleton<AppController> {

	private AppState applicationState;
	[SerializeField] private GameObject welcomeUI;
	[SerializeField] private GameObject selectDocumentUI;
	[SerializeField] private GameObject podium;
	[SerializeField] private GameObject projection;
	[SerializeField] private GameObject audience;

	// Use this for initialization
	void Start () {
		ShowWelcome();
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