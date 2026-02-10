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
        Console.WriteLine();
        CreateNewOrder(customerList, restaurantList);
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
            if (parts.Length < 10)
            {
                continue;
            }

            // Read and convert values (with basic error handling)
            try
            {
                int orderId = Convert.ToInt32(parts[0].Trim());
                string customerEmail = parts[1].Trim();
                string restaurantId = parts[2].Trim();

                string deliveryDate = parts[3].Trim();
                string deliveryTime = parts[4].Trim();

                // Address might be quoted
                string deliveryAddress = parts[5].Trim().Trim('"');
                DateTime orderDateTime = Convert.ToDateTime(parts[6].Trim());
                double orderTotal = Convert.ToDouble(parts[7].Trim());
                string orderStatus = parts[8].Trim();

                string item = parts[9].Trim().Trim('"');

                DateTime deliveryDateTime = Convert.ToDateTime(deliveryDate + " " + deliveryTime);

                // Create Order object using data loaded
                Order o = new Order(orderId, orderDateTime, deliveryDateTime, deliveryAddress, orderStatus,
                    "", false, orderTotal);

                if (item != "")
                {
                    string[] itemParts = item.Split('|');

                    Restaurant foundRestaurant = null;
                    foreach (Restaurant r in restaurantList)
                    {
                        if (r.RestaurantId == restaurantId)
                        {
                            foundRestaurant = r;
                            break;
                        }
                    }

                    foreach (string items in itemParts)
                    {
                        if (items == "")
                        {
                            continue;
                        }

                        string[] pair = items.Split(',');
                        if (pair.Length < 2)
                        {
                            continue;
                        }

                        string itemName = pair[0].Trim();
                        int qty = 0;

                        try
                        {
                            qty = Convert.ToInt32(pair[1].Trim());
                        }
                        catch
                        {
                            continue;
                        }

                        // Find the FoodItem details (price/desc) from restaurant menu
                        string desc = "";
                        double price = 0;

                        if (foundRestaurant != null && foundRestaurant.MenuList.Count > 0)
                        {
                            foreach (FoodItem fi in foundRestaurant.MenuList[0].FoodItemList)
                            {
                                if (fi.ItemName.ToLower() == itemName.ToLower())
                                {
                                    desc = fi.ItemDesc;
                                    price = fi.ItemPrice;
                                    break;
                                }
                            }
                        }

                        // Even if not found, store the item with price 0 so list exists
                        OrderedFoodItem of = new OrderedFoodItem(itemName, desc, price, "", qty);
                        o.AddOrderedFoodItem(of);
                    }
                }

                // Add order to Customer's Order List
                foreach (Customer c in customerList)
                {
                    if (c.EmailAddress == customerEmail)
                    {
                        c.AddOrder(o);
                        break;
                    }
                }

                // Add order to Restaurant’s Order Queue
                foreach (Restaurant r in restaurantList)
                {
                    if (r.RestaurantId == restaurantId)
                    {
                        r.OrderQueue.Enqueue(o);
                        break;
                    }
                }

                i++; // Increment order count
            }
            catch
            {
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
    Console.WriteLine();
}

// Basic Feature 5: Create a New Order
// Allows user to:
// Select customer and restaurant
// Choose delivery date/time and address
// Select food items and quantities
// Add special request
// Choose payment method
// Save order to memory and orders.csv

/* Generates the next available Order ID by finding the
   maximum existing OrderId among all customers orders
   and returns maxId + 1 */
int GetNextOrderId(List<Customer> customerList)
{
    int maxId = 0;

    // Loop through all customers
    foreach (Customer c in customerList)
    {
        // Loop through each customer's orders
        foreach (Order o in c.OrderList)
        {
            if (o.OrderId > maxId)
            {
                maxId = o.OrderId;
            }
        }
    }

    return maxId + 1;
}

