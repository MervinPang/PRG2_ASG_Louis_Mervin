using PRG2_ASG_Louis_Mervin;

//==========================================================
// Student Number : S10268761B (Basic Feature 1, 4, 6, 8)
// Student Name   : Louis Ng Bo Lin
// Partner Name   : Pang Wy Zen Mervin
//==========================================================

// Checkpoint 1 - Implemented Features:
// - Load restaurants from restaurants.csv and create Restaurant objects
// - Load orders from fooditems.csv
// - Create FoodItem objects and assign them to their respective restaurants

//==========================================================
// Student Number : S10275479D (Basic Feature 2, 3, 5, 7)
// Student Name   : Pang Wy Zen Mervin
// Partner Name   : Louis Ng Bo Lin
//==========================================================

// Checkpoint 1 - Implemented Features:
// - Load customers from customers.csv and create Customer objects
// - Load orders from orders.csv
// - Create Order objects and place them into: Restaurant’s Order Queue & Customer’s Order List

// Stores all Restaurant objects loaded from restaurants.csv
List<Restaurant> restaurantList = new List<Restaurant>();

// Stores all Customer objects loaded from customers.csv
List<Customer> customerList = new List<Customer>();

// Stack to store refunded orders
Stack<Order> refundStack = new Stack<Order>();

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

    try
    {
        option = Convert.ToInt32(Console.ReadLine());
    }
    catch
    {
        Console.WriteLine("Invalid option! Please enter a number.");
        continue; // Prevents second invalid message
    }

    if (option == 1)
    {
        Console.WriteLine();
        DisplayRestaurantsAndMenuItems(restaurantList);
    }
    else if (option == 2)
    {
        DisplayOrders(customerList, restaurantList);
    }
    else if (option == 3)
    {

    }
    else if (option == 4)
    {
        ProcessOrder(restaurantList, customerList, refundStack);
    }
    else if (option == 5)
    {

    }
    else if (option == 6)
    {

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
        string line;
        sr.ReadLine(); // skip header row

        while ((line = sr.ReadLine()) != null)
        {
            string[] parts = line.Split(',');

            // Validation
            if (parts.Length < 4)
            {
                continue;
            }

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

            // Read and convert values (with basic error handling)
            try
            {
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
            catch
            {
                // If the row has invalid date/number format, skip it to prevent crashing
                continue;
            }
        }
    }

    Console.WriteLine(i + " orders loaded!");
    Console.WriteLine();
}

// Basic Feature 3: List all restaurants and menu items
// Displays each restaurant's details followed by all food items (name, description, price)
void DisplayRestaurantsAndMenuItems(List<Restaurant> restaurantList)
{
    // Title header
    Console.WriteLine("All Restaurants and Menu Items");
    Console.WriteLine("==============================");

    // Loop through each menu in the restaurant
    foreach (Restaurant r in restaurantList)
    {
        // Display restaurant name and Id
        Console.WriteLine("Restaurant: " + r.RestaurantName + " (" + r.RestaurantId + ")");

        // Loop through each menu in the restaurant
        foreach (Menu m in r.MenuList)
        {
            // Loop through each food item in the menu
            foreach (FoodItem fi in m.FoodItemList)
            {
                Console.WriteLine("  - " + fi.ItemName + ": " + fi.ItemDesc + " - $" + fi.ItemPrice.ToString("0.00"));
            }
        }

        Console.WriteLine();
    }
}

// Basic Feature 4: List all orders with basic information
// Displays all orders from all restaurants and customers
// Sorts customer orders by order ID in ascending order
void DisplayOrders(List<Customer> customerList, List<Restaurant> restaurantList)
{
    // Collect all orders
    List<Order> orders = new List<Order>();
    foreach (Restaurant r in restaurantList)
    {
        foreach (Order o in r.OrderQueue)
        {
            orders.Add(o);
        }
    }

    // Sort by order ID by ascending
    orders.Sort((a, b) => a.OrderId.CompareTo(b.OrderId));

    Console.WriteLine("\nAll Orders");
    Console.WriteLine("==========");
    Console.WriteLine("{0,-10} {1,-15} {2,-15} {3,-20} {4,-10} {5}", "Order ID", "Customer", "Restaurant", "Delivery Date/Time", "Amount", "Status");
    Console.WriteLine("{0,-10} {1,-15} {2,-15} {3,-20} {4,-10} {5}", "--------", "----------", "-------------", "------------------", "------", "---------");

    // Display orders
    foreach (Order o in orders)
    {
        string customerName = "";
        string restaurantName = "";

        foreach (Customer c in customerList)
        {
            if (c.OrderList.Contains(o))
            {
                customerName = c.CustomerName;
                break;
            }
        }

        foreach (Restaurant r in restaurantList)
        {
            if (r.OrderQueue.Contains(o))
            {
                restaurantName = r.RestaurantName;
                break;
            }
        }

        Console.WriteLine(
            "{0,-10} {1,-15} {2,-15} {3,-20} ${4,-9:F2} {5}",
            o.OrderId,
            customerName,
            restaurantName,
            o.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm"),
            o.OrderTotal,
            o.OrderStatus
        );
    }
}

