using UnityEngine;
using System.Collections.Generic;

public class ClientView : MonoBehaviour
{
    private PlayerController playerController;
    private ClientManager clientManager;
    [SerializeField] private Table tablePlayerCollision; 

    private Animator anim;
    private Transform order; // GameObject padre de la UI
    private List<SpriteRenderer> spritesTypeList = new List<SpriteRenderer>();

    private List<string> orderFoodNames = new List<string>(); // Modificar esto para que sea unicamente un string solo

    [SerializeField] private List<FoodType> favoritesFoodTypes; // Las comidas que puede pedir

    private Dictionary<string, SpriteRenderer> spriteTypeDict = new();

    private bool canTakeOrder = false; // Se pone en true cuando nos acercamos a la mesa y no podio nada todavia

    public Animator Anim { get => anim; }

    public List<string> OrderFoodNames { get => orderFoodNames; }

    public bool CanTakeOrder {get => canTakeOrder; set => canTakeOrder = value; }


    void Awake()
    {
        GetComponents();
        SuscribeToPlayerControllerEvent();
    }

    void Update()
    {
        RotateOrderUIToLookAtPlayer();
    }

    void OnDestroy()
    {
        UnsuscribeToPlayerControllerEvent();
    }


    private void SuscribeToPlayerControllerEvent()
    {
        PlayerController.OnTakeOrder += InitializeRandomFoodUI;

        PlayerController.OnTableCollisionEnterForTakeOrder += SaveTable;
        PlayerController.OnTableCollisionExitForTakeOrder += ClearTable;
    }

    private void UnsuscribeToPlayerControllerEvent()
    {
        PlayerController.OnTakeOrder -= InitializeRandomFoodUI;

        PlayerController.OnTableCollisionEnterForTakeOrder -= SaveTable;
        PlayerController.OnTableCollisionExitForTakeOrder -= ClearTable;
    }


    public void ExecuteAnimParameterName(string animParameterName)
    {
        RestartAnimationsValues();
        anim.SetBool(animParameterName, true);
    }

    public void SetSpriteType(string spriteTypeNameGameObjectInHierarchy)
    {
        DisableAllSpriteTypes();

        if (spriteTypeDict.TryGetValue(spriteTypeNameGameObjectInHierarchy, out var spriteRenderer))
        {
            spriteRenderer.gameObject.SetActive(true);
            spriteRenderer.enabled = true;
        }
    }

    public void DisableAllSpriteTypes()
    {
        foreach (var type in spritesTypeList)
        {
            type.gameObject.SetActive(false);
        }
    }

    // Verifica que no se haya hecho ningun pedido, es decir verifica que la orden de la comida no este activada
    public bool ReturnSpriteOrderIsActive()
    {
        return spritesTypeList[0].gameObject.activeSelf;
    }

    public void InitializeRandomFoodUI()
    {
        if (spritesTypeList[2].gameObject.activeSelf && tablePlayerCollision != null) // Si esta activado el sprite de pedir comida, quiere decir que ya se le puede tomar el pedido
        {
            orderFoodNames.Clear();

            int randomIndex = Random.Range(0, favoritesFoodTypes.Count);
            FoodType selectedFood = favoritesFoodTypes[randomIndex];

            Sprite sprite = clientManager.GetSpriteForRandomFood(selectedFood);

            if (sprite != null)
            {
                DisableAllSpriteTypes();
                spritesTypeList[0].gameObject.SetActive(true);

                spritesTypeList[0].sprite = sprite;
                spritesTypeList[0].enabled = true;
                orderFoodNames.Add(selectedFood.ToString());
                AutoAdjustSpriteScale(sprite);
            }

            canTakeOrder = false;
            ClearTable();
        }
    }


    private void GetComponents()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        clientManager = FindFirstObjectByType<ClientManager>();

        anim = GetComponentInChildren<Animator>();
        order = transform.Find("Order");

        foreach (Transform child in order)
        {
            SpriteRenderer spriteRenderer = child.GetComponentInChildren<SpriteRenderer>();
            spritesTypeList.Add(spriteRenderer);
            spriteTypeDict.Add(child.name, spriteRenderer);
        }
    }

    private void AutoAdjustSpriteScale(Sprite sprite)
    {
        float maxDimension = 0.5f; 
        Vector2 spriteSize = sprite.bounds.size;

        float scaleFactor = maxDimension / Mathf.Max(spriteSize.x, spriteSize.y);
        spritesTypeList[0].transform.localScale = Vector3.one * scaleFactor;
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

    private void SaveTable(Table table)
    {
        if (canTakeOrder && !spritesTypeList[0].gameObject.activeSelf)
        {
            tablePlayerCollision = table;
        }
    }

    private void ClearTable()
    {
        tablePlayerCollision = null;
    }
}