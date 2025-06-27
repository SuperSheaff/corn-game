using System.Collections.Generic;
using UnityEngine;

public class MicRecorderUnity : MonoBehaviour {

	public static int MicOption;

	//public variables
	[Header("Choose A Microphone")]
	public int RecordingDeviceIndex = 0;
	[TextArea] public string RecordingDeviceName = null;
	[Header("Choose A Key To Play/Pause/Add Reverb To Recording")]
	public KeyCode holdToRecord;
	public KeyCode replayRecord;

	private int dataSize = 128;

	[SerializeField] private float minDataAvergageThreshold;
	public bool aboveMin = false;
	private float averageData = 0;
	private float[] data;
	[SerializeField] private AudioSource source;

	private List<AudioClip> recordedClips = new List<AudioClip>();

	private List<Transform> cubeVisuals = new List<Transform>();

	private void Start() {
		ChooseMicrophone(RecordingDeviceIndex);
		Debug.Log($"Microphone set to: {RecordingDeviceIndex}: {RecordingDeviceName}", gameObject);


		for (int i = 0; i < dataSize; i++) {
			cubeVisuals.Add(GameObject.CreatePrimitive(PrimitiveType.Cube).transform);
			cubeVisuals[i].position = new Vector3(i, 0, 0);
		}
	}

	public void ChooseMicrophone(int micIndex) {
		RecordingDeviceName = Microphone.devices[micIndex];
	}

	private void Update() {
		if (Input.GetKeyDown(holdToRecord)) {
			StartRecording();
		} else if (Input.GetKeyUp(holdToRecord)) {
			StopRecording();
		}

		if (Input.GetKeyDown(replayRecord)) {
			source.Play();
		}

		SampleData();

		for (int i = 0; i < data.Length; i++) {
			cubeVisuals[i].localScale = new Vector3(1, data[i] * 1000, 1);
		}
	}

	private void SampleData() {
		data = new float[dataSize];
		source.GetOutputData(data, 0);

		float dataSum = 0;
		for (int i = 0; i < data.Length; i++) {
			dataSum += Mathf.Abs(data[i]);
		}

		averageData = dataSum / data.Length;

		aboveMin = averageData > minDataAvergageThreshold;
	}

	public void StartRecording() {
		AudioClip newClip = Microphone.Start(RecordingDeviceName, true, 10, 44100);

		source.clip = newClip;
		source.Play();
	}


	public void StopRecording() {
		source.Stop();
	}
}
