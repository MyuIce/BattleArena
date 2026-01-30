using UnityEngine;

/*
public class ItemPickup : MonoBehaviour
{
    public itemdata itemdata;
    public GameObject pickupPromptUI; // 「Eで取得」UIオブジェクト

    private bool isPlayerInRange = false;

    void Start()
    {
        if (pickupPromptUI != null)
            pickupPromptUI.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.H))
        {
            Pickup();
        }
    }

    void Pickup()
    {
        InventoryManager.Instance.AddItem(itemdata);
        if (pickupPromptUI != null)
            pickupPromptUI.SetActive(false);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (pickupPromptUI != null)
                pickupPromptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (pickupPromptUI != null)
                pickupPromptUI.SetActive(false);
        }
    }
}

 */