using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AppManager : MonoBehaviour
{
    // Create a static link to this class so that other
    // classes can access its public methods & variables
    public static AppManager manager;

    [Header("Prefabs for instantiation")]
    [SerializeField] GameObject menuItem;
    [SerializeField] GameObject cartItem;

    [Header("UI Object References")]
    [SerializeField] GameObject[] screens;
    [SerializeField] GameObject[] gridLayoutGroups;
    [SerializeField] GameObject navbarCanvas;
    [SerializeField] TextMeshProUGUI[] navbarTexts;
    [SerializeField] RawImage[] navbarImages;
    [SerializeField] TMP_InputField searchInputField;
    [SerializeField] GameObject clearSearchButton;
    [SerializeField] TextMeshProUGUI shoppingCartPriceText;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemRangeText;
    [SerializeField] TextMeshProUGUI itemPriceText;

    private int currentScreenIndex = -1;
    private string myQuery;

    // Use this for initialization
    private void Start()
    {
        // Set static link to this class to the instance of itself
        manager = this;

        // Go to the sign in screen
        MenuGoTo(0);

        itemNameText.text = "Pepperoni";

        // string range = "Premium";

        // foreach (GameObject group in gridLayoutGroups)
        // {
        //     if (group.tag == range)
        //     {
        //         group.SetActive(true);
        //         GameObject newMenuItem = Instantiate(menuItem, group.transform);
        //         break;
        //     }
        // }
    }

    public void LoginButton()
    {

    }

    public void SignUpButton()
    {

    }

    public void MenuGoTo(int screenIndex)
    {
        // changes the UI canvas
        // The corresponding index for each screen are as follows:
        //          0 = Sign In
        //          1 = Sign Up
        //          2 = Menu
        //          3 = Shopping Cart
        //          4 = Account
        //          5 = History
        //          6 = Item
        //          7 = Checkout
        //          8 = Edit Account

        // Exit this method if the currentScreenIndex is already the screenIndex to switch to
        if (currentScreenIndex == screenIndex) return;

        // Disable all of the screens
        foreach (GameObject screen in screens)
        {
            screen.SetActive(false);
        }

        // Enable the new screen that is being switched to
        screens[screenIndex].SetActive(true);

        if (screens[screenIndex].tag == "Navbar Screen")
        {
            navbarCanvas.SetActive(true);
        }
        else
        {
            navbarCanvas.SetActive(false);
        }

        // Update the currentScreenIndex to the new one
        currentScreenIndex = screenIndex;
    }

    public void LoadMenuScreen()
    {
        Debug.Log("Menu screen refreshing...");
    }

    public void LoadItemScreen(int itemId)
    {
        Debug.Log("Data loading...");
    }

    public void HighlightNavButton(int buttonIndex)
    {
        // Reset the fonstyle and color for each text & image
        for (int index = 0; index < navbarTexts.Length; index++)
        {
            navbarTexts[index].fontStyle = TMPro.FontStyles.Normal;
            navbarTexts[index].color = navbarImages[index].color = new Color32(180, 180, 180, 255);
        }

        // Change the fontsyle to bold and color to black
        navbarTexts[buttonIndex].fontStyle = TMPro.FontStyles.Bold;
        navbarTexts[buttonIndex].color = navbarImages[buttonIndex].color = Color.white;
    }

    // This method is called whenever there is a change in the search InputField
    public void SearchInputUpdate()
    {
        // Check if the inputfield contains characters
        if (searchInputField.text.Length > 0)
        {
            clearSearchButton.SetActive(true);
        }
        else
        {
            clearSearchButton.SetActive(false);
        }

        // Reload the menu screen
        LoadMenuScreen();
    }

    public void ClearSearch()
    {
        searchInputField.text = "";
    }

    private void RunMyQuery()
    {
        // Close the DB is the reader is open
        if (DB.reader != null)
        {
            DB.CloseDB();
        }

        // Set up the path to the DB file
        string dbPath = "URI=file:" + Path.Combine(
                        Application.streamingAssetsPath,
                        "ValePizza.db");

        // Set up and open the connection to the DB
        DB.Connect(dbPath);

        // Run the query to fill the data reader
        DB.RunQuery(myQuery);
    }
}
