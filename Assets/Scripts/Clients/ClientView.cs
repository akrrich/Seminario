using UnityEngine;
using System;

public class ClientView : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField] private Animator anim;
    private Transform order; // GameObject padre de la UI
    private SpriteRenderer foodSprite;

    private static event Action onWalkEnter;
    private static event Action onSitEnter;
    private static event Action onStanUpEnter;

    private static event Action onFoodChange;

    private string foodName;
    private string[] foods = { "Fish", "Mouse" };

    public static Action OnWalkEnter { get => onWalkEnter; set => onWalkEnter = value; }
    public static Action OnSitEnter { get => onSitEnter; set => onSitEnter = value; }
    public static Action OnStandUpEnter { get => onStanUpEnter; set => onStanUpEnter = value; }

    public static Action OnFoodChange { get => onFoodChange; set => onFoodChange = value; }

    public string FoodName { get => foodName; }


    void Awake()
    {
        GetComponents();
        SuscribeToOwnEvent();
        InitializeRandomFoodUI();
    }

    void Update()
    {
        RotateOrderUIToLookAtPlayer();

        anim.transform.position = transform.position;
    }

    void OnEnable()
    {
        anim.SetBool("Walk", true);
    }

    void OnDestroy()
    {
        //UnsuscribeToOwnEvents();
    }


    private void GetComponents()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        anim = GetComponentInChildren<Animator>();
        order = transform.Find("Order");
        foodSprite = transform.Find("Order").transform.Find("FoodsPositions").transform.Find("Position1").GetComponent<SpriteRenderer>();
    }

    private void SuscribeToOwnEvent()
    {
        onWalkEnter += Walk;
        onSitEnter += Sit;
        onStanUpEnter += StandUp;

        onFoodChange += InitializeRandomFoodUI;
    }

    private void UnsuscribeToOwnEvents()
    {
        /*onWalkEnter -= () => ExecuteCurrentAnimation(0);
        onSitEnter -= () => ExecuteCurrentAnimation(1);
        onStanUpEnter -= () => ExecuteCurrentAnimation(2);

        onFoodChange -= InitializeRandomFoodUI;*/
    }

    private void InitializeRandomFoodUI()
    {
        int randomIndex = UnityEngine.Random.Range(0, foods.Length);
        foodName = foods[randomIndex];

        switch (foodName)
        {
            case "Fish":
                foodSprite.color = Color.yellow;
                break;

            case "Mouse":
                foodSprite.color = Color.blue;
                break;
        }
    }

    private void RotateOrderUIToLookAtPlayer()
    {
        Vector3 playerDirection = (playerController.transform.position - order.position).normalized;
        Vector3 lookDirection = new Vector3(playerDirection.x, 0, playerDirection.z);

        if (lookDirection != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            order.rotation = rotation;
        }
    }

    /*private void ExecuteCurrentAnimation(int parameterIndex) 
    {
        string[] parametersNames = { "Walk", "Sit", "StandUp" };

        for (int i = 0; i < parametersNames.Length; i++)
        {
            anim.SetBool(parametersNames[i], i == parameterIndex);

            if (i == parameterIndex)
            {
                break;
            }
        }
    }*/

    private void Walk()
    {
        anim.SetBool("Walk", true);
    }

    private void Sit()
    {
        anim.SetBool("Sit", true);
    }

    private void StandUp()
    {
        anim.SetBool("StandUp", true);
    }
}
