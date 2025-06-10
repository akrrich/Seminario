using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateDungeonDash<T> : State<T>
{
    private PlayerDungeonModel model;
    private PlayerDungeonView view;
    private DashHandler dash;
    
    public override void Exit()
    {
        base.Exit();
       
       
    }
}
