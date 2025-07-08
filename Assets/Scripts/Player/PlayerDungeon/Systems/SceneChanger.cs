using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("PortalToTabern"))
        {
            collider.gameObject.SetActive(false);
            string[] additiveScenes = { "TabernUI", "CompartidoUI" };
            StartCoroutine(ScenesManager.Instance.LoadScene("Tabern", additiveScenes));
        }
    }
}
