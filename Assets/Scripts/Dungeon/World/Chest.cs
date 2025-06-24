
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

    private bool opened;
    private Collider col;
    private DropHandler dropHandler;

    private void Awake()
    {
        col = GetComponent<Collider>();
        dropHandler = GetComponent<DropHandler>();

        dropHandler.Init(table, lootDB, spawnPoint);
    }

    // ————————————————————————— IInteractable
    public void Interact()
    {
        if (opened) return;
        OpenChest();
    }

    // ————————————————————————— Interno
    private void OpenChest()
    {
        opened = true;
        col.enabled = false;                     // ya no colisiona / interactúa

        // 1. Reproducir fx o animación
        if (fxOpen) Instantiate(fxOpen, transform.position, Quaternion.identity);
        if (animator) animator.SetTrigger("Open");

        // 2. Generar loot
        dropHandler.DropLoot();

        // 3. Opcional: destruir cofre tras X segundos
        if (destroyAfterOpen)
            Destroy(gameObject, 2f);
    }
}
