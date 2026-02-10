using PRG2_ASG_Louis_Mervin;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Runtime.Intrinsics.X86;
using System.Threading.Channels;
using System.Threading.Tasks;

//===========================================================
// Student Number : S10268761B (Basic Feature 1, 4, 6, 8, A)
// Student Name   : Louis Ng Bo Lin
// Partner Name   : Pang Wy Zen Mervin
//===========================================================

// Checkpoint 1 - Implemented Features:
// - Load restaurants from restaurants.csv and create Restaurant objects
// - Load orders from fooditems.csv
// - Create FoodItem objects and assign them to their respective restaurants

// Final Submission - Implemented Features:
// - List all orders with basic information
// - Process an order
// - Delete an existing order
// - Bulk process pending orders for today

//===========================================================
// Student Number : S10275479D (Basic Feature 2, 3, 5, 7, B)
// Student Name   : Pang Wy Zen Mervin
// Partner Name   : Louis Ng Bo Lin
//===========================================================

// Checkpoint 1 - Implemented Features:
// - Load customers from customers.csv and create Customer objects
// - Load orders from orders.csv
// - Create Order objects and place them into: Restaurant’s Order Queue & Customer’s Order List

// Final Submission - Implemented Features:
// - Loaded customers and orders from CSV files
// - Created Order objects and stored them in Restaurant queues and Customer order lists
// - Displayed all restaurants and their menu items
// - Created new orders with item selection, payment and status update
// - Modified existing pending orders with validation and payment handling

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
        Console.WriteLine();
        DisplayOrders(customerList, restaurantList);
    }
    else if (option == 3)
    {
        Console.WriteLine();
        CreateNewOrder(customerList, restaurantList);
    }
    else if (option == 4)
    {
        Console.WriteLine();
        ProcessOrder(restaurantList, customerList, refundStack);
    }
    else if (option == 5)
    {
        Console.WriteLine();
        ModifyExistingOrder(customerList, restaurantList);
    }
    else if (option == 6)
    {
        Console.WriteLine();
        DeleteOrder(customerList, restaurantList, refundStack);
    }
    else if (option == 7)
    {
        Console.WriteLine();
        BulkProcessPendingOrdersToday(restaurantList, refundStack);
    }
    else if (option == 0)
    {
        SaveAllRestaurantQueues(restaurantList);
        SaveAllRestaurantStacks(restaurantList);

        Console.WriteLine("Queue and stack saved. Goodbye!");
        break;
    }
    else
    {
        Console.WriteLine("Invalid option! Please try again.");
    }
}

void SaveAllRestaurantQueues(List<Restaurant> restaurantList)
{
    using (StreamWriter sw = new StreamWriter("queue.csv"))
    {
        sw.WriteLine("RestaurantId,OrderId,OrderStatus,OrderTotal");

        foreach (Restaurant r in restaurantList)
        {
            foreach (Order o in r.OrderQueue)
            {
                sw.WriteLine(
                    r.RestaurantId + "," +
                    o.OrderId + "," +
                    o.OrderStatus + "," +
                    o.OrderTotal.ToString("0.00")
                );
            }
        }
    }
}

