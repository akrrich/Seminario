using System.Collections;
using UnityEngine;

/// <summary>
/// Permite hacer parpadear el mesh en rojo cuando recibe da�o.
/// No crea materiales nuevos: usa un MaterialPropertyBlock.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class DamageFlash : MonoBehaviour
{
    [Header("Flash")]
    [Tooltip("Color del destello")]
    [SerializeField] private Color flashColor = Color.red;
    [Tooltip("Duraci�n de cada destello (segundos)")]
    [SerializeField] private float flashTime = 0.08f;
    [Tooltip("Cu�ntas veces parpadea")]
    [SerializeField] private int flashes = 3;

    private static readonly int ColorID = Shader.PropertyToID("_Color"); 

    private Renderer rendererRef;
    private MaterialPropertyBlock block;
    private Color originalColor;

    private void Awake()
    {
        rendererRef = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();

        // Guarda el color original la primera vez
        rendererRef.GetPropertyBlock(block);
        originalColor = block.HasColor(ColorID)
                        ? block.GetColor(ColorID)
                        : rendererRef.sharedMaterial.GetColor(ColorID);
    }

    /// <summary>Llamar desde TakeDamage.</summary>
    public void TriggerFlash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        for (int i = 0; i < flashes; i++)
        {
            // Pone rojo
            block.SetColor(ColorID, flashColor);
            rendererRef.SetPropertyBlock(block);
            yield return new WaitForSeconds(flashTime);

            // Restaura color original
            block.SetColor(ColorID, originalColor);
            rendererRef.SetPropertyBlock(block);
            yield return new WaitForSeconds(flashTime);
        }
    }
}
