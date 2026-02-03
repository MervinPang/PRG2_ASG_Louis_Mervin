using PRG2_ASG_Louis_Mervin;

//==========================================================
// Student Number : S10275479D (Basic Feature 2)
// Student Name   : Pang Wy Zen Mervin
// Partner Name   : Louis Ng Bo Lin
//==========================================================
//
// Checkpoint 1 - Implemented Features:
// - Load customers from customers.csv and create Customer objects
// - Load orders from orders.csv
// - Create Order objects and place them into: Restaurant’s Order Queue & Customer’s Order List
// - Display customer list in formatted table
// - Display all orders with basic information

// Stores all Restaurant objects loaded from restaurants.csv
List<Restaurant> restaurantList = new List<Restaurant>();

// Stores all Customer objects loaded from customers.csv
List<Customer> customerList = new List<Customer>();

Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
LoadRestaurants(restaurantList);
LoadFoodItems(restaurantList);  
LoadCustomers(customerList);
DisplayCustomers(customerList);

// --------------- DISPLAY METHODS ---------------

// Basic Feature 1: Load restaurants from restaurants.csv
// Creates Restaurant objects based on the data loaded and stores them into restaurantList
void LoadRestaurants(List<Restaurant> restaurantList)
{
    int i = 0;

    using (StreamReader sr = new StreamReader("restaurants.csv"))
    {
        string line;
        sr.ReadLine();

        while ((line = sr.ReadLine()) != null)
        {
            string[] parts = line.Split(',');

            if (parts.Length < 3)
            {
                continue;
            }

            string id = parts[0].Trim();
            string name = parts[1].Trim();
            string email = parts[2].Trim();

            Restaurant restaurant = new Restaurant(id, name, email);
            restaurantList.Add(restaurant);
            i++;
        }
    }

    Console.WriteLine(i + " restaurants loaded!");
}

// Basic Feature 1: Load food items from fooditems.csv
// Creates FoodItem objects and assigns them to their respective restaurants
void LoadFoodItems(List<Restaurant> restaurantList)
{
    int i = 0;
    using (StreamReader sr = new StreamReader("fooditems.csv"))
    {
        sr.ReadLine(); // skip header row
        string line;

        while ((line = sr.ReadLine()) != null)
        {
            string[] parts = line.Split(',');

            string restaurantId = parts[0].Trim();
            string itemName = parts[1].Trim();
            string itemDesc = parts[2].Trim();
            double itemPrice = Convert.ToDouble(parts[3].Trim());

            FoodItem foodItem = new FoodItem(itemName, itemDesc, itemPrice, "");

            foreach (Restaurant r in restaurantList) // assign to correct restaurant
            {
                if (r.RestaurantId == restaurantId)
                {
                    if (r.MenuList.Count == 0)
                    {
                        r.MenuList.Add(new Menu(restaurantId + "M1", "Main Menu"));
                    }

                    r.MenuList[0].AddFoodItem(foodItem);
                    i++;
                    break;
                }
            }
        }
    }

    Console.WriteLine(i + " food items loaded!");
}

// Displays all customers in a formatted table
void DisplayCustomers(List<Customer> customerList)
{
    Console.WriteLine("\nCustomer List");
    Console.WriteLine("================");
    Console.WriteLine("\n{0,-5} {1,-20} {2}", "No.", "Customer Name", "Email Address");
    Console.WriteLine("{0,-5} {1,-20} {2}", "---", "-------------", "-------------");

    int i = 1;
    foreach (Customer customer in customerList)
    {
        Console.WriteLine("{0,-5} {1,-20} {2}", i, customer.CustomerName, customer.EmailAddress);
        i++;
    }
}

// Basic Feature 2: Load customers from customers.csv
// Creates Customer objects based on the data loaded and stores them into customerList
void LoadCustomers(List<Customer> customerList)
{
    int i = 0;

    using (StreamReader sr = new StreamReader("customers.csv"))
    {
        string line;

        // Skip header row
        sr.ReadLine();

        // Read each customer record
        while ((line = sr.ReadLine()) != null)
        {
            string[] parts = line.Split(',');

            // Validation (Skip invalid rows)
            if (parts.Length < 2)
            {
                continue;
            }

            string name = parts[0].Trim();
            string email = parts[1].Trim();

            // Create Customer object and store in list
            Customer customer = new Customer(email, name);
            customerList.Add(customer);
            i++;
        }
    }

    // Display load confirmation
    Console.WriteLine(i + " customers loaded!");
}