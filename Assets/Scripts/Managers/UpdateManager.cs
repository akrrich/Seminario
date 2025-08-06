using System;

public class UpdateManager : Singleton<UpdateManager>
{
    // Adaptarlo de forma correcta el UpdateManager en cada script personalmente segun sus necesidades

    private static Action onUpdate;
    private static Action onFixedUpdate;

    public static Action OnUpdate { get => onUpdate; set => onUpdate = value; }
    public static Action OnFixedUpdate { get => onFixedUpdate; set => onFixedUpdate = value; }


    void Awake()
    {
        CreateSingleton(true);
    }

    void Update()
    {
        if (!ScenesManager.Instance.IsInLoadingScenePanel && !ScenesManager.Instance.IsInExitGamePanel)
        {
            onUpdate?.Invoke();
        }
    }

    void FixedUpdate()
    {
        if (!ScenesManager.Instance.IsInLoadingScenePanel && !ScenesManager.Instance.IsInExitGamePanel)
        {
            onFixedUpdate?.Invoke();
        }
    }
}
