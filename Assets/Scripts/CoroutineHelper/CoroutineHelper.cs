using System.Collections;
using UnityEngine;

public class CoroutineHelper : Singleton<CoroutineHelper>
{
    void Awake()
    {
        CreateSingleton(false);
    }


    public Coroutine StartHelperCoroutine(IEnumerator routine)
    {
        return Instance.StartCoroutine(routine);
    }
}
