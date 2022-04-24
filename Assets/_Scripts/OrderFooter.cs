using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrderFooter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dateTimeText;
    [SerializeField] TextMeshProUGUI totalText;

    public void FillDetails(string date, string time, float total)
    {
        dateTimeText.text = date + "    " + time;
        totalText.text = total.ToString("C");
    }
}
