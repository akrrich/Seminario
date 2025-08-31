using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyType", menuName = "Dungeon/Enemy Type")]
public class EnemyTypeSO : ScriptableObject
{
    public string enemyName;
    public GameObject enemyPrefab;
    public int baseHealth = 100;
    public int baseDamage = 10;
    public float baseSpeed = 3.5f;
    public int minCoins = 1;
    public int maxCoins = 5;

    [TextArea] public string description;

    // Método para crear instancia configurada
    public EnemyBase CreateInstance(Transform parent, Vector3 position, int layer, bool isSpecial = false)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError($"EnemyType {enemyName} has no prefab assigned!");
            return null;
        }

        GameObject instance = Instantiate(enemyPrefab, position, Quaternion.identity, parent);
        EnemyBase enemy = instance.GetComponent<EnemyBase>();

        return enemy;
    }
}