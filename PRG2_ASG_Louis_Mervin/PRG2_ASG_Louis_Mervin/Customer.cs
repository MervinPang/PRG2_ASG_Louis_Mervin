//==========================================================
// Student Number : S10268761B
// Student Name   : Louis Ng Bo Lin
// Partner Name   : Pang Wy Zen Mervin
//==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG2_ASG_Louis_Mervin
{
    class Customer
    {
        public string EmailAddress { get; set; }
        public string CustomerName { get; set; }

        public List<Order> OrderList = new List<Order>();

        public Customer() { } // Default
        public Customer(string emailAddress, string customerName)
        {
            EmailAddress = emailAddress;
            CustomerName = customerName;
        }

        public void AddOrder(Order o) // Add an order
        {
            OrderList.Add(o);
        }

        public void DisplayAllOrders() // Display all orders
        {
            foreach (Order o in OrderList)
            {
                Console.WriteLine(o);
            }
        }

        public bool RemoveOrder(Order o) // Remove an order
        {
            return OrderList.Remove(o);
        }

        public override string ToString()
        {
            return "Customer Name: " + CustomerName + ", Email Address: " + EmailAddress;
        }
    }
}