void SaveAllRestaurantStacks(List<Restaurant> restaurantList)
{
    using (StreamWriter sw = new StreamWriter("stack.csv"))
    {
        sw.WriteLine("RestaurantId,OrderId,OrderStatus,OrderTotal");

        foreach (Restaurant r in restaurantList)
        {
            foreach (Order o in refundStack)
            {
                sw.WriteLine(
                    r.RestaurantId + "," +
                    o.OrderId + "," +
                    o.OrderStatus + "," +
                    o.OrderTotal.ToString("0.00")
                );
            }
        }
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
    Console.WriteLine("7. Bulk process pending orders for today");
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
            string[] parts = line.Split(',', 10);

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
                string deliveryAddress = parts[5].Trim();
                DateTime orderDateTime = Convert.ToDateTime(parts[6].Trim());
                double orderTotal = Convert.ToDouble(parts[7].Trim());
                string orderStatus = parts[8].Trim();

                string item = parts[9].Trim();
                item = item.Trim('"'); // Remove the quotes

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
void DisplayOrders(List<Customer> customerList, List<Restaurant> restaurantList)
{
    Console.WriteLine("\nAll Orders");
    Console.WriteLine("==========");
    Console.WriteLine("{0,-10} {1,-15} {2,-15} {3,-20} {4,-10} {5}", "Order ID", "Customer", "Restaurant", "Delivery Date/Time", "Amount", "Status");
    Console.WriteLine("{0,-10} {1,-15} {2,-15} {3,-20} {4,-10} {5}", "--------", "----------", "-------------", "------------------", "------", "---------");

    foreach (Customer c in customerList)
    {
        foreach (Order o in c.OrderList)
        {
            // Find restaurant name
            string restaurantName = "";
            foreach (Restaurant r in restaurantList)
            {
                if (r.OrderQueue.Contains(o))
                {
                    restaurantName = r.RestaurantName;
                    break;
                }
            }
            Console.WriteLine("{0,-10} {1,-15} {2,-15} {3,-20} ${4,-9:0.00} {5}", o.OrderId, c.CustomerName, restaurantName, o.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm"), o.OrderTotal, o.OrderStatus);
        }
    }
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
        Console.Write("Enter item number (0 to finish): ");
        int itemNo;

        try
        {
            itemNo = Convert.ToInt32(Console.ReadLine());
        }
        catch
        {
            Console.WriteLine("Invalid input. Please enter a number.");
            continue;
        }

        if (itemNo == 0) break;

        if (itemNo < 1 || itemNo > menuItems.Count)
        {
            Console.WriteLine("Invalid item number. Please choose 1 to " + menuItems.Count + ".");
            continue;
        }

        Console.Write("Enter quantity: ");
        int qty;

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

    if (special != null)
    {
        special = special.Trim().ToUpper();
    }

    if (special == "Y")
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

    if (pay != null)
    {
        pay = pay.Trim().ToUpper();
    }

    if (pay != "Y")
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

        pmInput = pmInput.Trim().ToUpper();

        if (pmInput == "CC" || pmInput == "PP" || pmInput == "CD")
        {
            paymentMethod = pmInput;
        }
        else
        {
            Console.WriteLine("Invalid method. Enter CC, PP, or CD.");
        }
    }

    // Finalise Order
    newOrder.OrderId = GetNextOrderId(customerList);
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
        if (itemsString.Length > 0) itemsString += "|";
        itemsString += item.ItemName + "," + item.QtyOrdered;
    }

    string orderLine = newOrder.OrderId + "," + selectedCustomer.EmailAddress + "," + selectedRestaurant.RestaurantId + "," +
        newOrder.DeliveryDateTime.ToString("dd/MM/yyyy") + "," + newOrder.DeliveryDateTime.ToString("HH:mm") + "," +
        newOrder.DeliveryAddress + "," + newOrder.OrderDateTime.ToString("dd/MM/yyyy HH:mm") + "," +
        newOrder.OrderTotal.ToString("0.00") + "," + newOrder.OrderStatus + "," + "\"" + itemsString + "\"";

    using (StreamWriter sw = new StreamWriter("orders.csv", true))
    {
        sw.WriteLine(orderLine);
    }

    Console.WriteLine("\nOrder " + newOrder.OrderId + " created successfully! Status: Pending");
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

    // Find restaurant (use RestaurantList)
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

        // Display order info
        Console.WriteLine($"\nOrder {order.OrderId}:");
        Console.WriteLine($"Customer: {customerName}");

        Console.WriteLine("Ordered Items:");
        if (order.OrderedFoodItemList != null && order.OrderedFoodItemList.Count > 0)
        {
            order.DisplayOrderedFoodItems();
        }
        else
        {
            Console.WriteLine("Items not loaded yet");
        }

        Console.WriteLine($"Delivery date/time: {order.DeliveryDateTime:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"Total Amount: ${order.OrderTotal:0.00}");
        Console.WriteLine($"Order Status: {order.OrderStatus}");

        // Prompt
        Console.Write("[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
        string choice = Console.ReadLine().Trim().ToUpper();

        if (choice == "C")
        {
            // Confirm only if Pending
            if (order.OrderStatus == "Pending")
            {
                order.OrderStatus = "Preparing";
                Console.WriteLine($"Order {order.OrderId} confirmed. Status: Preparing");
            }
            else
            {
                Console.WriteLine("Confirmation failed");
            }
        }
        else if (choice == "R")
        {
            if (order.OrderStatus == "Pending")
            {
                order.OrderStatus = "Rejected";
                refundStack.Push(order);
                Console.WriteLine($"Order {order.OrderId} rejected. Refund of ${order.OrderTotal:0.00} processed.");
            }
            else
            {
                Console.WriteLine("Reject failed");
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
                Console.WriteLine("Skip failed");
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
                Console.WriteLine("Deliver failed");
            }
        }
        else
        {
            Console.WriteLine("Invalid choice.");
        }

        // Put the order back into the queue
        selectedRestaurant.OrderQueue.Enqueue(order);
    }
}

// Basic Feature 7: Modify an existing pending order for a customer
// Update One order row inside orders.csv by matching OrderId
// Read all lines first
// Overwrites the file and replaces only the matching order row
void UpdateOrderInCsv(string filePath, Order selectedOrder)
{
    // Read entire CSV into memory
    string[] lines = File.ReadAllLines(filePath);

    using (StreamWriter sw = new StreamWriter(filePath, false))
    {
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            // Keep header line and blank lines as-is
            if (i == 0 || line.Trim() == "")
            {
                sw.WriteLine(line);
                continue;
            }

            // Split into max 10 columns so the last column (Items) won't break due to commas
            string[] parts = line.Split(',', 10);

            int orderIdInFile;
            try
            {
                orderIdInFile = Convert.ToInt32(parts[0]);
            }
            catch
            {
                // If OrderId cannot be read (corrupt line), keep the line unchanged
                sw.WriteLine(line);
                continue;
            }

            // If not the order we are updating keep line unchanged
            if (orderIdInFile != selectedOrder.OrderId)
            {
                sw.WriteLine(line);
                continue;
            }

            parts[3] = selectedOrder.DeliveryDateTime.ToString("dd/MM/yyyy");
            parts[4] = selectedOrder.DeliveryDateTime.ToString("HH:mm");
            parts[5] = selectedOrder.DeliveryAddress;
            parts[7] = selectedOrder.OrderTotal.ToString("0.00");
            parts[9] = UpdateItemsColumn(selectedOrder);

            // Rebuild the CSV row back into a single line
            string updatedLine = "";
            for (int j = 0; j < parts.Length; j++)
            {
                updatedLine += parts[j];
                if (j < parts.Length - 1)
                {
                    updatedLine += ",";
                }
            }

            // Write the updated row
            sw.WriteLine(updatedLine);
        }
    }
}

// Build the Items column for CSV
string UpdateItemsColumn(Order order)
{
    string items = "";

    for (int i = 0; i < order.OrderedFoodItemList.Count; i++)
    {
        OrderedFoodItem it = order.OrderedFoodItemList[i];

        // Remove quotes in item names (prevents breaking CSV)
        string name = it.ItemName.Replace("\"", "");

        // Add ItemName,Qty
        items += name + "," + it.QtyOrdered;

        // Separate multiple items using '|'
        if (i < order.OrderedFoodItemList.Count - 1)
        {
            items += "|";
        }
    }

    // Wrap in quotes so excel keeps it in one cell
    return "\"" + items + "\"";
}

// Modify an existing pending order for a customer
void ModifyExistingOrder(List<Customer> customerList, List<Restaurant> restaurantList)
{
    Console.WriteLine("\nModify Order");
    Console.WriteLine("============");

    // Prompt user for Customer Email + validate (not empty + must exist)
    Customer selectedCustomer = null;
    while (selectedCustomer == null)
    {
        Console.Write("Enter Customer Email: ");
        string email = Console.ReadLine();

        if (email == null) email = "";
        email = email.Trim();

        if (email == "")
        {
            Console.WriteLine("Invalid email. Please enter a valid Customer Email.");
            continue;
        }

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
            Console.WriteLine("Customer not found. Please enter a valid Customer Email.");
        }
    }

    // Get all Pending orders for this customer
    List<Order> pendingOrders = new List<Order>();

    foreach (Order o in selectedCustomer.OrderList)
    {
        if (o.OrderStatus == "Pending")
        {
            pendingOrders.Add(o);
        }
    }

    if (pendingOrders.Count == 0)
    {
        Console.WriteLine("No Pending orders found for this customer.");
        Console.WriteLine();
        return;
    }

    // Display Pending orders
    Console.WriteLine("Pending Orders:");
    foreach (Order o in pendingOrders)
    {
        Console.WriteLine(o.OrderId);
    }

    // Prompt user for Order ID and validate (must exist in pending list)
    Order selectedOrder = null;
    while (selectedOrder == null)
    {
        Console.Write("Enter Order ID: ");
        string id = Console.ReadLine();

        if (id == null) id = "";
        id = id.Trim();

        int orderId;
        try
        {
            orderId = Convert.ToInt32(id);
        }
        catch
        {
            Console.WriteLine("Invalid Order ID. Please enter a number.");
            continue;
        }

        foreach (Order o in pendingOrders)
        {
            if (o.OrderId == orderId)
            {
                selectedOrder = o;
                break;
            }
        }

        if (selectedOrder == null)
        {
            Console.WriteLine("Invalid Order ID. Please enter an Order ID from the Pending Orders list.");
        }
    }

    // Find the restaurant that contains this order (so item changes can use the menu)
    Restaurant orderRestaurant = null;
    foreach (Restaurant r in restaurantList)
    {
        if (r.OrderQueue.Contains(selectedOrder))
        {
            orderRestaurant = r;
            break;
        }
    }

    if (orderRestaurant == null)
    {
        Console.WriteLine("Restaurant for this order could not be found. Unable to modify items.");
        Console.WriteLine();
        return;
    }

    // Display current order details (items + address + delivery time)
    Console.WriteLine("Order Items:");
    DisplayOrderItems(selectedOrder);

    Console.WriteLine("Address:");
    Console.WriteLine(selectedOrder.DeliveryAddress);

    Console.WriteLine("Delivery Date/Time:");
    Console.WriteLine(selectedOrder.DeliveryDateTime.ToString("dd/M/yyyy, HH:mm"));

    // Save old values for revert if payment is not made
    string oldAddress = selectedOrder.DeliveryAddress;
    DateTime oldDeliveryDateTime = selectedOrder.DeliveryDateTime;
    double oldTotal = selectedOrder.OrderTotal;

    // Keep copy old items for revert
    List<OrderedFoodItem> oldItems = new List<OrderedFoodItem>();
    foreach (OrderedFoodItem item in selectedOrder.OrderedFoodItemList)
    {
        OrderedFoodItem copy = new OrderedFoodItem(item.ItemName, item.ItemDesc, item.ItemPrice, item.Customise, item.QtyOrdered);
        oldItems.Add(copy);
    }

    // Prompt user to choose update type and validate 1 to 3
    int modChoice = 0;
    while (modChoice < 1 || modChoice > 3)
    {
        Console.Write("\nModify: [1] Items [2] Address [3] Delivery Time: ");
        string choice = Console.ReadLine();

        if (choice == null) choice = "";
        choice = choice.Trim();

        try
        {
            modChoice = Convert.ToInt32(choice);
        }
        catch
        {
            Console.WriteLine("Invalid choice. Enter 1, 2, or 3.");
            modChoice = 0;
        }

        if (modChoice < 1 || modChoice > 3)
        {
            Console.WriteLine("Invalid choice. Enter 1, 2, or 3.");
        }
    }

    // Apply update
    string updateMsg = "";

    if (modChoice == 1)
    {
        // Modify items and recalculate OrderTotal inside ModifyOrderItems
        ModifyOrderItems(selectedOrder, orderRestaurant);
        updateMsg = "Order " + selectedOrder.OrderId + " updated. New Order Total: $" + selectedOrder.OrderTotal.ToString("0.00");
    }
    else if (modChoice == 2)
    {
        // Modify address and validate in ModifyDeliveryAddress
        selectedOrder.DeliveryAddress = ModifyDeliveryAddress("Enter new Delivery Address: ");
        updateMsg = "Order " + selectedOrder.OrderId + " updated. New Address: " + selectedOrder.DeliveryAddress;
    }
    else if (modChoice == 3)
    {
        // Modify delivery time and validate in ModifyDeliveryTime
        TimeSpan newTime = ModifyDeliveryTime("Enter new Delivery Time (hh:mm): ");
        selectedOrder.DeliveryDateTime = selectedOrder.DeliveryDateTime.Date + newTime;
        updateMsg = "Order " + selectedOrder.OrderId + " updated. New Delivery Time: " + selectedOrder.DeliveryDateTime.ToString("HH:mm");
    }

    // If total increased, prompt payment
    double newTotal = selectedOrder.OrderTotal;

    if (newTotal > oldTotal)
    {
        Console.WriteLine("\nOrder total increased from $" + oldTotal.ToString("0.00") + " to $" + newTotal.ToString("0.00"));

        string payChoice = PayAmount("Pay the additional amount now? [Y/N]: ");

        if (payChoice == "Y")
        {
            // If payment accepted, mark as paid
            string paymentMethod = ReadPaymentMethod();
            selectedOrder.OrderPaymentMethod = paymentMethod;
            selectedOrder.OrderPaid = true;
        }
        else
        {
            // If payment declined, revert changes
            selectedOrder.DeliveryAddress = oldAddress;
            selectedOrder.DeliveryDateTime = oldDeliveryDateTime;
            selectedOrder.OrderTotal = oldTotal;
            selectedOrder.OrderPaid = false;

            selectedOrder.OrderedFoodItemList.Clear();
            foreach (OrderedFoodItem item in oldItems)
            {
                selectedOrder.AddOrderedFoodItem(item);
            }

            Console.WriteLine("Update cancelled because payment was not made.");
            Console.WriteLine();
            return;
        }
    }

    Console.WriteLine();
    // This line counts as "update existing order" in the CSV
    UpdateOrderInCsv("orders.csv", selectedOrder);
    Console.WriteLine(updateMsg);

    // Display updated items if items were updated
    if (modChoice == 1)
    {
        DisplayOrderItems(selectedOrder);
    }

    Console.WriteLine();
}
void DisplayOrderItems(Order order)
{
    if (order.OrderedFoodItemList == null || order.OrderedFoodItemList.Count == 0)
    {
        Console.WriteLine("No items.");
        return;
    }

    int i = 1;

    foreach (OrderedFoodItem item in order.OrderedFoodItemList)
    {
        Console.WriteLine(i + ". " + item.ItemName + " - " + item.QtyOrdered);
        i++;
    }
}

