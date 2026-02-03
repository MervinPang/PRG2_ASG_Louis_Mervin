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
    class Restaurant
    {
        public string RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantEmail { get; set; }

        public Queue<Order> OrderQueue { get; set; } = new Queue<Order>();
        public List<SpecialOffer> SpecialOfferList { get; set; } = new List<SpecialOffer>();
        public List<Menu> MenuList { get; set; } = new List<Menu>();

        public Restaurant() { } // Default
        public Restaurant(string restaurantId, string restaurantName, string restaurantEmail)
        {
            RestaurantId = restaurantId;
            RestaurantName = restaurantName;
            RestaurantEmail = restaurantEmail;
        }

        public void DisplayOrders() // Display all orders
        {
            foreach (Order o in OrderQueue)
            {
                Console.WriteLine(o);
            }
        }

        public void DisplaySpecialOffers() // Display all special offers
        {
            foreach (SpecialOffer so in SpecialOfferList)
            {
                Console.WriteLine(so);
            }
        }

        public void DisplayMenu() // Display all menus
        {
            foreach (Menu m in MenuList)
            {
                Console.WriteLine(m);
            }
        }

        public void AddMenu(Menu m) // Add a menu
        {
            if (m != null)
            {
                MenuList.Add(m);
            }
        }

        public bool RemoveMenu(Menu m) // Remove a menu
        {
            return MenuList.Remove(m);
        }

        public override string ToString()
        {
            return "Restaurant ID: " + RestaurantId + ", Name: " + RestaurantName + ", Email: " + RestaurantEmail;
        }
    }
}
