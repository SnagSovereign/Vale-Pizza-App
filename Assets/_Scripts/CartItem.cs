using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CartItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemPriceText;
    [SerializeField] TextMeshProUGUI itemQuantityText;
    [SerializeField] RawImage itemImage;

    private int itemId = -1;
    private int quantity = 0;

    public void SetItemId(int id)
    {
        itemId = id;
    }

    public int GetItemId()
    {
        return itemId;
    }

    public void IncrementQuantity()
    {
        quantity++;
        // TODO: update the shopping cart list in AppManager
        UpdateUI();
    }

    public void DecrementQuantity()
    {
        if (quantity <= 0) return;
        quantity--;
        // TODO: update the shopping cart list in AppManager
        UpdateUI();
    }

    public void DeleteCartItem()
    {
        // Remove the item from the shopping cart list


        Destroy(gameObject);
    }

    public void FillDetails(string itemName, float itemPrice, string imageName)
    {
        itemNameText.text = itemName;
        itemPriceText.text = itemPrice.ToString("C");

        // Fetch the image

        // Display the image
    }

    private void UpdateUI()
    {
        itemQuantityText.text = quantity.ToString();
    }
}
