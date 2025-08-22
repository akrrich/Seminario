using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;
    [SerializeField] private Camera cam;
    [SerializeField] private float duration;
    [SerializeField] private float magnitude;
    [SerializeField] private float zoomAmount;

    private void Awake()
    {
        originalPos = transform.localPosition;
        cam = GetComponent<Camera>();
    }

    public void Shake(float duration, float magnitude, float zoomAmount)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration, magnitude, zoomAmount));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude, float zoomAmount)
    {
        float elapsed = 0f;
        float originalFOV = cam.fieldOfView;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0f);

            cam.fieldOfView = Mathf.Lerp(originalFOV, originalFOV - zoomAmount, 0.5f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        cam.fieldOfView = originalFOV;
    }
}
