using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateDungeonRun<T> : State<T>
{
    private PlayerDungeonModel dungeonModel;
    private PlayerDungeonView dungeonView;
  
    public override void Exit()
    {
        base.Exit();
        dungeonModel.Speed = 0f;
        //dungeonView.PlayRunAnimation(false);
    }
}
