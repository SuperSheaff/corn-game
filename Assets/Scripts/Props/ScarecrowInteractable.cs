using UnityEngine;

public class ScarecrowInteractable : Interactable
{
    [SerializeField] private ScarecrowState targetState = ScarecrowState.OnPost;
    public string PromptText;

    public override void OnInteract()
    {
        ScarecrowController.Instance?.SetState(targetState);
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
