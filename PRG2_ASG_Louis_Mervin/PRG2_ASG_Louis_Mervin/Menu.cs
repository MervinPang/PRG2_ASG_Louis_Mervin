//==========================================================
// Student Number : S10275479D
// Student Name   : Pang Wy Zen Mervin
// Partner Name   : Louis Ng Bo Lin
//==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG2_ASG_Louis_Mervin
{
    class Menu
    {
        // Menu containing a list of food items
        public string MenuId { get; set; }
        public string MenuName { get; set; }

        // List of food items available in this menu
        public List<FoodItem> FoodItemList { get; set; } = new List<FoodItem>();

        // Default Constructor
        public Menu() { }

        // Parameterized Constructor
        public Menu(string menuId, string menuName)
        {
            MenuId = menuId;
            MenuName = menuName;
        }

        // Adds a food item to the menu
        public void AddFoodItem(FoodItem foodItem)
        {
            if (foodItem != null)
            {
                FoodItemList.Add(foodItem);
            }
        }

        // Removes a food item from the menu
        public bool RemoveFoodItem(FoodItem foodItem)
        {
            if (foodItem == null || FoodItemList.Count == 0)
            {
                return false;
            }

            return FoodItemList.Remove(foodItem);
        }

        // Displays all food items in the menu
        public void DisplayFoodItems()
        {
            foreach (FoodItem f in FoodItemList)
            {
                Console.WriteLine(f);
            }
        }

        // Formatted string containing menu details
        public override string ToString()
        {
            return MenuName + " (" + MenuId + ")";
        }
    }
}
