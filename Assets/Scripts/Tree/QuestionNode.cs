using System;

public class QuestionNode : ITreeNode
{
    private Func<bool> question;
    private ITreeNode tNode; // True node
    private ITreeNode fNode; // False node


    public QuestionNode(Func<bool> question, ITreeNode tNode, ITreeNode fNode)
    {
        this.question = question;
        this.tNode = tNode;
        this.fNode = fNode;
    }


    public void Execute()
    {
        if (PauseManager.Instance != null && !PauseManager.Instance.IsGamePaused)
        {
            if (question())
            {
                if (tNode != null)
                {
                    tNode.Execute();
                }
            }

            else
            {
                if (fNode != null)
                {
                    fNode.Execute();
                }
            }
        }
    }
}
