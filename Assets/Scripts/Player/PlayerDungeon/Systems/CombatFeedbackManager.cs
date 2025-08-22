using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatFeedbackManager : MonoBehaviour
{
    //Only for the playerfeedback getting damage and doing the damage, vfx and sfx.
    [SerializeField] private List<AudioClip> damageSounds;
    [SerializeField] private GameObject hitVFX;
    private CameraShake cameraShake;

    public static CombatFeedbackManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    public void PlayRandomDamageSound(Vector3 position)
    {
        if (damageSounds == null || damageSounds.Count == 0) return;

        int index = Random.Range(0, damageSounds.Count);
        AudioClip selected = damageSounds[index];

        AudioSource.PlayClipAtPoint(selected, position);
    }

    public void ShakeCamera(float duration = 0.1f, float magnitude = 0.2f, float zoom = 5f)
    {
        cameraShake?.Shake(duration, magnitude, zoom);
    }
}
