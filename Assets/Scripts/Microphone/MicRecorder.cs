using FMODUnity;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class MicRecorder : MonoBehaviour {
	//public variables
	[Header("Choose A Microphone")]
	public int RecordingDeviceIndex = 0;
	[TextArea] public string RecordingDeviceName = null;
	[Header("Choose A Key To Play/Pause/Add Reverb To Recording")]
	public KeyCode holdToRecord;
	public KeyCode replayRecord;

	//FMOD Objects
	private FMOD.Sound sound;
	private FMOD.CREATESOUNDEXINFO exinfo;
	private FMOD.Channel channel;
	private FMOD.ChannelGroup channelGroup;

	//How many recording devices are plugged in for us to use.
	private int numOfDriversConnected = 0;

	//Info about the device we're recording with.
	private int SampleRate = 0;
	private int NumOfChannels = 0;

	[SerializeField] private List<FMOD.Sound> storedSounds = new List<FMOD.Sound>();

	private void Start() {
		ChooseMicrophone(RecordingDeviceIndex);
		Debug.Log($"Microphone set to default settings: {RecordingDeviceIndex}: {RecordingDeviceName}", gameObject);
	}

	public void ChooseMicrophone(int micIndex) {
		//Step 1: Check to see if any recording devices (or drivers) are plugged in and available for us to use.
		RuntimeManager.CoreSystem.getRecordNumDrivers(out _, out numOfDriversConnected);

		if (numOfDriversConnected == 0) {
			Debug.Log("Hey! Plug a Microhpone in ya dummy!!!");
			return;
		} else {
			Debug.Log("You have " + numOfDriversConnected + " microphones available to record with.");
		}

		if (micIndex >= numOfDriversConnected) {
			Debug.Log($"Chosen index: {micIndex} is greater than the number of drivers connected", gameObject);
			return;
		}

		RecordingDeviceIndex = micIndex;

		SetupMicrophone();
	}

	private void SetupMicrophone() {
		//Step 2: Get all of the information we can about the recording device (or driver) that we're
		//        going to use to record with.
		RuntimeManager.CoreSystem.getRecordDriverInfo(RecordingDeviceIndex, out RecordingDeviceName, 50,
			out _, out SampleRate, out _, out NumOfChannels, out _);


		//Next we want to create an "FMOD Sound Object", but to do that, we first need to use our 
		//FMOD.CREATESOUNDEXINFO variable to hold and pass information such as the sample rate we're
		//recording at and the num of channels we're recording with into our Sound object.


		//Step 3: Store relevant information into FMOD.CREATESOUNDEXINFO variable.

		exinfo.cbsize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(FMOD.CREATESOUNDEXINFO));
		exinfo.numchannels = NumOfChannels;
		exinfo.format = FMOD.SOUND_FORMAT.PCM16;
		exinfo.defaultfrequency = SampleRate;
		exinfo.length = (uint)SampleRate * sizeof(short) * (uint)NumOfChannels * 10;
	}

	private void Update() {
		if (Input.GetKeyDown(holdToRecord)) {
			StartRecording();
		} else if (Input.GetKeyUp(holdToRecord)) {
			StopRecording();
		}

		if (Input.GetKeyDown(replayRecord)) {
			if (storedSounds.Count == 0) {
				//return;
			}

			FMOD.Sound latestSound = storedSounds[storedSounds.Count - 1];
			Debug.Log($"Playing. totalSounds: {storedSounds.Count}", gameObject);
			RuntimeManager.CoreSystem.playSound(latestSound, channelGroup, false, out channel);

			uint length;
			latestSound.getLength(out length, FMOD.TIMEUNIT.RAWBYTES);

			byte[] buffer = new byte[(int)length];
			uint read;
			latestSound.seekData(10);
			latestSound.readData(buffer, out read);

			Debug.Log($"read length: {read}", gameObject);

			for (int i  = 0; i < 100; i++) {

				//Debug.Log($"buffer: {buffer[i]}", gameObject);
				
			}
		}

		bool channelPlaying;
		FMOD.RESULT result = channel.isPlaying(out channelPlaying);

		if (result == FMOD.RESULT.OK && channelPlaying) {
			float audibility = 0;
			channel.getAudibility(out audibility);
			Debug.Log($"volume: {audibility}", gameObject);

		}
	}

	private void StartRecording() {
		Debug.Log($"Recording", gameObject);

		storedSounds.Add(CreateSound());

		//Step 5: Start recording through our chosen device into our Sound object.
		RuntimeManager.CoreSystem.recordStart(RecordingDeviceIndex, storedSounds[storedSounds.Count - 1], true);
	}

	private FMOD.Sound CreateSound() {
		FMOD.Sound clip;
		//Step 4: Create an FMOD Sound "object". This is what will hold our voice as it is recorded.
		RuntimeManager.CoreSystem.createSound(exinfo.userdata, FMOD.MODE.DEFAULT | FMOD.MODE.OPENUSER, 
			ref exinfo, out clip);

		return clip;
	}

	private void StopRecording() {
		Debug.Log($"Not Recording", gameObject);
		RuntimeManager.CoreSystem.recordStop(RecordingDeviceIndex);
	}

}
