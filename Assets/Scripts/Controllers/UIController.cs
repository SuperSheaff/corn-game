using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    public GameObject HandIdle;
    public GameObject HandReceiving;
    public GameObject HandTransmitting;
    public GameObject HandTool;

    public GameObject RadioTutorialText;
    public GameObject WalkTutorialText;
    private EventInstance WalkieOnInstance;
    [SerializeField]
    private EventReference _walkieOnEvent;
    private EventInstance WalkieOffInstance;
    [SerializeField]
    private EventReference _walkieOffEvent;

    public TextMeshProUGUI promptTextUI; // assigned in inspector

    public enum HandName { Idle, Receiving, Transmitting }

    [SerializeField] private Image fadeImage;      // Fullscreen black image
    [SerializeField] private float fadeDuration = 2f;
    private Coroutine currentFade;

    //fmod
    private

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        fadeImage.gameObject.SetActive(true);

        //fmod
        WalkieOnInstance = RuntimeManager.CreateInstance(_walkieOnEvent);
        WalkieOffInstance = RuntimeManager.CreateInstance(_walkieOffEvent);

        FadeFromBlack();
    }

    public void ShowHand(HandName hand)
    {
        HandIdle.SetActive(false);
        HandReceiving.SetActive(false);
        HandTransmitting.SetActive(false);

        switch (hand)
        {
            case HandName.Idle:
                HandIdle.SetActive(true);
                WalkieOnInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                WalkieOffInstance.start();
                break;
            case HandName.Receiving:
                HandReceiving.SetActive(true);
                break;
            case HandName.Transmitting:
                HandTransmitting.SetActive(true);
                //fmod
                WalkieOnInstance.start();
                break;
        }
    }

    public void ShowTool(bool value)
    {
        HandTool.SetActive(value);
    }

    public void WalkieTutorialText(bool show)
    {
        RadioTutorialText.SetActive(show);
    }

    public void MoveTutorialText(bool show)
    {
        WalkTutorialText.SetActive(show);
    }

    public void ShowPrompt(string text)
    {
        promptTextUI.text = text;
        promptTextUI.gameObject.SetActive(true);
    }

    public void HidePrompt()
    {
        promptTextUI.gameObject.SetActive(false);
    }

    public void FadeFromBlack()
    {
        if (currentFade != null) StopCoroutine(currentFade);
        fadeImage.gameObject.SetActive(true);
        currentFade = StartCoroutine(FadeRoutine(Color.black, new Color(0, 0, 0, 0)));
    }

    public void FadeToBlack()
    {
        if (currentFade != null) StopCoroutine(currentFade);
        fadeImage.gameObject.SetActive(true);
        currentFade = StartCoroutine(FadeRoutine(new Color(0, 0, 0, 0), Color.black));
    }

    private System.Collections.IEnumerator FadeRoutine(Color startColor, Color endColor)
    {
        float timer = 0f;

        fadeImage.color = startColor;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        fadeImage.color = endColor;

        if (endColor.a == 0f)
            fadeImage.gameObject.SetActive(false); // Hide if fully transparent
    }
}
