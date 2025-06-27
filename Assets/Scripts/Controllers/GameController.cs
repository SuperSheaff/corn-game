using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System;
using StarterAssets;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField] private Animator jumpScareAnimator;
    [SerializeField] private GameObject player;

    public bool IsTransmitting { get; private set; }
    private bool isReceiving        = false;

    private KeyCode _walkieTalkieButton = KeyCode.Mouse0;

    // FLAG CITY

    public bool WalkieTutorialPassed    = false;
    public bool Dialogue1Started        = false;
    public bool Dialogue1Finished       = false;
    public bool Dialogue2Started        = false;
    public bool Dialogue2Finished       = false;
    public bool Dialogue3Started        = false;
    public bool Dialogue3Finished       = false;
    public bool Dialogue4Started        = false;
    public bool Dialogue4Finished       = false;
    public bool Dialogue5Started        = false;
    public bool Dialogue5Finished       = false;
    public bool Dialogue6Started        = false;
    public bool Dialogue6Finished       = false;
    public bool Dialogue7Started        = false;
    public bool Dialogue7Finished       = false;
    public bool Dialogue8Started        = false;
    public bool Dialogue8Finished       = false;

    public bool MovementEnabled         = false;
    public bool MovementTutorialPassed  = false;
    public bool ScareCrow1Finished      = false;

    private bool waitingForTransmitAfterDialogue = true;
    private float transmitHoldTimer = 0f;
    private bool readyToTriggerNextDialogue = false;
    private Action nextScriptedDialogue;

    private int _scareSelector;

    [SerializeField] private float dialogue4Delay = 5f; // Set in Inspector

    // DIALOGUE & FMOD CITY

    private EventInstance _dialogueInstance1;
    [SerializeField] private EventReference _dialogueEvent1;

    private EventInstance _dialogueInstance2;
    [SerializeField] private EventReference _dialogueEvent2;

    private EventInstance _dialogueInstance3;
    [SerializeField] private EventReference _dialogueEvent3;

    private EventInstance _dialogueInstance4;
    [SerializeField] private EventReference _dialogueEvent4;

    private EventInstance _dialogueInstance5;
    [SerializeField] private EventReference _dialogueEvent5;

    private EventInstance _dialogueInstance6;
    [SerializeField] private EventReference _dialogueEvent6;

    private EventInstance _dialogueInstance7;
    [SerializeField] private EventReference _dialogueEvent7;

    private EventInstance _dialogueInstance8;
    [SerializeField] private EventReference _dialogueEvent8;
    
    // SCENE OBJECT CITY

    public GameObject Scene2;
    public GameObject Scene4;
    public GameObject Scene5;

    public Transform teleportDestination; // assign in Inspector
    public GameObject playerObject;     // assign or find by tag

    void Start()
    {
        SetupVariables();
        //StartCoroutine(DelayedAction(PlayDialogue1, 10f));
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        CheckInput();
        CheckDialogueAndLogic();
        ShowTutorialText();
    }

    public void StartReceiving()
    {
        isReceiving = true;
        UIController.Instance.ShowHand(UIController.HandName.Receiving);
    }

    public void StopReceiving()
    {
        isReceiving = false;
        UIController.Instance.ShowHand(UIController.HandName.Idle);
    }

    public void ShowTutorialText()
    {
        if (!WalkieTutorialPassed)
        {
            UIController.Instance.WalkieTutorialText(true);
        }
        if (Dialogue2Finished && !MovementTutorialPassed)
        {
            UIController.Instance.WalkieTutorialText(false);
            MovementEnabled = true;
            UIController.Instance.MoveTutorialText(true);
        }
        else if (Dialogue2Finished && MovementTutorialPassed)
        {
            MovementEnabled = true;
            UIController.Instance.MoveTutorialText(false);
        }
    }

    // --- CHECK FUNCTIONS ---

    public void CheckInput()
    {
        if (!isReceiving)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                PlayJumpScare();
            }
            if (Input.GetKey(_walkieTalkieButton))
            {
                IsTransmitting = true;

                if (Input.GetKeyDown(_walkieTalkieButton)) UIController.Instance.ShowHand(UIController.HandName.Transmitting);

                if (waitingForTransmitAfterDialogue)
                {
                    transmitHoldTimer += Time.deltaTime;
                    Debug.Log(transmitHoldTimer);

                    if (transmitHoldTimer >= 1f)
                    {
                        readyToTriggerNextDialogue = true;
                    }
                }
            }

            if (Input.GetKeyUp(_walkieTalkieButton))
            {
                if (waitingForTransmitAfterDialogue && readyToTriggerNextDialogue)
                {
                    // Player held transmit long enough, now released
                    Debug.Log("Transmit released after valid hold — starting Dialogue 2 soon");
                    StartCoroutine(DelayedAction(nextScriptedDialogue, 1.5f)); // or whatever delay you want

                    // Clear flags
                    waitingForTransmitAfterDialogue = false;
                    readyToTriggerNextDialogue = false;
                }

                IsTransmitting = false;
                UIController.Instance.ShowHand(UIController.HandName.Idle);
                transmitHoldTimer = 0f;
            }
        }
        else
        {
            IsTransmitting = false;
        }
    }

    public void CheckDialogueAndLogic()
    {
        //Walkie Tutorial
        //if the player has used the walkie for 1 second, enable dialogue 1 to play.
        if (readyToTriggerNextDialogue && !WalkieTutorialPassed)
        {
            WalkieTutorialPassed = true;
            Debug.Log("walkie Tutorial Passed");
            transmitHoldTimer = 0f;
            nextScriptedDialogue = PlayDialogue1;
        }
        // Dialogue 1 Finished
        if (PlaybackState(_dialogueInstance1) != PLAYBACK_STATE.PLAYING && Dialogue1Started && !Dialogue1Finished)
        {
            Debug.Log("Dialogue 1 Finished");
            Dialogue1Finished = true;
            StopReceiving();

            // Wait for the player to transmit
            waitingForTransmitAfterDialogue = true;
            transmitHoldTimer = 0f;
            Debug.Log("Dialogue 1 Finished — waiting for transmit hold...");
            nextScriptedDialogue = PlayDialogue2;
        }

        // Dialogue 2 Finished
        if (PlaybackState(_dialogueInstance2) != PLAYBACK_STATE.PLAYING && Dialogue2Started && !Dialogue2Finished)
        {
            Debug.Log("Dialogue 2 Finished");
            Dialogue2Finished = true;
            player.GetComponent<FirstPersonController>().SetMovementAllowed(true);
            StopReceiving();
        }

        // Dialogue 3 Finished
        if (PlaybackState(_dialogueInstance3) != PLAYBACK_STATE.PLAYING && Dialogue3Started && !Dialogue3Finished)
        {
            Debug.Log("Dialogue 3 Finished");
            Dialogue3Finished = true;
            StopReceiving();
            StartCoroutine(PlayDialogue4AfterDelay(dialogue4Delay));
        }

        // Dialogue 4 Finished
        if (PlaybackState(_dialogueInstance4) != PLAYBACK_STATE.PLAYING && Dialogue4Started && !Dialogue4Finished)
        {
            Debug.Log("Dialogue 4 Finished");
            Dialogue4Finished = true;
            StopReceiving();
        }

        // Dialogue 5 Finished
        if (PlaybackState(_dialogueInstance5) != PLAYBACK_STATE.PLAYING && Dialogue5Started && !Dialogue5Finished)
        {
            Debug.Log("Dialogue 5 Finished");
            Dialogue5Finished = true;
            StopReceiving();
        }

        // Dialogue 6 Finished
        if (PlaybackState(_dialogueInstance6) != PLAYBACK_STATE.PLAYING && Dialogue6Started && !Dialogue6Finished)
        {
            Debug.Log("Dialogue 6 Finished");
            Dialogue6Finished = true;
            StopReceiving();
            PlayDialogue7();
        }

        // Dialogue 7 Finished
        if (PlaybackState(_dialogueInstance7) != PLAYBACK_STATE.PLAYING && Dialogue7Started && !Dialogue7Finished)
        {
            Debug.Log("Dialogue 7 Finished");
            Dialogue7Finished = true;
            StopReceiving();
        }

        // Dialogue 8 Finished
        if (PlaybackState(_dialogueInstance8) != PLAYBACK_STATE.PLAYING && Dialogue8Started && !Dialogue8Finished)
        {
            Debug.Log("Dialogue 8 Finished");
            Dialogue8Finished = true;
            StopReceiving();
        }

        // if (ScareCrow1Finished)
        // {
        //     SetupScene2();
        //     PlayDialogue3();
        // }
    }
    
    // --- SETUP FUNCTIONS ---

    public void SetupVariables()
    {
        // FMOD
        _dialogueInstance1 = RuntimeManager.CreateInstance(_dialogueEvent1);
        _dialogueInstance2 = RuntimeManager.CreateInstance(_dialogueEvent2);
        _dialogueInstance3 = RuntimeManager.CreateInstance(_dialogueEvent3);
        _dialogueInstance4 = RuntimeManager.CreateInstance(_dialogueEvent4);
        _dialogueInstance5 = RuntimeManager.CreateInstance(_dialogueEvent5);
        _dialogueInstance6 = RuntimeManager.CreateInstance(_dialogueEvent6);
        _dialogueInstance7 = RuntimeManager.CreateInstance(_dialogueEvent7);
        _dialogueInstance8 = RuntimeManager.CreateInstance(_dialogueEvent8);

        // Flags
        Dialogue1Started    = false;
        Dialogue1Finished   = false;
        Dialogue2Started    = false;
        Dialogue2Finished   = false;
        Dialogue3Started    = false;
        Dialogue3Finished   = false;
        Dialogue4Started    = false;
        Dialogue4Finished   = false;
        Dialogue5Started    = false;
        Dialogue5Finished   = false;
        Dialogue6Started    = false;
        Dialogue6Finished   = false;
        Dialogue7Started    = false;
        Dialogue7Finished   = false;
        Dialogue8Started    = false;
        Dialogue8Finished   = false;

    }

    public void SetupScene2()
    {
        Scene2.SetActive(true);
        PlayDialogue3();
    }

    public void SetupScene3()
    {
        ScarecrowController.Instance?.SetState(ScarecrowState.Standing);
        PlayDialogue5();
    }

    public void SetupScene4()
    {
        Scene4.SetActive(true);
        PlayDialogue6();
        // start some dialogue @ oscar
    }

    public void SetupScene5()
    {
        TeleportPlayerToGround();
        PlayDialogue8();
        Scene5.SetActive(true);
        Debug.Log("BIG CROW TOIME");
        // start some dialogue @ oscar
    }

    // --- PlAY FUNCTIONS ---

    public void PlayDialogue1()
    {
        StartReceiving();
        _dialogueInstance1.start();
        _dialogueInstance1.release();
        StartCoroutine(DelayedBoolSet(() => Dialogue1Started = true, 0.2f));
    }

    public void PlayDialogue2()
    {
        StartReceiving();
        _dialogueInstance2.start();
        _dialogueInstance2.release();
        StartCoroutine(DelayedBoolSet(() => Dialogue2Started = true, 0.2f));
    }

    public void PlayDialogue3()
    {
        StartReceiving();
        _dialogueInstance3.start();
        _dialogueInstance3.release();
        StartCoroutine(DelayedBoolSet(() => Dialogue3Started = true, 0.2f));
    }

    public void PlayDialogue4()
    {
        StartReceiving();
        _dialogueInstance4.start();
        _dialogueInstance4.release();
        StartCoroutine(DelayedBoolSet(() => Dialogue4Started = true, 0.2f));
    }

    public void PlayDialogue5()
    {
        StartReceiving();
        _dialogueInstance5.start();
        _dialogueInstance5.release();
        StartCoroutine(DelayedBoolSet(() => Dialogue5Started = true, 0.2f));
    }

    public void PlayDialogue6()
    {
        StartReceiving();
        _dialogueInstance6.start();
        _dialogueInstance6.release();
        StartCoroutine(DelayedBoolSet(() => Dialogue6Started = true, 0.2f));
    }

    public void PlayDialogue7()
    {
        StartReceiving();
        _dialogueInstance7.start();
        _dialogueInstance7.release();
        StartCoroutine(DelayedBoolSet(() => Dialogue7Started = true, 0.2f));
    }

    public void PlayDialogue8()
    {
        StartReceiving();
        _dialogueInstance8.start();
        _dialogueInstance8.release();
        StartCoroutine(DelayedBoolSet(() => Dialogue8Started = true, 0.2f));
    }

    public void PlayJumpScare()
    {
        _scareSelector = UnityEngine.Random.Range(1,4);
        if (jumpScareAnimator != null)
        {
            switch (_scareSelector)
            {
                case 1:
                    jumpScareAnimator.SetBool("Select V", true);
                    jumpScareAnimator.SetBool("Select H", false);
                    jumpScareAnimator.SetBool("Select -H", false);
                    break;
                case 2:
                    jumpScareAnimator.SetBool("Select V", false);
                    jumpScareAnimator.SetBool("Select H", true);
                    jumpScareAnimator.SetBool("Select -H", false);
                    break;
                case 3:
                    jumpScareAnimator.SetBool("Select V", false);
                    jumpScareAnimator.SetBool("Select H", false);
                    jumpScareAnimator.SetBool("Select -H", true);
                    break;
            }
            jumpScareAnimator.SetTrigger("PlayJumpScare");
        }
        else
        {
            Debug.LogWarning("Jump scare animator not assigned!");
        }
    }
    
    private IEnumerator PlayDialogue4AfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayDialogue4();
    }

    
    // --- HELPER FUNCTIONS ---

    private IEnumerator DelayedAction(System.Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    private IEnumerator DelayedBoolSet(System.Action setBoolAction, float delay)
    {
        yield return new WaitForSeconds(delay);
        setBoolAction.Invoke();
    }

    PLAYBACK_STATE PlaybackState(EventInstance instance)
    {
        PLAYBACK_STATE pS;
        instance.getPlaybackState(out pS);
        return pS;
    }

    public void TeleportPlayerToGround()
    {
        if (playerObject != null && teleportDestination != null)
        {
            playerObject.transform.position = teleportDestination.position;
            playerObject.transform.rotation = teleportDestination.rotation; 
        }
    }

    public void MovementTutorialTrigger()
    {
        MovementTutorialPassed = true;
    }
}
