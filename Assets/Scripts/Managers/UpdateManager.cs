using System;

public class UpdateManager : Singleton<UpdateManager>
{
    // Adaptarlo de forma correcta el UpdateManager en cada script personalmente segun sus necesidades

    private static event Action onUpdateAllTime; // Exclusivo para los Scripts que necesitan ejecutar Update durante todo el tiempo sin condicion alguna (EJ: scripts que necesitan ejecutar Update durante las pantallas de carga, etc)
    private static event Action onUpdate;
    private static event Action onFixedUpdate;

    public static Action OnUpdateAllTime { get => onUpdateAllTime; set => onUpdateAllTime = value; }
    public static Action OnUpdate { get => onUpdate; set => onUpdate = value; }
    public static Action OnFixedUpdate { get => onFixedUpdate; set => onFixedUpdate = value; }


    void Awake()
    {
        CreateSingleton(true);
    }

    void Update()
    {
        onUpdateAllTime?.Invoke();

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
