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
LoadOrders(customerList, restaurantList);

// -------------------- MAIN MENU LOOP --------------------

int option = -1;
while (option != 0)
{
    DisplayMenu();
    option = Convert.ToInt32(Console.ReadLine());

    if (option == 1)
    {
        
    }
    else if (option == 2)
    {
        DisplayOrders(customerList, restaurantList);
    }
    else if (option == 0)
    {
        Console.WriteLine("Goodbye!");
    }
    else
    {
        Console.WriteLine("Invalid option! Please try again.");
    }
}

// ----------------------- METHODS -----------------------

// Displays the main menu options
void DisplayMenu()
{
    Console.WriteLine();
    Console.WriteLine("===== Gruberoo Food Delivery System =====");
    Console.WriteLine("1. List all restaurants and menu items");
    Console.WriteLine("2. List all orders");
    Console.WriteLine("3. Create a new order");
    Console.WriteLine("4. Process an order");
    Console.WriteLine("5. Modify an existing order");
    Console.WriteLine("6. Delete an existing order");
    Console.WriteLine("0. Exit");
    Console.Write("Enter your choice: ");
}

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

// Basic Feature 2: Load orders from orders.csv
// Creates Order objects based on the data loaded
// Places each Order into the corresponding Restaurant's Order Queue & the corresponding Customer's Order List
void LoadOrders(List<Customer> customerList, List<Restaurant> restaurantList)
{
    int i = 0; // Counter for successfully loaded orders

    using (StreamReader sr = new StreamReader("orders.csv"))
    {
        string line;

        // Skip header row
        sr.ReadLine();

        while ((line = sr.ReadLine()) != null)
        {
            string[] parts = line.Split(',');

            // Validation (Skip invalid rows)
            if (parts.Length < 9)
            {
                continue;
            }

            int orderId = Convert.ToInt32(parts[0].Trim());
            string customerEmail = parts[1].Trim();
            string restaurantId = parts[2].Trim();
            string deliveryAddress = parts[5].Trim();
            DateTime orderDateTime = Convert.ToDateTime(parts[6].Trim());
            DateTime deliveryDateTime = Convert.ToDateTime(parts[3].Trim() + " " + parts[4].Trim());
            double orderTotal = Convert.ToDouble(parts[7].Trim());
            string orderStatus = parts[8].Trim();

            // Create Order object using data loaded
            Order o = new Order(orderId, orderDateTime, deliveryDateTime, deliveryAddress, orderStatus,
                "", false, orderTotal);

            // Add order to the corresponding Customer's Order List
            foreach (Customer customer in customerList)
            {
                if (customer.EmailAddress == customerEmail)
                {
                    customer.AddOrder(o);
                    break;
                }
            }

            // Add order to the corresponding Restaurant’s Order Queue
            foreach (Restaurant restaurant in restaurantList)
            {
                if (restaurant.RestaurantId == restaurantId)
                {
                    restaurant.OrderQueue.Enqueue(o);
                    break;
                }
            }

            i++; // Increment order count
        }
    }

    Console.WriteLine(i + " orders loaded!");
}

// Basic Feature 2: List all orders with basic information
void DisplayOrders(List<Customer> customerList, List<Restaurant> restaurantList)
{
    Console.WriteLine("\nAll Orders");
    Console.WriteLine("==========");
    Console.WriteLine("\n{0,-10} {1,-15} {2,-18} {3,-20} {4,-10} {5}", "Order ID", "Customer", "Restaurant", "Delivery Date/Time", "Amount", "Status");
    Console.WriteLine("-------------------------------------------------------------------------------------");

    // Loop through each restaurant's order queue
    foreach (Restaurant restaurant in restaurantList)
    {
        foreach (Order order in restaurant.OrderQueue)
        {
            string customerName = "";

            // Find the customer who placed this order
            foreach (Customer customer in customerList)
            {
                if (customer.OrderList.Contains(order))
                {
                    customerName = customer.CustomerName;
                    break;
                }
            }

            // Display order details in formatted row
            Console.WriteLine("{0,-10} {1,-15} {2,-18} {3,-20} ${4,-9:F2} {5}", order.OrderId, customerName, restaurant.RestaurantName,
                order.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm"), order.OrderTotal, order.OrderStatus);
        }
    }
}