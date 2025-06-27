using UnityEngine;

public class RadioTowerInteractable : Interactable
{
    public string PromptText;

    public override void OnInteract()
    {
        gameObject.SetActive(false);
        GameController.Instance?.SetupScene5();
        UIController.Instance?.ShowTool(false);
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
