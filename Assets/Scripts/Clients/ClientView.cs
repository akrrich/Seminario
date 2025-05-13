using UnityEngine;
using System;
using System.Collections.Generic;

public class ClientView : MonoBehaviour
{
    private PlayerController playerController;

    private Animator anim;
    private SpriteRenderer foodSpriteRenderer;
    private Transform order; // GameObject padre de la UI

    private List<string> orderFoodNames = new List<string>();

    [SerializeField] private List<FoodType> favoritesFoodTypes; // Las comidas que puede pedir
    [SerializeField] private List<FoodTypeSpritePair> foodSpritePairs; 

    private Dictionary<FoodType, Sprite> foodSpriteDict = new();

    private static event Action onFoodChangeUI; // Modificar para que no sea statico

    public List<string> OrderFoodNames { get => orderFoodNames; }

    public static Action OnFoodChangeUI { get => onFoodChangeUI; set => onFoodChangeUI = value; }


    void Awake()
    {
        GetComponents();
        SuscribeToOwnEvent();
        InitializeFoodSpriteDictionary();
    }

    void Update()
    {
        RotateOrderUIToLookAtPlayer();
        //MoveAnimationTowardsRootNode();
    }

    void OnDestroy()
    {
        UnsuscribeToOwnEvents();
    }


    public void WalkAnim()
    {
        RestartAnimationsValues();
        anim.SetBool("Walk", true);
    }

    public void SitAnim()
    {
        RestartAnimationsValues();
        anim.SetBool("Sit", true);
    }

    public void StandUpAnim()
    {
        RestartAnimationsValues();
        anim.SetBool("StandUp", true);
    }


    private void GetComponents()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        //anim = GetComponentInChildren<Animator>();
        foodSpriteRenderer = transform.Find("Order").Find("SpriteFood").GetComponent<SpriteRenderer>();
        order = transform.Find("Order");
    }

    private void SuscribeToOwnEvent()
    {
        onFoodChangeUI += InitializeRandomFoodUI;
    }

    private void UnsuscribeToOwnEvents()
    {
        onFoodChangeUI -= InitializeRandomFoodUI;
    }

    private void InitializeFoodSpriteDictionary()
    {
        foodSpriteDict.Clear();

        foreach (var pair in foodSpritePairs)
        {
            if (!foodSpriteDict.ContainsKey(pair.FoodType))
            {
                foodSpriteDict.Add(pair.FoodType, pair.Sprite);
            }
        }
    }

    private void InitializeRandomFoodUI()
    {
        orderFoodNames.Clear();

        int randomIndex = UnityEngine.Random.Range(0, favoritesFoodTypes.Count);
        FoodType selectedFood = favoritesFoodTypes[randomIndex];

        if (foodSpriteDict.TryGetValue(selectedFood, out Sprite sprite))
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

    private void MoveAnimationTowardsRootNode()
    {
        anim.transform.position = transform.position;
    }

    private void RestartAnimationsValues()
    {
        string[] parametersNames = { "Walk", "Sit", "StandUp" };

        for (int i = 0; i < parametersNames.Length; i++)
        {
            anim.SetBool(parametersNames[i], false);
        }
    }
}

[Serializable]
public class FoodTypeSpritePair
{
    [SerializeField] private FoodType foodType;
    [SerializeField] private Sprite sprite;

    public FoodType FoodType { get => foodType; } 
    public Sprite Sprite { get => sprite; }
}