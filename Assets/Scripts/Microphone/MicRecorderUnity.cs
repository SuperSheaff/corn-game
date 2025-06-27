using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

	private bool isVoice;

	public bool IsVoice {
		get { return isVoice; }
		private set {
			if (isVoice.Equals(value)) return;
			isVoice = value;
			IsVoiceChangeEvent.Invoke(isVoice);
		}
	}

	public UnityEvent<bool> IsVoiceChangeEvent = new UnityEvent<bool>();

	public bool IsAvailable { get; private set; }

	public bool aboveMin = false;
	private float averageData = 0;
	private float[] data;
	[SerializeField] private AudioSource source;

	private List<AudioClip> recordedClips = new List<AudioClip>();

	private List<Transform> cubeVisuals = new List<Transform>();

	private void Start() {
		minDataAvergageThreshold = 0.02f;
		//RecordingDeviceIndex = MicOption;
		ChooseMicrophone(RecordingDeviceIndex);
		Debug.Log($"Microphone set to: {RecordingDeviceIndex}: {RecordingDeviceName}", gameObject);

		data = new float[dataSize];

		for (int i = 0; i < dataSize; i++) {
			cubeVisuals.Add(GameObject.CreatePrimitive(PrimitiveType.Cube).transform);
			cubeVisuals[i].position = new Vector3(i, 0, 0);
		}
	}

	public void ChooseMicrophone(int micIndex) {
		if (Microphone.devices.Length == 0) {
			Debug.Log($"No Microphones detected", gameObject);
			return;
		}

		if (micIndex >= Microphone.devices.Length) {
			Debug.Log($"Mic index out of range: {micIndex} | length: {Microphone.devices.Length}", gameObject);
			return;
		}

		RecordingDeviceName = Microphone.devices[micIndex];

		IsAvailable = true;
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

		if (source.isPlaying) {
			SampleData();
			for (int i = 0; i < data.Length; i++) {
				cubeVisuals[i].localScale = new Vector3(1, data[i] * 1000, 1);
			}
		}


		IsVoice = source.isPlaying && averageData > minDataAvergageThreshold;
	}

	private void SampleData() {
		source.GetOutputData(data, 0);

		//Debug.Log($"Sampling data:", gameObject);

		float dataSum = 0;
		for (int i = 0; i < data.Length; i++) {
			dataSum += Mathf.Abs(data[i]);
		}

		averageData = dataSum / data.Length;

		aboveMin = averageData > minDataAvergageThreshold;
	}

	public void StartRecording() {
		source.clip = Microphone.Start(RecordingDeviceName, true, 10, 44100);

		//source.clip = newClip;
		source.Play();
	}


	public void StopRecording() {
		source.Stop();
	}
}
