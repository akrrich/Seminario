using UnityEngine;

[CreateAssetMenu(fileName = "NewRoomConfig", menuName = "Dungeon/Room Config")]
public class RoomConfig : ScriptableObject
{
    public string roomID;
    public RoomSize size;

    //Para mas adelante
    [Header("Ambientación aleatoria")]
    public Vector2 fogChanceRange = new Vector2(0f, 0.3f);
    public Vector2 objectScaleRange = new Vector2(1f, 3f);

    [Header("Opciones extra")]
    public bool allowFog = true;
    public bool allowTraps = true;
    public bool allowLoot = true;
}
