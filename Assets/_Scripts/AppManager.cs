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
    [SerializeField] GameObject[] menuGridLayouts;
    [SerializeField] GameObject navbarCanvas;
    [SerializeField] TextMeshProUGUI[] navbarTexts;
    [SerializeField] RawImage[] navbarImages;
    [SerializeField] TMP_InputField searchInputField;
    [SerializeField] GameObject clearSearchButton;
    [SerializeField] TextMeshProUGUI shoppingCartPriceText;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemRangeText;
    [SerializeField] TextMeshProUGUI itemDescriptionText;
    [SerializeField] TextMeshProUGUI itemPriceText;
    [SerializeField] TextMeshProUGUI itemEnergyText;
    [SerializeField] RawImage itemImage;

    [Header("Textures")]
    [SerializeField] Texture2D emptyTexture;

    private int currentScreenIndex = -1;
    private int currentItemId = -1;
    private string myQuery;

    // A list of arrays that contain the itemId & itemQuantity
    public List<int[]> shoppingCart = new List<int[]>();

    // Use this for initialization
    private void Start()
    {
        // Set static link to this class to the instance of itself
        manager = this;

        // Go to the sign in screen
        MenuGoTo(0);

        LoadMenuScreen();

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
        // Validate 
    }

    public void SignUpButton()
    {

    }

    public void AddToCart()
    {
        // Loop through all of the existing cart items
        foreach (int[] item in shoppingCart)
        {
            // If the item is already in the cart
            if (item[0] == currentItemId)
            {
                // Increment the quantity by one
                item[1]++;

                // Exit this method
                return;
            }
        }

        // If the item is not already in the cart,
        // then add it and give it a quantity of one
        shoppingCart.Add(new int[2] { currentItemId, 1 });
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
        // Destroy all of the current menu items
        foreach (GameObject gridLayout in menuGridLayouts)
        {
            foreach (Transform menuItem in gridLayout.transform)
            {
                if (menuItem.gameObject.GetComponent<MenuItem>())
                {
                    Destroy(menuItem.gameObject);
                }
            }

            // Diable the grid layout group
            gridLayout.SetActive(false);
        }

        // If the seach input field is empty
        if (string.IsNullOrWhiteSpace(searchInputField.text))
        {
            // Select all of the rows in the menu table
            myQuery = "SELECT * FROM menu";
        }
        // Otherwise if the search input field contains text
        else
        {
            // Select rows in the menu table where the name or range are 
            // LIKE the search term
            myQuery = "SELECT * FROM menu WHERE Name LIKE '%" + searchInputField.text
                    + "%' OR Range LIKE '%" + searchInputField.text + "%';";
        }

        RunMyQuery();

        while (DB.reader.Read())
        {
            GameObject newItem = null;

            // Loop through each of the menu grid layouts
            foreach (GameObject gridLayout in menuGridLayouts)
            {
                // If the range of the layout matches the range of the pizza
                if (gridLayout.tag == DB.reader.GetString(2))
                {
                    // enable the grid layout group
                    gridLayout.SetActive(true);
                    // Instantiate the menu item inside the grid layout
                    newItem = Instantiate(menuItem, gridLayout.transform);
                    break;
                }
            }

            // Store reference to MenuItem component on object
            MenuItem newMenuItem = newItem.GetComponent<MenuItem>();

            // Set the ID on the menu item
            newMenuItem.SetItemId(DB.reader.GetInt32(0));

            // Fill in the text on the menu item
            newMenuItem.FillDetails(DB.reader.GetString(1),
                                    DB.reader.GetFloat(5),
                                    DB.reader.GetInt32(4),
                                    DB.reader.GetString(6));

        }

        DB.CloseDB();
    }

    public void LoadItemScreen(int itemId)
    {
        // Set the image temporarily to be blank
        itemImage.texture = emptyTexture;

        // Set the currentItemId to the item that was just clicked
        currentItemId = itemId;

        // SELECT the item row in the menu table
        myQuery = "SELECT * FROM menu WHERE ID = " + currentItemId + ";";

        RunMyQuery();

        if (DB.reader.Read())
        {
            itemNameText.text = DB.reader.GetString(1);
            itemRangeText.text = DB.reader.GetString(2).ToUpper();
            itemDescriptionText.text = DB.reader.GetString(3);
            itemEnergyText.text = DB.reader.GetInt32(4) + " KJ";
            itemPriceText.text = DB.reader.GetFloat(5).ToString("C");
            ImageProcessing.FetchMyImage(itemImage, DB.reader.GetString(6));
        }


        DB.CloseDB();
    }

    public void LoadShoppingCart()
    {
        foreach (int[] item in shoppingCart)
        {
            print("ID: " + item[0] + "   Quantity: " + item[1]);
        }
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
        LoadMenuScreen();
    }

    private void RunMyQuery()
    {
        // Close the DB if the reader is open
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

    void OnApplicationQuit()
    {
        DB.CloseDB();
    }
}
