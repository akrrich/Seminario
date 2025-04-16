using UnityEngine;
using System;
using System.Collections.Generic;

public class ClientView : MonoBehaviour
{
    private PlayerController playerController;

    private Animator anim;
    private Transform order; // GameObject padre de la UI
    private List<SpriteRenderer> foodSprites = new List<SpriteRenderer>();

    private List<string> orderFoodNames = new List<string>();

    private static event Action onWalkEnter;
    private static event Action onSitEnter;
    private static event Action onStanUpEnter;

    private static event Action onFoodChange;

    private string[] foodsTypes = { "Fish", "Mouse" };

    public List<string> OrderFoodNames { get => orderFoodNames; }

    public static Action OnWalkEnter { get => onWalkEnter; set => onWalkEnter = value; }
    public static Action OnSitEnter { get => onSitEnter; set => onSitEnter = value; }
    public static Action OnStandUpEnter { get => onStanUpEnter; set => onStanUpEnter = value; }

    public static Action OnFoodChange { get => onFoodChange; set => onFoodChange = value; }


    void Awake()
    {
        GetComponents();
        SuscribeToOwnEvent();
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


    private void GetComponents()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        //anim = GetComponentInChildren<Animator>();
        order = transform.Find("Order");

        Transform foodsPositions = order.Find("FoodsPositions");

        foreach (Transform position in foodsPositions)
        {
            foodSprites.Add(position.GetComponent<SpriteRenderer>());
        }
    }

    private void SuscribeToOwnEvent()
    {
        onWalkEnter += WalkAnim;
        onSitEnter += SitAnim;
        onStanUpEnter += StandUpAnim;

        onFoodChange += InitializeRandomFoodUI;
    }

    private void UnsuscribeToOwnEvents()
    {
        onWalkEnter -= WalkAnim;
        onSitEnter -= SitAnim;
        onStanUpEnter -= StandUpAnim;

        onFoodChange -= InitializeRandomFoodUI;
    }

    private void InitializeRandomFoodUI()
    {
        orderFoodNames.Clear();

        int foodCount = UnityEngine.Random.Range(1, foodSprites.Count + 1);

        for (int i = 0; i < foodSprites.Count; i++)
        {
            SpriteRenderer sprite = foodSprites[i];

            if (i < foodCount)
            {
                int randomIndex = UnityEngine.Random.Range(0, foodsTypes.Length);
                string selectedFood = foodsTypes[randomIndex];

                switch (selectedFood)
                {
                    case "Fish": sprite.color = Color.yellow; break;
                    case "Mouse": sprite.color = Color.blue; break;
                }

                orderFoodNames.Add(selectedFood);
                sprite.enabled = true;
            }

            else
            {
                sprite.color = Color.clear;
                sprite.enabled = false;
            }
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

    private void MoveAnimationTowardsRootNode()
    {
        anim.transform.position = transform.position;
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

    private void WalkAnim()
    {
        anim.SetBool("Walk", true);
    }

    private void SitAnim()
    {
        anim.SetBool("Sit", true);
    }

    private void StandUpAnim()
    {
        anim.SetBool("StandUp", true);
    }
}
