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
        if (question())
        {
            tNode.Execute();
        }
        else
        {
            fNode.Execute();
        }
    }
}