void ModifyOrderItems(Order order, Restaurant restaurant)
{
    if (restaurant.MenuList.Count == 0 || restaurant.MenuList[0].FoodItemList.Count == 0)
    {
        Console.WriteLine("No menu items available. Cannot modify items.");
        return;
    }

    List<FoodItem> menuItems = restaurant.MenuList[0].FoodItemList;

    Console.WriteLine("\nAvailable Food Items:");
    for (int i = 0; i < menuItems.Count; i++)
    {
        Console.WriteLine((i + 1) + ". " + menuItems[i].ItemName + " - $" + menuItems[i].ItemPrice.ToString("0.00"));
    }

    List<OrderedFoodItem> newItems = new List<OrderedFoodItem>();

    while (true)
    {
        Console.Write("Enter item number (0 to finish): ");
        int itemNo;

        try
        {
            itemNo = Convert.ToInt32(Console.ReadLine());
        }
        catch
        {
            Console.WriteLine("Invalid input. Please enter a number.");
            continue;
        }

        if (itemNo == 0) break;

        if (itemNo < 1 || itemNo > menuItems.Count)
        {
            Console.WriteLine("Invalid item number. Please choose 1 to " + menuItems.Count + ".");
            continue;
        }

        Console.Write("Enter quantity: ");
        int qty;

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
        newItems.Add(of);
    }

    if (newItems.Count == 0)
    {
        Console.WriteLine("No items selected. Items not changed.");
        return;
    }

    order.OrderedFoodItemList.Clear();
    foreach (OrderedFoodItem item in newItems)
    {
        order.AddOrderedFoodItem(item);
    }

    double itemsTotal = 0;

    foreach (OrderedFoodItem it in order.OrderedFoodItemList)
    {
        itemsTotal += it.ItemPrice * it.QtyOrdered;
    }

    double deliveryFee = 5.00;
    order.OrderTotal = itemsTotal + deliveryFee;
}

