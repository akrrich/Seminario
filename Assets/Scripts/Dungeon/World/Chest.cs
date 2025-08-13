using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(DropHandler))]
public class Chest : MonoBehaviour, IInteractable
{
    [Header("Loot & FX")]
    [SerializeField] private DropTable table;
    [SerializeField] private LootPrefabDatabase lootDB;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject fxOpen;
    [SerializeField] private Animator animator;     
    [SerializeField] private bool destroyAfterOpen = false;
    [SerializeField] private float waitTimeBeforeDestroy = 2f; // Tiempo antes de destruir el cofre

    private bool opened;
    private Collider col;
    private DropHandler dropHandler;
    private Outline outline;

    private void Awake()
    {
        col = GetComponent<Collider>();
        dropHandler = GetComponent<DropHandler>();
        animator = GetComponentInChildren<Animator>();
        outline = GetComponent<Outline>();

        dropHandler.Init(table, lootDB, spawnPoint);
        if (outline != null)
        {
            outline.OutlineWidth = 0f; // Forzamos a que arranque apagado
        }
    }
    public void Interact()
    {
        if (opened) return;
        StartCoroutine(CO_OpenChest());
    }


    private IEnumerator CO_OpenChest()
    {
        opened = true;
        col.enabled = false;

        if (fxOpen) fxOpen.GetComponent<ParticleSystem>().Play();
        if (animator) animator.SetBool("Open", true);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        dropHandler.DropLoot();

        if (animator) animator.SetBool("Open", false);

        if (destroyAfterOpen)
        {
            yield return new WaitForSeconds(waitTimeBeforeDestroy);
            Destroy(gameObject);
        }
    }

    public void ShowOutline()
    {
        if (outline != null)
        {
            outline.OutlineWidth = 5f;
            InteractionManagerUI.Instance.ModifyCenterPointUI(InteractionType.Interactive);
        }
    }

    public void HideOutline()
    {
        if (outline != null)
        {
            outline.OutlineWidth = 0f;
            InteractionManagerUI.Instance.ModifyCenterPointUI(InteractionType.Normal);
        }
    }
}
