using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrderRow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI quantityText;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemSubtotalText;

    public void FillDetails(int quantity, string itemName, float itemPrice)
    {
        quantityText.text = quantity.ToString();
        itemNameText.text = itemName;
        itemSubtotalText.text = (quantity * itemPrice).ToString("C");
    }
}
