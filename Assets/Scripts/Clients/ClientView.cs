using UnityEngine;

public class ClientView : MonoBehaviour
{
    private PlayerController playerController;

    private Animator anim;
    private Transform order; // GameObject padre de la UI
    private SpriteRenderer foodSprite;

    private string food;
    private string[] foods = { "fish", "mouse" };


    void Awake()
    {
        GetComponents();
        InitializeRandomFoodUI();
    }

    void Update()
    {
        RotateOrderUIToLookAtPlayer();
    }


    private void GetComponents()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        //anim = GetComponentInChildren<Animator>();
        order = transform.Find("Order");
        foodSprite = transform.Find("Order").transform.Find("FoodsPositions").transform.Find("Position1").GetComponent<SpriteRenderer>();
    }

    private void InitializeRandomFoodUI()
    {
        int randomIndex = Random.Range(0, foods.Length);
        food = foods[randomIndex];

        switch (food)
        {
            case "fish":
                foodSprite.color = Color.yellow;
                break;

            case "mouse":
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
}
