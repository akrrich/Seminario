using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
public Camera[] cameras; 
    private int currentCameraIndex = 0;

    void Start()
    {
        
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(i == currentCameraIndex);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) 
        {
            // Desactiva la cámara actual
            cameras[currentCameraIndex].gameObject.SetActive(false);

            // Incrementa el índice (y vuelve a 0 si llega al final)
            currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;

            // Activa la nueva cámara
            cameras[currentCameraIndex].gameObject.SetActive(true);
        }
    }
}