string ModifyDeliveryAddress(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string input = Console.ReadLine();

        if (input == null) input = "";
        input = input.Trim();

        if (input == "")
        {
            Console.WriteLine("Value cannot be empty.");
            continue;
        }

        return input;
    }
}

string PayAmount(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string input = Console.ReadLine();

        if (input == null) input = "";
        input = input.Trim().ToUpper();

        if (input == "Y" || input == "N")
        {
            return input;
        }

        Console.WriteLine("Invalid input. Please enter Y or N.");
    }
}

TimeSpan ModifyDeliveryTime(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string time = Console.ReadLine();

        if (time == null) time = "";
        time = time.Trim();

        if (time == "")
        {
            Console.WriteLine("Invalid time. Please enter hh:mm.");
            continue;
        }

        try
        {
            return TimeSpan.Parse(time);
        }
        catch
        {
            Console.WriteLine("Invalid time. Please enter hh:mm.");
        }
    }
}

string ReadPaymentMethod()
{
    while (true)
    {
        Console.Write("[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");
        string pm = Console.ReadLine();

        if (pm == null) pm = "";
        pm = pm.Trim().ToUpper();

        if (pm == "CC" || pm == "PP" || pm == "CD")
        {
            return pm;
        }

        Console.WriteLine("Invalid method. Enter CC, PP, or CD.");
    }
}