// Basic Feature 6: Process an order
// Prompts user to enter a restaurant ID, then displays the orders in that restaurant's queue in order
// For each order, prompts user to confirm, reject, skip or deliver the order
void ProcessOrder(List<Restaurant> restaurantList, List<Customer> customerList, Stack<Order> refundStack)
{
    Console.WriteLine("\nProcess Order");
    Console.WriteLine("=============");
    Console.Write("Enter Restaurant ID: ");
    string restaurantId = Console.ReadLine().Trim();

    // Find restaurant
    Restaurant selectedRestaurant = null;
    foreach (Restaurant r in restaurantList)
    {
        if (r.RestaurantId == restaurantId)
        {
            selectedRestaurant = r;
            break;
        }
    }

    if (selectedRestaurant == null)
    {
        Console.WriteLine("Restaurant not found");
        return;
    }

    if (selectedRestaurant.OrderQueue.Count == 0)
    {
        Console.WriteLine("No orders in this restaurant's queue");
        return;
    }

    int count = selectedRestaurant.OrderQueue.Count;

    // Loop through queue in order
    for (int i = 0; i < count; i++)
    {
        Order order = selectedRestaurant.OrderQueue.Dequeue();

        // Find customer name
        string customerName = "";
        foreach (Customer c in customerList)
        {
            if (c.OrderList.Contains(order))
            {
                customerName = c.CustomerName;
                break;
            }
        }

        // Display basic order info
        Console.WriteLine($"\nOrder {order.OrderId}:");
        Console.WriteLine($"Customer: {customerName}");

        Console.WriteLine("Ordered Items:");
        if (order.OrderedFoodItemList != null && order.OrderedFoodItemList.Count > 0)
        {
            int itemNo = 1;
            foreach (OrderedFoodItem of in order.OrderedFoodItemList)
            {
                Console.WriteLine($"{itemNo}. {of.ItemName} - {of.QtyOrdered}");
                itemNo++;
            }
        }
        else
        {
            Console.WriteLine("Items not loaded yet");
        }

        Console.WriteLine($"Delivery date/time: {order.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm")}");
        Console.WriteLine($"Total Amount: ${order.OrderTotal.ToString("0.00")}");
        Console.WriteLine($"Order Status: {order.OrderStatus}");

        // Prompt the user
        Console.Write("[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
        string choice = Console.ReadLine().Trim().ToUpper();

        if (choice == "C")
        {
            if (order.OrderStatus == "Pending")
            {
                order.OrderStatus = "Preparing";
                Console.WriteLine($"Order {order.OrderId} confirmed. Status: Preparing");
            }
            else
            {
                Console.WriteLine("Order pending");
            }
        }
        else if (choice == "R")
        {
            if (order.OrderStatus == "Pending")
            {
                order.OrderStatus = "Rejected";
                refundStack.Push(order);
                Console.WriteLine($"Order {order.OrderId} rejected. Refund of ${order.OrderTotal:0.00} processed");
            }
            else
            {
                Console.WriteLine("Order pending");
            }
        }
        else if (choice == "S")
        {
            if (order.OrderStatus == "Cancelled")
            {
                Console.WriteLine($"Order {order.OrderId} skipped. Status: Skipped");
            }
            else
            {
                Console.WriteLine("Order is already cancelled");
            }
        }
        else if (choice == "D")
        {
            if (order.OrderStatus == "Preparing")
            {
                order.OrderStatus = "Delivered";
                Console.WriteLine($"Order {order.OrderId} delivered. Status: Delivered");
            }
            else
            {
                Console.WriteLine("Order is preparing");
            }
        }
        else
        {
            Console.WriteLine("Invalid choice");
        }

        selectedRestaurant.OrderQueue.Enqueue(order);
    }
}