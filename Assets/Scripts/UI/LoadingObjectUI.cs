using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingObjectUI : MonoBehaviour
{
    [Header("Referencias de Objetos")]
    [SerializeField] private RectTransform outsideRing;
    [SerializeField] private RectTransform insideSquare;
    [SerializeField] private RectTransform triangle;

    [Header("Configuración Anillo Exterior")]
    [SerializeField] private float outsideRingRotateSpeed = 30f; // Grados por segundo
    [SerializeField] private LeanTweenType outsideRingEase = LeanTweenType.linear;

    [Header("Configuración Cuadrado Interior")]
    [SerializeField] private float insideSquareRotateSpeed = -60f; // Grados por segundo (negativo para otra dirección)
    [SerializeField] private LeanTweenType insideSquareEase = LeanTweenType.linear; 

    [Header("Configuración Triángulo")]
    [SerializeField] private float triangleShiftRotateDegrees = 180f; // Rotación total en un shift
  //  [SerializeField] private float triangleShiftTime = 0.25f; // Tiempo que toma hacer el shift
    [SerializeField] private float triangleLoopDuration = 3f;
    [SerializeField] private LeanTweenType triangleShiftEase = LeanTweenType.easeOutQuad;
    

    void OnEnable()
    {
        AnimateOutsideRing();
        AnimateInsideSquare();
        AnimateTriangleShift();
    }

    private void AnimateOutsideRing()
    {
        if (outsideRing == null) return;

        // Gira 360 grados sobre el eje Z (Vector3.forward)
        float timeToCompleteLoop = 360f / outsideRingRotateSpeed;

        if (!ScenesManager.Instance.IsInExitGamePanel)
        {
            LeanTween.rotateAroundLocal(outsideRing.gameObject, Vector3.forward, 360f, timeToCompleteLoop)
            .setEase(outsideRingEase)
            .setIgnoreTimeScale(false)
            .setLoopCount(-1); // -1 para bucle infinito
            return;
        }

        else
        {
            LeanTween.rotateAroundLocal(outsideRing.gameObject, Vector3.forward, 360f, timeToCompleteLoop)
            .setEase(outsideRingEase)
            .setIgnoreTimeScale(true)
            .setLoopCount(-1); // -1 para bucle infinito
            return;
        }
    }

    private void AnimateInsideSquare()
    {
        if (insideSquare == null) return;

        // Gira -360 grados (sentido horario) sobre el eje Z
        float timeToCompleteLoop = 360f / Mathf.Abs(insideSquareRotateSpeed);

        if (!ScenesManager.Instance.IsInExitGamePanel)
        {
            LeanTween.rotateAroundLocal(insideSquare.gameObject, Vector3.forward, -360f, timeToCompleteLoop)
            .setEase(insideSquareEase)
            .setIgnoreTimeScale(false)
            .setLoopCount(-1);
            return;
        }

        else
        {
            LeanTween.rotateAroundLocal(outsideRing.gameObject, Vector3.forward, -360f, timeToCompleteLoop)
            .setEase(outsideRingEase)
            .setIgnoreTimeScale(true)
            .setLoopCount(-1); // -1 para bucle infinito
            return;
        }
    }

    private void AnimateTriangleShift()
    {
        if (triangle == null) return;

        if (!ScenesManager.Instance.IsInExitGamePanel)
        {
            LeanTween.rotateAroundLocal(gameObject, Vector3.forward, triangleShiftRotateDegrees, triangleLoopDuration)
             .setEase(triangleShiftEase) // El ease de un timer no importa
             .setIgnoreTimeScale(false)
             .setLoopCount(-1); // Repetir infinitamente
            return;
        }

        else
        {
            LeanTween.rotateAroundLocal(gameObject, Vector3.forward, triangleShiftRotateDegrees, triangleLoopDuration)
             .setEase(triangleShiftEase) // El ease de un timer no importa
             .setIgnoreTimeScale(true)
             .setLoopCount(-1); // Repetir infinitamente
            return;
        }
    }  
}