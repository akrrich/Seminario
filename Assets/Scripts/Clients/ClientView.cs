using UnityEngine;
using System.Collections.Generic;

public class ClientView : MonoBehaviour
{
    private PlayerController playerController;
    private ClientManager clientManager;

    private Animator anim;
    private SpriteRenderer foodSpriteRenderer;
    private Transform order; // GameObject padre de la UI

    private List<string> orderFoodNames = new List<string>(); // Modificar esto para que sea unicamente un string solo

    [SerializeField] private List<FoodType> favoritesFoodTypes; // Las comidas que puede pedir

    private bool isInstantiateFirstTime = true;

    public Animator Anim { get => anim; }

    public List<string> OrderFoodNames { get => orderFoodNames; }


    void Awake()
    {
        GetComponents();
    }

    void OnEnable()
    {
        InitializeClientForPool();
    }

    void Update()
    {
        RotateOrderUIToLookAtPlayer();
    }


    public void ExecuteAnimParameterName(string animParameterName)
    {
        RestartAnimationsValues();
        anim.SetBool(animParameterName, true);
    }


    private void GetComponents()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        clientManager = FindFirstObjectByType<ClientManager>();

        anim = GetComponentInChildren<Animator>();
        foodSpriteRenderer = transform.Find("Order").Find("SpriteFood").GetComponent<SpriteRenderer>();
        order = transform.Find("Order");
    }

    private void InitializeClientForPool()
    {
        if (!isInstantiateFirstTime)
        {
            InitializeRandomFoodUI();
            return;
        }

        isInstantiateFirstTime = false;
    }

    private void InitializeRandomFoodUI()
    {
        orderFoodNames.Clear();

        int randomIndex = Random.Range(0, favoritesFoodTypes.Count);
        FoodType selectedFood = favoritesFoodTypes[randomIndex];

        Sprite sprite = clientManager.GetSpriteForRandomFood(selectedFood);

        if (sprite != null)
        {
            foodSpriteRenderer.sprite = sprite;
            foodSpriteRenderer.enabled = true;
            orderFoodNames.Add(selectedFood.ToString());
            AutoAdjustSpriteScale(sprite);
        }
    }

    private void AutoAdjustSpriteScale(Sprite sprite)
    {
        float maxDimension = 0.5f; 
        Vector2 spriteSize = sprite.bounds.size;

        float scaleFactor = maxDimension / Mathf.Max(spriteSize.x, spriteSize.y);
        foodSpriteRenderer.transform.localScale = Vector3.one * scaleFactor;
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

    private void RestartAnimationsValues()
    {
        string[] parametersNames = { "Walk", "Sit", "StandUp", "DuringSit", "WaitingForChair" };

        for (int i = 0; i < parametersNames.Length; i++)
        {
            anim.SetBool(parametersNames[i], false);
        }
    }
}