// Basic Feature 8: Delete an existing order
// Only pending orders can be deleted 
void DeleteOrder(List<Customer> customerList, List<Restaurant> restaurantList, Stack<Order> refundStack)
{
    Console.WriteLine("\nDelete Order");
    Console.WriteLine("============");
    Console.Write("Enter Customer Email: ");
    string email = Console.ReadLine().Trim();

    // Find customer
    Customer selectedCustomer = null;
    foreach (Customer c in customerList)
    {
        if (c.EmailAddress == email)
        {
            selectedCustomer = c;
            break;
        }
    }

    if (selectedCustomer == null)
    {
        Console.WriteLine("Customer not found");
        return;
    }

    // Show pending orders for that customer
    Console.WriteLine("Pending Orders:");
    bool hasPending = false;

    foreach (Order o in selectedCustomer.OrderList)
    {
        if (o.OrderStatus == "Pending")
        {
            Console.WriteLine(o.OrderId);
            hasPending = true;
        }
    }

    if (!hasPending)
    {
        Console.WriteLine("No pending orders found");
        return;
    }

    Console.Write("Enter Order ID: ");
    int orderId;

    try
    {
        orderId = Convert.ToInt32(Console.ReadLine());
    }
    catch
    {
        Console.WriteLine("Invalid order ID");
        return;
    }

    // Find the order in customer's list
    Order selectedOrder = null;
    foreach (Order o in selectedCustomer.OrderList)
    {
        if (o.OrderId == orderId)
        {
            selectedOrder = o;
            break;
        }
    }

    if (selectedOrder == null)
    {
        Console.WriteLine("Order ID not found for this customer");
        return;
    }

    // Must be pending to delete
    if (selectedOrder.OrderStatus != "Pending")
    {
        Console.WriteLine("Only pending orders can be deleted");
        return;
    }

    // Display order info
    Console.WriteLine($"\nCustomer: {selectedCustomer.CustomerName}");
    Console.WriteLine("Ordered Items:");

    if (selectedOrder.OrderedFoodItemList != null && selectedOrder.OrderedFoodItemList.Count > 0)
    {
        selectedOrder.DisplayOrderedFoodItems();
    }
    else
    {
        Console.WriteLine("Items not loaded yet");
    }

    Console.WriteLine($"Delivery date/time: {selectedOrder.DeliveryDateTime:dd/MM/yyyy HH:mm}");
    Console.WriteLine($"Total Amount: ${selectedOrder.OrderTotal:0.00}");
    Console.WriteLine($"Order Status: {selectedOrder.OrderStatus}");

    Console.Write("\nConfirm deletion? [Y/N]: ");
    string confirm = Console.ReadLine().Trim().ToUpper();

    if (confirm != "Y")
    {
        Console.WriteLine("Deletion cancelled");
        return;
    }

    selectedOrder.OrderStatus = "Cancelled";
    refundStack.Push(selectedOrder);
    selectedCustomer.RemoveOrder(selectedOrder);

    foreach (Restaurant r in restaurantList)
    {
        int qCount = r.OrderQueue.Count;

        for (int i = 0; i < qCount; i++)
        {
            Order current = r.OrderQueue.Dequeue();
            if (current != selectedOrder)
            {
                r.OrderQueue.Enqueue(current);
            }
        }
    }

    Console.WriteLine($"Order {selectedOrder.OrderId} cancelled and removed. Refund of ${selectedOrder.OrderTotal:0.00} processed.");
}

