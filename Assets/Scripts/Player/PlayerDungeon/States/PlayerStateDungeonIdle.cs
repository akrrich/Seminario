using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateDungeonIdle<T> : State<T>
{
    private PlayerDungeonModel dungeonModel;
    private PlayerDungeonView dungeonView;
  
    public override void Exit()
    {
        base.Exit();
        // Optional: Cleanup or stop animations
    }
}
