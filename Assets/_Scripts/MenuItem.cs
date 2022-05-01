using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuItem : MonoBehaviour
{
    private int itemId = -1;

    [SerializeField] RawImage itemImage;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemPriceText;
    [SerializeField] TextMeshProUGUI itemEnergyText;

    public void SetItemId(int id)
    {
        itemId = id;
    }

    public void FillDetails(string itemName, float itemPrice, int itemEnergy, string imageName)
    {
        itemNameText.text = itemName;
        itemPriceText.text = itemPrice.ToString("C");
        itemEnergyText.text = itemEnergy + " KJ";

        // Fetch the image
        ImageProcessing.FetchMyImage(itemImage, imageName);
    }

    public void MenuItemClicked()
    {
        // Switch to the item screen
        AppManager.manager.MenuGoTo(6);

        // Load the correct data into the item screen
        AppManager.manager.LoadItem(itemId);
    }
}
