using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatFeedbackManager : MonoBehaviour
{
    //Only for the playerfeedback getting damage and doing the damage, vfx and sfx.
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private GameObject hitVFX;

    public static CombatFeedbackManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayHitEffect(Vector3 position)
    {
        Instantiate(hitVFX, position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(hitSound, position);
    }
}
