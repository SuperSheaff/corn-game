using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

	[Header("Scene Loading")]
	[SerializeField] private int sceneIndexToLoad = 1;

	[Header("Mic options")]
	[SerializeField] private TMP_Dropdown micListDropdown;

	private void Start() {
		RefreshMicList();
	}

	public void RefreshMicList() {
		micListDropdown.ClearOptions();
		micListDropdown.AddOptions(Microphone.devices.ToList());
	}

	public void OnDropdownChange(int index) {
		MicRecorderUnity.MicOption = index;
	}

	public void Play() {
		SceneManager.LoadScene(sceneIndexToLoad);
	}
}
