using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScenesManagerData", menuName = "ScriptableObjects/Create New ScenesManagerData")]
public class ScenesManagerData : ScriptableObject
{
    [SerializeField] private List<string> panelTips;

    [SerializeField] private float duringTimeLoadingScenePanel;
    [SerializeField] private float duringTimeExitGamePanel;

    public List<string> PanelTips { get => panelTips; } 

    public float DuringTimeLoadingScenePanel { get => duringTimeLoadingScenePanel; }
    public float DuringTimeExitGamePanel { get => duringTimeExitGamePanel; }
}
