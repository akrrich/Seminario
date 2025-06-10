using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateDungeonWalk<T> : State<T>
{
    private PlayerDungeonModel dungeonModel;
    private PlayerDungeonView dungeonView;

    public override void Exit()
    {
        base.Exit();
        dungeonView.PlayWalkAnimation(false);
    }
}
