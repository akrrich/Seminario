using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum InteractionType
{
    Normal, Interactive
}

public class InteractionManagerUI : Singleton<InteractionManagerUI>
{
    [SerializeField] private InteractionManagerUIData interactionManagerUIData;

    [SerializeField] private Image centerPointUI;
    [SerializeField] private GameObject interactionMessage;
    [SerializeField] private TextMeshProUGUI interactionMessageText;

    public Image CenterPointUI { get => centerPointUI; }
    public TextMeshProUGUI InteractionMessageText { get => interactionMessageText; }
    public GameObject InteractionMessage { get => interactionMessage; }

    void Awake()
    {
        CreateSingleton(false);
        interactionMessage.gameObject.SetActive(false);
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
            centerPointUI.color = interactionManagerUIData.InteractiveColor;
        }

        else if (interactionType == InteractionType.Normal)
        {
            centerPointUI.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            centerPointUI.color = interactionManagerUIData.NormalColor;
        }
    }
}
