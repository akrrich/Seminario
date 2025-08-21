using UnityEngine;

[CreateAssetMenu(fileName = "ScenesManagerData", menuName = "ScriptableObjects/Create New ScenesManagerData")]
public class ScenesManagerData : ScriptableObject
{
    [SerializeField] private float duringTimeLoadingScenePanel;
    [SerializeField] private float duringTimeExitGamePanel;

    public float DuringTimeLoadingScenePanel { get => duringTimeLoadingScenePanel; }
    public float DuringTimeExitGamePanel { get => duringTimeExitGamePanel; }
}
