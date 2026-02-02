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
    class Order
    {
        // Customer's Order
        public int OrderId { get; set; }
        public DateTime OrderDateTime { get; set; }
        public double OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        public DateTime DeliveryDateTime { get; set; }
        public string DeliveryAddress { get; set; }
        public string OrderPaymentMethod { get; set; }
        public bool OrderPaid { get; set; }

        // List of ordered food items in this order
        public List<OrderedFoodItem> OrderedFoodItemList { get; set; } = new List<OrderedFoodItem>();

        // Special offer applied to this order (if any)
        public SpecialOffer SpecialOffer { get; set; }

        // Default Constructor
        public Order() { }

        // Parameterized Constructor
        public Order(int orderId, DateTime orderDateTime, DateTime deliveryDateTime, string deliveryAddress,
             string orderStatus, string orderPaymentMethod, bool orderPaid, double orderTotal)
        {
            OrderId = orderId;
            OrderDateTime = orderDateTime;
            DeliveryDateTime = deliveryDateTime;
            DeliveryAddress = deliveryAddress;
            OrderStatus = orderStatus;
            OrderPaymentMethod = orderPaymentMethod;
            OrderPaid = orderPaid;
            OrderTotal = orderTotal;
        }

        // Adds an ordered food item to the order
        public void AddOrderedFoodItem(OrderedFoodItem orderedFoodItem)
        {
            if (orderedFoodItem != null)
            {
                OrderedFoodItemList.Add(orderedFoodItem);
            }
        }

        // Removes an ordered food item from the order
        public bool RemoveOrderedFoodItem(OrderedFoodItem orderedFoodItem)
        {
            if (orderedFoodItem == null || OrderedFoodItemList.Count == 0)
            {
                return false;
            }

            return OrderedFoodItemList.Remove(orderedFoodItem);
        }

        // Displays all ordered food items in the order
        public void DisplayOrderedFoodItems()
        {
            foreach (OrderedFoodItem of in OrderedFoodItemList)
            {
                Console.WriteLine(of);
            }
        }

        // Calculates and returns the total cost of the order
        public double CalculateOrderTotal()
        {
            double total = 0;

            foreach (OrderedFoodItem of in OrderedFoodItemList)
            {
                total += of.CalculateSubTotal();
            }

            // Apply special offer discount if applicable
            if (SpecialOffer != null && SpecialOffer.Discount > 0)
            {
                total = total * (1 - SpecialOffer.Discount);
            }

            OrderTotal = total;
            return OrderTotal;
        }

        // Formatted string containing order summary details
        public override string ToString()
        {
            string offerText;
            if (SpecialOffer == null)
            {
                offerText = "No Offer";
            }
            else
            {
                offerText = SpecialOffer.OfferCode;
            }

            return "Order: " + OrderId + " | Total: $" + OrderTotal.ToString("F2") + " | Status: " + OrderStatus + " | Offer: " + offerText;
        }
    }
}
