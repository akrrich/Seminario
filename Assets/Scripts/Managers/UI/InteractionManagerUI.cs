using UnityEngine;
using UnityEngine.UI;

public enum InteractionType
{
    Normal, Interactive
}

public class InteractionManagerUI : Singleton<InteractionManagerUI>
{
    [SerializeField] private Image centerPointUI;

    [SerializeField] private Color interactiveColor;

    public Image CenterPointUI { get => centerPointUI; }


    void Awake()
    {
        CreateSingleton(false);
    }

    // En modo BUILD desaparece el punto de forma correcta
    public void ShowOrHideCenterPointUI(bool value)
    {
        if (this != null)
        {
            centerPointUI.gameObject.SetActive(value);
        }
    }

    public void ModifyCenterPointUI(InteractionType interactionType)
    {
        if (interactionType == InteractionType.Interactive)
        {
            centerPointUI.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            centerPointUI.color = interactiveColor;
        }

        else if (interactionType == InteractionType.Normal)
        {
            centerPointUI.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            centerPointUI.color = Color.white;
        }
    }
}
