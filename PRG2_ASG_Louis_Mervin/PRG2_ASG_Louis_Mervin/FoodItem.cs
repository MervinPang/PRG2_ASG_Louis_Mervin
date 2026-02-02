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
    class FoodItem
    {
        // Food item available in a menu
        public string ItemName { get; set; }
        public string ItemDesc { get; set; }
        public double ItemPrice { get; set; }
        public string Customise { get; set; }

        // Default Constructor
        public FoodItem() { }

        // Parameterized Constructor
        public FoodItem(string itemName, string itemDesc, double itemPrice, string customise)
        {
            ItemName = itemName;
            ItemDesc = itemDesc;
            ItemPrice = itemPrice;
            Customise = customise;
        }

        // Formatted string containing food item details
        public override string ToString()
        {
            return ItemName + " - " + ItemDesc + " ($" + ItemPrice.ToString("F2") + ") Customise: " + Customise;
        }
    }
}