// Advanced Feature (a): Bulk processing of unprocessed orders for a current day
// For all restaurants, process all pending orders for today
void BulkProcessPendingOrdersToday(List<Restaurant> restaurantList, Stack<Order> refundStack)
{
    Console.WriteLine("\nBulk Process Pending Orders Today");
    Console.WriteLine("=================================");

    DateTime now = DateTime.Now;
    int totalPendingToday = 0;

    foreach (Restaurant r in restaurantList)
    {
        foreach (Order o in r.OrderQueue)
        {
            if (o.OrderStatus == "Pending" && o.DeliveryDateTime.Date == now.Date)
            {
                totalPendingToday++;
            }
        }
    }

    Console.WriteLine($"Total pending orders today: {totalPendingToday}");

    if (totalPendingToday == 0)
    {
        Console.WriteLine("No pending orders to process");
        return;
    }

    int processed = 0;
    int preparing = 0;
    int rejected = 0;

    // Process each restaurant queue
    foreach (Restaurant r in restaurantList)
    {
        int qCount = r.OrderQueue.Count;

        for (int i = 0; i < qCount; i++)
        {
            Order o = r.OrderQueue.Dequeue();

            if (o.OrderStatus == "Pending" && o.DeliveryDateTime.Date == now.Date)
            {
                TimeSpan timeLeft = o.DeliveryDateTime - now;

                if (timeLeft.TotalMinutes < 60)
                {
                    o.OrderStatus = "Rejected";
                    refundStack.Push(o);
                    rejected++;
                }
                else
                {
                    o.OrderStatus = "Preparing";
                    preparing++;
                }

                processed++;
            }

            // Put order back to keep queue data
            r.OrderQueue.Enqueue(o);
        }
    }

    // Summary output
    Console.WriteLine("\nSummary");
    Console.WriteLine("-------");
    Console.WriteLine($"Orders processed: {processed}");
    Console.WriteLine($"Preparing: {preparing}");
    Console.WriteLine($"Rejected: {rejected}");

    double percentage = (processed * 100.0) / totalPendingToday;
    Console.WriteLine($"Automatically processed orders percentage: {percentage.ToString("0.00")}%");
}

