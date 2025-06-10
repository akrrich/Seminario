using UnityEngine;

public class PlayerDungeonController : MonoBehaviour
{
    private PlayerDungeonModel model;
    private FSM<PlayerPhase> fsm = new FSM<PlayerPhase>();
   
    private CombatHandler combat;
    private DashHandler dash;

    


    private void Awake()
    {
        GetReferences();
        InitializeFSM();
    }

    private void Update()
    {
        if (model.IsDead) return;
        fsm.OnExecute();
        
    }

   

    private void GetReferences()
    {
        model = GetComponent<PlayerDungeonModel>();
        
        combat = GetComponent<CombatHandler>();
        dash = GetComponent<DashHandler>();
    }

    private void InitializeFSM()
    {
        fsm = new FSM<PlayerPhase>();
        PlayerStateDungeonIdle<PlayerPhase> psIdle = new PlayerStateDungeonIdle<PlayerPhase>(); 
   
    }

 
}
