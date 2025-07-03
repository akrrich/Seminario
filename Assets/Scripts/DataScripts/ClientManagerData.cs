using UnityEngine;

[CreateAssetMenu(fileName = "ClientManagerData", menuName = "ScriptableObjects/Create New ClientManagerData")]
public class ClientManagerData : ScriptableObject
{
    [SerializeField] private float timeToWaitForSpawnNewClient;

    public float TimeToWaitForSpawnNewClient { get => timeToWaitForSpawnNewClient; }
}
