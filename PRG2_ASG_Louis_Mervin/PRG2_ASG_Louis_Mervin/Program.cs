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

// Stores all Customer objects loaded from customers.csv
List<Customer> customerList = new List<Customer>();
Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
LoadCustomers(customerList);
DisplayCustomers(customerList);

// --------------- DISPLAY METHODS ---------------
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