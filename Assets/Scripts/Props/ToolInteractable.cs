using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class ToolInteractable : Interactable
{
    public string PromptText;

    private EventInstance plierStingerInstance;
    [SerializeField] EventReference _plierStingerEvent;

    public void Start()
    {
        plierStingerInstance = RuntimeManager.CreateInstance(_plierStingerEvent);
    }

    public override void OnInteract()
    {
        UIController.Instance?.ShowTool(true);
        gameObject.SetActive(false);
        plierStingerInstance.start();
        GameController.Instance?.SetupScene3();
    }

    public override void OnLookAt()
    {
        UIController.Instance?.ShowPrompt(PromptText);
    }

    public override void OnLookAway()
    {
        UIController.Instance?.HidePrompt();
    }
}
