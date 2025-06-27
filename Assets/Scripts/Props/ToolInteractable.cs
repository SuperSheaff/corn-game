using UnityEngine;

public class ToolInteractable : Interactable
{
    public string PromptText;

    public override void OnInteract()
    {
        UIController.Instance?.ShowTool(true);
        gameObject.SetActive(false);
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