void CreateNewOrder(List<Customer> customerList, List<Restaurant> restaurantList)
{
    Console.WriteLine("Create New Order");
    Console.WriteLine("================");

    // Select Customer by Email
    Customer selectedCustomer = null;
    while (selectedCustomer == null)
    {
        Console.Write("Enter Customer Email: ");
        string email = Console.ReadLine();

        if (email == null)
        {
            Console.WriteLine("Customer not found. Please enter a valid email.");
            continue;
        }

        email = email.Trim();
        if (email == "")
        {
            Console.WriteLine("Customer not found. Please enter a valid email.");
            continue;
        }

        // Find matching customer (case-insensitive)
        foreach (Customer c in customerList)
        {
            if (c.EmailAddress.ToLower() == email.ToLower())
            {
                selectedCustomer = c;
                break;
            }
        }

        if (selectedCustomer == null)
        {
            Console.WriteLine("Customer not found. Please enter a valid email.");
        }
    }

    // Select Restaurant by ID
    Restaurant selectedRestaurant = null;
    while (selectedRestaurant == null)
    {
        Console.Write("Enter Restaurant ID: ");
        string restId = Console.ReadLine();

        if (restId == null)
        {
            Console.WriteLine("Restaurant ID cannot be empty.");
            continue;
        }

        restId = restId.Trim();
        if (restId == "")
        {
            Console.WriteLine("Restaurant ID cannot be empty.");
            continue;
        }

        // Find matching restaurant (case-insensitive)
        foreach (Restaurant r in restaurantList)
        {
            if (r.RestaurantId.ToLower() == restId.ToLower())
            {
                selectedRestaurant = r;
                break;
            }
        }

        if (selectedRestaurant == null)
        {
            Console.WriteLine("Restaurant not found. Please enter a valid Restaurant ID.");
        }
    }

    // Input Delivery Date
    DateTime deliveryDate;
    while (true)
    {
        Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
        string date = Console.ReadLine();

        if (date == null)
        {
            Console.WriteLine("Invalid date. Please enter in dd/mm/yyyy.");
            continue;
        }

        date = date.Trim();
        if (date == "")
        {
            Console.WriteLine("Invalid date. Please enter in dd/mm/yyyy.");
            continue;
        }

        // Format validation
        string[] parts = date.Split('/');
        if (parts.Length != 3)
        {
            Console.WriteLine("Invalid date. Please enter in dd/mm/yyyy.");
            continue;
        }

        try
        {
            deliveryDate = Convert.ToDateTime(date);
            break;
        }
        catch
        {
            Console.WriteLine("Invalid date. Please enter in dd/mm/yyyy");
        }
    }

    // Input Delivery Time
    TimeSpan deliveryTime;
    while (true)
    {
        Console.Write("Enter Delivery Time (hh:mm): ");
        string time = Console.ReadLine();

        if (time == null)
        {
            Console.WriteLine("Invalid time. Please enter hh:mm.");
            continue;
        }

        time = time.Trim();
        if (time == "")
        {
            Console.WriteLine("Invalid time. Please enter hh:mm.");
            continue;
        }

        // Format validation
        string[] tparts = time.Split(':');
        if (tparts.Length != 2)
        {
            Console.WriteLine("Invalid time. Please enter hh:mm.");
            continue;
        }

        try
        {
            deliveryTime = TimeSpan.Parse(time);
            break;
        }
        catch
        {
            Console.WriteLine("Invalid time. Please enter hh:mm.");
        }
    }

    DateTime deliveryDateTime = deliveryDate.Date + deliveryTime;

    // Input Delivery Address
    string deliveryAddress = "";
    while (deliveryAddress == "")
    {
        Console.Write("Enter Delivery Address: ");
        string address = Console.ReadLine();

        if (address == null)
        {
            Console.WriteLine("Delivery Address cannot be empty.");
            continue;
        }

        address = address.Trim();
        if (address == "")
        {
            Console.WriteLine("Delivery Address cannot be empty.");
            continue;
        }

        deliveryAddress = address;
    }

    // Validate Menu Availability
    if (selectedRestaurant.MenuList.Count == 0 ||
        selectedRestaurant.MenuList[0].FoodItemList.Count == 0)
    {
        Console.WriteLine("No food items found for this restaurant.");
        return;
    }

    List<FoodItem> menuItems = selectedRestaurant.MenuList[0].FoodItemList;

    // Display Menu Items
    Console.WriteLine("\nAvailable Food Items:");
    for (int i = 0; i < menuItems.Count; i++)
    {
        Console.WriteLine((i + 1) + ". " + menuItems[i].ItemName + " - $" + menuItems[i].ItemPrice.ToString("0.00"));
    }

    // Create Order Object
    Order newOrder = new Order();
    newOrder.DeliveryDateTime = deliveryDateTime;
    newOrder.DeliveryAddress = deliveryAddress;

    // Select Food Items
    while (true)
    {
        int itemNo;
        Console.Write("Enter item number (0 to finish): ");
        try
        {
            itemNo = Convert.ToInt32(Console.ReadLine());
        }
        catch
        {
            Console.WriteLine("Invalid input. Please enter a number.");
            continue;
        }

        if (itemNo == 0)
        {
            break;
        }

        if (itemNo < 1 || itemNo > menuItems.Count)
        {
            Console.WriteLine("Invalid item number. Please choose 1 to " + menuItems.Count + ".");
            continue;
        }

        int qty;
        Console.Write("Enter quantity: ");
        try
        {
            qty = Convert.ToInt32(Console.ReadLine());
        }
        catch
        {
            Console.WriteLine("Invalid quantity. Please enter a number.");
            continue;
        }

        if (qty <= 0)
        {
            Console.WriteLine("Quantity must be at least 1.");
            continue;
        }

        FoodItem fi = menuItems[itemNo - 1];
        OrderedFoodItem of = new OrderedFoodItem(fi.ItemName, fi.ItemDesc, fi.ItemPrice, "", qty);
        newOrder.AddOrderedFoodItem(of);
    }

    if (newOrder.OrderedFoodItemList.Count == 0)
    {
        Console.WriteLine("No items selected. Order creation cancelled.");
        return;
    }

    // Special Request (Optional)
    Console.Write("Add special request? [Y/N]: ");
    string special = Console.ReadLine();

    if (special == "Y" || special == "y")
    {
        string specialRequest = "";

        while (specialRequest == "")
        {
            Console.Write("Enter special request: ");
            specialRequest = Console.ReadLine();

            if (specialRequest == null || specialRequest == "")
            {
                Console.WriteLine("Special request cannot be empty.");
                specialRequest = "";
            }
        }

        foreach (OrderedFoodItem ofi in newOrder.OrderedFoodItemList)
        {
            ofi.Customise = specialRequest;
        }
    }

    // Calculate Total Cost
    double itemsTotal = newOrder.CalculateOrderTotal();
    double deliveryFee = 5.00;
    double finalTotal = itemsTotal + deliveryFee;

    newOrder.OrderTotal = finalTotal;

    Console.WriteLine("\nOrder Total: $" + itemsTotal.ToString("0.00") + " + $" +
        deliveryFee.ToString("0.00") + " (delivery) = $" + finalTotal.ToString("0.00"));

    // Payment Method
    Console.Write("Proceed to payment? [Y/N]: ");
    string pay = Console.ReadLine();

    if (special != null) special = special.Trim();

    if (pay != "Y" && pay != "y")
    {
        return;
    }

    Console.WriteLine("\nPayment method:");
    string paymentMethod = "";

    while (paymentMethod == "")
    {
        Console.Write("[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");
        string pmInput = Console.ReadLine();

        if (pmInput == null)
        {
            Console.WriteLine("Invalid method. Enter CC, PP, or CD.");
            continue;
        }

        pmInput = pmInput.Trim();

        if (pmInput == "CC" || pmInput == "cc")
        {
            paymentMethod = "CC";
        }
        else if (pmInput == "PP" || pmInput == "pp")
        {
            paymentMethod = "PP";
        }
        else if (pmInput == "CD" || pmInput == "cd")
        {
            paymentMethod = "CD";
        }
        else
        {
            Console.WriteLine("Invalid method. Enter CC, PP, or CD.");
        }
    }

    // Finalise Order
    int newOrderId = GetNextOrderId(customerList);

    newOrder.OrderId = newOrderId;
    newOrder.OrderDateTime = DateTime.Now;
    newOrder.OrderStatus = "Pending";
    newOrder.OrderPaymentMethod = paymentMethod;
    newOrder.OrderPaid = false;

    // Add to restaurant queue and customer list
    selectedRestaurant.OrderQueue.Enqueue(newOrder);
    selectedCustomer.AddOrder(newOrder);

    // Append Order to CSV
    string itemsString = "";
    foreach (OrderedFoodItem item in newOrder.OrderedFoodItemList)
    {
        if (itemsString.Length > 0)
            itemsString += "|";

        itemsString += $"{item.ItemName},{item.QtyOrdered}";
    }

    string orderLine =
        $"{newOrder.OrderId},{selectedCustomer.EmailAddress},{selectedRestaurant.RestaurantId}," +
        $"{newOrder.DeliveryDateTime:dd/MM/yyyy},{newOrder.DeliveryDateTime:HH:mm}," +
        $"\"{newOrder.DeliveryAddress}\"," +
        $"{newOrder.OrderDateTime:dd/MM/yyyy HH:mm}," +
        $"{newOrder.OrderTotal:0.00},{newOrder.OrderStatus}," +
        $"\"{itemsString}\"";

    using (StreamWriter sw = new StreamWriter("orders.csv", true))
    {
        sw.WriteLine(orderLine);
    }

    Console.WriteLine("\nOrder " + newOrderId + " created successfully! Status: Pending");
    Console.WriteLine();
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