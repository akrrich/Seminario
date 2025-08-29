using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(DropHandler))]
[RequireComponent(typeof(Outline))] // Requerir componente Outline
public class Chest : MonoBehaviour, IInteractable
{
    [Header("CONFIGURACIÓN REQUERIDA")]
    [Tooltip("El objeto debe estar en la layer 'Interactable' para ser detectado por el sistema de interacción")]
    [SerializeField] private bool layerCheck = true; // Solo para referencia en inspector

    [Header("LOOT & SISTEMA DE RECOMPENSAS")]
    [Tooltip("Tabla de loot que define qué items pueden dropear")]
    [SerializeField] private DropTable table;

    [Tooltip("Base de datos de prefabs de loot para instanciar los items")]
    [SerializeField] private LootPrefabDatabase lootDB;

    [Tooltip("Punto de spawn donde aparecerán los items del cofre")]
    [SerializeField] private Transform spawnPoint;

    [Header("ANIMACIÓN & EFECTOS VISUALES")]
    [Tooltip("Animator que controla la animación de apertura/cierre del cofre")]
    [SerializeField] private Animator animator;

    [Tooltip("Sistema de partículas que se activa al abrir el cofre")]
    [SerializeField] private GameObject fxOpen;

    [Header("COMPORTAMIENTO")]
    [Tooltip("Si se marca, el cofre se destruirá después de ser abierto")]
    [SerializeField] private bool destroyAfterOpen = false;

    [Tooltip("Tiempo de espera antes de destruir el cofre (si destroyAfterOpen está activado)")]
    [SerializeField] private float waitTimeBeforeDestroy = 2f;

    private bool opened;
    private Collider col;
    private DropHandler dropHandler;
    private Outline outline;

    public InteractionMode InteractionMode => InteractionMode.Press;

    private void Awake()
    {
        col = GetComponent<Collider>();
        dropHandler = GetComponent<DropHandler>();
        outline = GetComponent<Outline>();

        // Buscar animator en hijos si no está asignado
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // Inicializar el sistema de drop
        dropHandler.Init(table, lootDB, spawnPoint);

        // Configurar outline
        if (outline != null)
        {
            outline.OutlineWidth = 0f;
            outline.OutlineColor = Color.yellow; 
        }

        // Verificar layer (solo para debug)
        if (layerCheck && gameObject.layer != LayerMask.NameToLayer("Interactable"))
        {
            Debug.LogWarning($"[Chest] {gameObject.name} no está en la layer 'Interactable'. El sistema de detección podría no funcionar correctamente.");
        }
    }

    public void Interact(bool isPressed)
    {
        if (opened) return;
        StartCoroutine(CO_OpenChest());
    }

    private IEnumerator CO_OpenChest()
    {
        opened = true;
        col.enabled = false; // Desactivar colisionador para evitar múltiples interacciones

        // Reproducir efectos y animación
        if (fxOpen != null)
            fxOpen.SetActive(true);

        if (animator != null)
            animator.SetBool("Open", true);

        // Esperar a que termine la animación
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Soltar el loot
        dropHandler.DropLoot();

        // Si se destruye después de abrir, esperar y destruir
        if (destroyAfterOpen)
        {
            yield return new WaitForSeconds(waitTimeBeforeDestroy);
            Destroy(gameObject);
        }
    }

    public void ShowOutline()
    {
        Debug.Log("askd eeee cofre");
        if (outline != null && !opened)
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

   
    public void ShowMessage(TextMeshProUGUI interactionManagerUIText)
    {
       
    }

    public void HideMessage(TextMeshProUGUI interactionManagerUIText)
    {
       
    }
}