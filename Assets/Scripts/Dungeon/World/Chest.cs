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
    [SerializeField] private Animator animator;     // opcional
    [SerializeField] private bool destroyAfterOpen = false;
    [SerializeField] private float waitTimeBeforeDestroy = 2f; // Tiempo antes de destruir el cofre

    private bool opened;
    private Collider col;
    private DropHandler dropHandler;

    private void Awake()
    {
        col = GetComponent<Collider>();
        dropHandler = GetComponent<DropHandler>();
        animator = GetComponentInChildren<Animator>();
        dropHandler.Init(table, lootDB, spawnPoint);
    }

    public void Interact()
    {
        if (opened) return;
        StartCoroutine(OpenChest());
    }

    private IEnumerator OpenChest()
    {
        opened = true;
        col.enabled = false;

        if (animator) animator.SetBool("Open", true);
        
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        if (fxOpen) fxOpen.GetComponent<ParticleSystem>().Play();

        dropHandler.DropLoot();

        if (animator) animator.SetBool("Open", false);

        if (destroyAfterOpen)
            yield return new WaitForSeconds(waitTimeBeforeDestroy);

    }
}