// Advanced Feature (B)
// Displays total order amount and refunds for each restaurant
// Then displays overall totals and final amount earned by Gruberoo
void DisplayRestaurantOrderSummary(List<Restaurant> restaurantList)
{
    // Delivery fee charged per order
    double deliveryFee = 5.00;

    // Overall totals across all restaurants
    double grandTotalOrders = 0;
    double grandTotalRefunds = 0;

    Console.WriteLine("\nRestaurant Order Summary");
    Console.WriteLine("========================");

    // Loop through each restaurant
    foreach (Restaurant r in restaurantList)
    {
        double restaurantOrderTotal = 0;
        double restaurantRefundTotal = 0;

        // Loop through all orders for this restaurant
        foreach (Order o in r.OrderQueue)
        {
            // Successful orders (Delivered)
            if (o.OrderStatus == "Delivered")
            {
                // Subtract delivery fee from each order
                restaurantOrderTotal += (o.OrderTotal - deliveryFee);
            }
            // Refunded orders
            else if (o.OrderStatus == "Refunded" || o.OrderStatus == "Cancelled")
            {
                restaurantRefundTotal += o.OrderTotal;
            }
        }

        // Display totals for this restaurant
        Console.WriteLine("\nRestaurant: " + r.RestaurantName + " (" + r.RestaurantId + ")");
        Console.WriteLine("Total Order Amount (excluding delivery): $" + restaurantOrderTotal.ToString("0.00"));
        Console.WriteLine("Total Refunds: $" + restaurantRefundTotal.ToString("0.00"));

        // Add to overall totals
        grandTotalOrders += restaurantOrderTotal;
        grandTotalRefunds += restaurantRefundTotal;
    }

    // Display overall summary after processing all restaurants
    Console.WriteLine("\n========================");
    Console.WriteLine("Overall Summary");
    Console.WriteLine("========================");
    Console.WriteLine("Total Order Amount: $" + grandTotalOrders.ToString("0.00"));
    Console.WriteLine("Total Refunds: $" + grandTotalRefunds.ToString("0.00"));
    Console.WriteLine("Final Amount Gruberoo Earns: $" + (grandTotalOrders - grandTotalRefunds).ToString("0.00"));
    Console.WriteLine();
}