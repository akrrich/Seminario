using System;

public class ActionNode : ITreeNode
{
    private Action action;


    public ActionNode(Action action)
    {
        this.action = action;
    }


    public void Execute()
    {
        if (PauseManager.Instance != null && !PauseManager.Instance.IsGamePaused)
        {
            if (action != null)
            {
                action();
            }
        }
    }
}
