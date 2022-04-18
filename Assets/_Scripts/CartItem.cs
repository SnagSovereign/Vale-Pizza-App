using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CartItem : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI quantityText;

    private int itemId = -1;
    private int quantity = 0;

    public int GetItemId()
    {
        return itemId;
    }

    public void SetItemId(int id)
    {
        itemId = id;
    }

    public void IncrementQuantity()
    {
        quantity++;
        UpdateUI();
    }

    public void DecrementQuantity()
    {
        if (quantity <= 0) return;
        quantity--;
        UpdateUI();
    }

    private void UpdateUI()
    {
        quantityText.text = quantity.ToString();
    }

    public void DeleteCartItem()
    {
        Destroy(gameObject);
    }
}
