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
    public bool MovementEnabled         = false;
    public bool MovementTutorialPassed  = false;
    public bool ScareCrow1Finished      = false;


    private bool waitingForTransmitAfterDialogue = true;
    private float transmitHoldTimer = 0f;
    private bool readyToTriggerNextDialogue = false;
    private Action nextScriptedDialogue;

    private int _scareSelector;

    // DIALOGUE & FMOD CITY

    private EventInstance _dialogueInstance1;
    [SerializeField] private EventReference _dialogueEvent1;

    private EventInstance _dialogueInstance2;
    [SerializeField] private EventReference _dialogueEvent2;

    // SCENE OBJECT CITY

    public GameObject Scene2;
    public GameObject Scene4;
    public GameObject Scene5;

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
        else
        {
            UIController.Instance.WalkieTutorialText(false);
        }
        if (Dialogue2Finished && !MovementTutorialPassed)
        {
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

        // Flags
        Dialogue1Started = false;
        Dialogue1Finished = false;
        Dialogue2Started = false;
        Dialogue2Finished = false;
    }

    public void SetupScene2()
    {
        Scene2.SetActive(true);
        // start some dialogue @ oscar
    }

    public void SetupScene3()
    {
        ScarecrowController.Instance?.SetState(ScarecrowState.Standing);
        // start some dialogue @ oscar
    }

    public void SetupScene4()
    {
        Scene4.SetActive(true);
        // start some dialogue @ oscar
    }

    public void SetupScene5()
    {
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
        // StartReceiving();
        // _dialogueInstance2.start();
        // _dialogueInstance2.release();
        // StartCoroutine(DelayedBoolSet(() => Dialogue2Started = true, 0.2f));

        Debug.Log("Dialogue 3");
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
}
