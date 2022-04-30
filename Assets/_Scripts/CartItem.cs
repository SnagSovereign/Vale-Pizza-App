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
    private int itemQuantity = 0;

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
        itemQuantity++;
        // TODO: update the shopping cart list in AppManager

        UpdateShoppingCartList();

        UpdateUI();
    }

    public void DecrementQuantity()
    {
        if (itemQuantity <= 1)
        {
            DeleteCartItem();
            return;
        }

        itemQuantity--;

        UpdateShoppingCartList();

        UpdateUI();
    }

    private void UpdateShoppingCartList()
    {
        foreach (int[] item in AppManager.manager.shoppingCart)
        {
            if (item[0] == itemId)
            {
                item[1] = itemQuantity;
                break;
            }
        }
    }

    public void DeleteCartItem()
    {
        // Remove the item from the shopping cart list
        AppManager.manager.RemoveFromCart(itemId);

        Destroy(gameObject);
    }

    public void FillDetails(string itemName, float itemPrice, string imageName, int quantity)
    {
        itemNameText.text = itemName;
        itemPriceText.text = itemPrice.ToString("C");
        ImageProcessing.FetchMyImage(itemImage, imageName);
        itemQuantity = quantity;
        UpdateUI();
    }

    private void UpdateUI()
    {
        itemQuantityText.text = itemQuantity.ToString();
    }
}
