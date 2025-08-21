using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateDungeonJump<T> : State<T>
{
    private PlayerDungeonModel model;
    private PlayerDungeonView view;
   
    public override void Exit()
    {
        base.Exit();
        // Optional: cleanup or reset jump state
    }
}
