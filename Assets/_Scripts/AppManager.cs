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
    [SerializeField] GameObject order;
    [SerializeField] GameObject orderRow;
    [SerializeField] GameObject orderFooter;

    [Header("UI Object References")]
    [SerializeField] GameObject[] screens;
    [SerializeField] GameObject[] menuGridLayouts;
    [SerializeField] GameObject navbarCanvas;
    [SerializeField] TextMeshProUGUI[] navbarTexts;
    [SerializeField] RawImage[] navbarImages;
    [SerializeField] TMP_InputField signInEmailField;
    [SerializeField] TMP_InputField signInPasswordField;
    [SerializeField] TMP_InputField signUpNameField;
    [SerializeField] TMP_InputField signUpEmailField;
    [SerializeField] TMP_InputField signUpAddressField;
    [SerializeField] TMP_Dropdown signUpPaymentDropdown;
    [SerializeField] TMP_InputField signUpPasswordField;
    [SerializeField] TMP_InputField signUpConfirmPasswordField;
    [SerializeField] TMP_InputField searchInputField;
    [SerializeField] GameObject clearSearchButton;
    [SerializeField] TextMeshProUGUI shoppingCartTotalText;
    [SerializeField] Transform shoppingCartContent;
    [SerializeField] TextMeshProUGUI checkoutTotalText;
    [SerializeField] Transform checkoutContent;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemRangeText;
    [SerializeField] TextMeshProUGUI itemDescriptionText;
    [SerializeField] TextMeshProUGUI itemPriceText;
    [SerializeField] TextMeshProUGUI itemEnergyText;
    [SerializeField] RawImage itemImage;
    [SerializeField] TextMeshProUGUI accountNameText;
    [SerializeField] TextMeshProUGUI accountEmailText;
    [SerializeField] TextMeshProUGUI accountAddressText;
    [SerializeField] TextMeshProUGUI accountPaymentText;

    [Header("Textures")]
    [SerializeField] Texture2D emptyTexture;

    private int currentScreenIndex = -1;
    private int currentItemId = -1;
    private int currentUserId = -1;
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
        // Validate the user's input
        if (string.IsNullOrWhiteSpace(signInEmailField.text))
        {
            Debug.LogWarning("Email is required");
            return;
        }

        if (string.IsNullOrWhiteSpace(signInPasswordField.text))
        {
            Debug.LogWarning("Password is required");
            return;
        }

        // Verify that the user's details are correct
        myQuery = "SELECT * FROM user;";
        RunMyQuery();
        // Loop through all of the users in the DB
        while (DB.reader.Read())
        {
            // If the email and password matches
            if (DB.reader.GetString(3) == signInEmailField.text
             && DB.reader.GetString(4) == signInPasswordField.text)
            {
                currentUserId = DB.reader.GetInt32(0);
                Debug.Log("Welcome, " + DB.reader.GetString(1));
                ClearSignInAndSignUp();
                LoadMenu();
                MenuGoTo(2);
                HighlightNavButton(0);
                DB.CloseDB();
                return;
            }
        }
        DB.CloseDB();
        Debug.LogWarning("Email or Password is incorrect");
    }

    public void SignUpButton()
    {
        // Validate the user's input
        if (string.IsNullOrWhiteSpace(signUpNameField.text))
        {
            Debug.LogWarning("Name is a required field");
            return;
        }

        if (string.IsNullOrWhiteSpace(signUpEmailField.text))
        {
            Debug.LogWarning("Email is a required field");
            return;
        }

        if (string.IsNullOrWhiteSpace(signUpAddressField.text))
        {
            signUpAddressField.text = "";
        }

        if (string.IsNullOrWhiteSpace(signUpPasswordField.text))
        {
            Debug.LogWarning("Password is a required field");
            return;
        }

        if (string.IsNullOrWhiteSpace(signUpConfirmPasswordField.text))
        {
            Debug.LogWarning("Password confirmation is required");
            return;
        }

        // Ensure that another user with the same email does not already exist
        myQuery = "SELECT Email FROM user;";
        RunMyQuery();
        // Loop through all of the users in the DB
        while (DB.reader.Read())
        {
            if (DB.reader.GetString(0) == signUpEmailField.text)
            {
                Debug.LogWarning("Account already exists with " + signUpEmailField.text);
                DB.CloseDB();
                return;
            }
        }
        DB.CloseDB();

        // Check if the password is 8 characters
        if (signUpPasswordField.text.Length < 8)
        {
            Debug.LogWarning("Password must be at least 8 characters");
            return;
        }

        // Check if the password and confirmation match
        if (signUpPasswordField.text != signUpConfirmPasswordField.text)
        {
            Debug.LogWarning("Password's do not match");
            return;
        }

        // Insert a new user into the DB
        myQuery = "INSERT INTO user (Name,Address,Email,Password,PaymentMethod) " +
                  "VALUES('" + signUpNameField.text + "','" + signUpAddressField.text
                  + "','" + signUpEmailField.text + "','" + signUpPasswordField.text
                  + "','" + signUpPaymentDropdown.options[signUpPaymentDropdown.value].text
                  + "');";

        RunMyQuery();
        DB.CloseDB();

        // Get the ID of the new user
        myQuery = "SELECT MAX(ID) FROM user;";
        RunMyQuery();
        if (DB.reader.Read())
        {
            currentUserId = DB.reader.GetInt32(0);
        }
        DB.CloseDB();

        // Load the menu screen
        LoadMenu();
        MenuGoTo(2);
        HighlightNavButton(0);
        Debug.Log("Welcome, " + signUpNameField.text);
        ClearSignInAndSignUp();
    }

    public void LogoutButton()
    {
        // Reset user ID back to default
        currentUserId = -1;
        // Go to the login screen
        MenuGoTo(0);
    }

    public void ClearSignInAndSignUp()
    {
        searchInputField.text = "";
        signInEmailField.text = "";
        signInPasswordField.text = "";
        signUpNameField.text = "";
        signUpEmailField.text = "";
        signUpAddressField.text = "";
        signUpPaymentDropdown.value = 0;
        signUpPasswordField.text = "";
        signUpConfirmPasswordField.text = "";
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

    public void RemoveFromCart(int itemId)
    {
        // Find the index of the item
        foreach (int[] item in shoppingCart)
        {
            if (item[0] == itemId)
            {
                shoppingCart.Remove(item);
                break;
            }
        }

        shoppingCartTotalText.text = CalculateTotal().ToString("C");
    }

    public void UpdateItemQuantity(int itemId, int newQuantity)
    {
        foreach (int[] item in shoppingCart)
        {
            if (item[0] == itemId)
            {
                item[1] = newQuantity;
                shoppingCartTotalText.text = CalculateTotal().ToString("C");
                break;
            }
        }
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

    public void LoadMenu()
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

    public void LoadItem(int itemId)
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
        // Destroy all of the cart item panels
        foreach (Transform cartItem in shoppingCartContent)
        {
            Destroy(cartItem.gameObject);
        }

        foreach (int[] item in shoppingCart)
        {
            // Instantiate a cart item panel
            GameObject newItem = Instantiate(cartItem, shoppingCartContent);

            // Store reference to CartItem Component
            CartItem newCartItem = newItem.GetComponent<CartItem>();

            // Set the ID on the cart item
            newCartItem.SetItemId(item[0]);

            // Get the details of the item
            myQuery = "SELECT * FROM menu WHERE ID = " + item[0] + ";";

            RunMyQuery();

            if (DB.reader.Read())
            {
                // Fill in the details of the item
                newCartItem.FillDetails(DB.reader.GetString(1),
                                        DB.reader.GetFloat(5),
                                        DB.reader.GetString(6),
                                        item[1]);
            }

            DB.CloseDB();
        }

        shoppingCartTotalText.text = CalculateTotal().ToString("C");
    }

    public void LoadCheckout()
    {
        // Remove all of the current rows in the checkout
        foreach (Transform orderRow in checkoutContent)
        {
            Destroy(orderRow.gameObject);
        }

        checkoutTotalText.text = CalculateTotal().ToString("C");

        // Loop through every item in the shopping cart
        foreach (int[] item in shoppingCart)
        {
            // Instantiate a new row
            GameObject newOrderRow = Instantiate(orderRow, checkoutContent);

            // Get the name and price of the item
            myQuery = "SELECT Name, Price FROM menu WHERE ID = " + item[0] + ";";

            RunMyQuery();

            if (DB.reader.Read())
            {
                // Display the quantity, name, and subtotal of the item
                newOrderRow.GetComponent<OrderRow>().FillDetails
                (
                    item[1], DB.reader.GetString(0), DB.reader.GetFloat(1)
                );
            }

            DB.CloseDB();
        }
    }

    public void LoadAccount()
    {
        // SELECT all of the user's details
        myQuery = "SELECT * FROM user WHERE ID = " + currentUserId + ";";
        RunMyQuery();
        if (DB.reader.Read())
        {
            accountNameText.text = DB.reader.GetString(1);
            accountEmailText.text = DB.reader.GetString(3);
            accountAddressText.text = DB.reader.GetString(2);
            accountPaymentText.text = DB.reader.GetString(5);
        }
        DB.CloseDB();
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
    public void OnSearchSubmit()
    {
        // Reload the menu screen
        LoadMenu();
    }

    public void OnSearchChange()
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
    }

    public void ClearSearch()
    {
        searchInputField.text = "";
        LoadMenu();
    }

    private float CalculateTotal()
    {
        float total = 0.0f;

        foreach (int[] item in shoppingCart)
        {
            // GET the price of the item
            myQuery = "SELECT Price FROM menu WHERE ID = " + item[0] + ";";

            RunMyQuery();

            if (DB.reader.Read())
            {
                // Add the quantity * price of the item to the total
                total += item[1] * DB.reader.GetFloat(0);
            }

            DB.CloseDB();
        }

        return total;
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
