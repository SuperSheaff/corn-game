using FMODUnity;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MicRecorderUnity : MonoBehaviour {
	//public variables
	[Header("Choose A Microphone")]
	public int RecordingDeviceIndex = 0;
	[TextArea] public string RecordingDeviceName = null;
	[Header("Choose A Key To Play/Pause/Add Reverb To Recording")]
	public KeyCode holdToRecord;
	public KeyCode replayRecord;

	private int dataSize = 128;
	private float[] data;
	private AudioSource source;

	private List<Transform> cubeVisuals = new List<Transform>();

	private void Start() {
		ChooseMicrophone(RecordingDeviceIndex);
		Debug.Log($"Microphone set to default settings: {RecordingDeviceIndex}: {RecordingDeviceName}", gameObject);

		for (int i = 0; i < dataSize; i++) {
			cubeVisuals.Add(GameObject.CreatePrimitive(PrimitiveType.Cube).transform);
			cubeVisuals[i].position = new Vector3(i, 0, 0);
		}
	}

	public void ChooseMicrophone(int micIndex) {
		data = new float[dataSize];

		RecordingDeviceName = Microphone.devices[micIndex];

		source = gameObject.AddComponent<AudioSource>();
		
		source.clip = Microphone.Start(RecordingDeviceName, true, 10, 44100);

		source.Play();
	}

	private void SetupMicrophone() {
	}

	private void Update() {
		if (Input.GetKeyDown(holdToRecord)) {
			StartRecording();
		} else if (Input.GetKeyUp(holdToRecord)) {
			StopRecording();
		}

		//source.GetOutputData(data, 1);
		source.GetOutputData(data, 0);

		for (int i = 0; i < data.Length; i++) {
			cubeVisuals[i].localScale = new Vector3(1, data[i] * 1000, 1);
		}

	}

	private void StartRecording() {
	}


	private void StopRecording() {
	}
}
