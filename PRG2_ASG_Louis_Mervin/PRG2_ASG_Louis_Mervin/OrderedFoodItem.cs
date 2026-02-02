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
    class OrderedFoodItem : FoodItem
    {
        // Food item that has been ordered by a customer
        public int QtyOrdered { get; set; }
        public double SubTotal { get; set; }

        // Default Constructor
        public OrderedFoodItem() : base() { }

        // Parameterized Constructor
        public OrderedFoodItem(string itemName, string itemDesc, double itemPrice, string customise, int qtyOrdered)
            : base(itemName, itemDesc, itemPrice, customise)
        {
            QtyOrdered = qtyOrdered;
            SubTotal = CalculateSubTotal();
        }

        // Calculates and returns the subtotal based on quantity and item price
        public double CalculateSubTotal()
        {
            SubTotal = ItemPrice * QtyOrdered;
            return SubTotal;
        }

        // Formatted string containing ordered food item details
        public override string ToString()
        {
            return ItemName + " x " + QtyOrdered + " = $" + SubTotal.ToString("F2");
        }
    }
}
