using UnityEngine;

public class DungeonInputReader
{
    public Vector3 GetMovementDirection()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        return new Vector3(h, 0, v).normalized;
    }

    public bool IsAttackPressed() => Input.GetButtonDown("Fire1");
    public bool IsDashPressed() => Input.GetKeyDown(KeyCode.Space);
}
