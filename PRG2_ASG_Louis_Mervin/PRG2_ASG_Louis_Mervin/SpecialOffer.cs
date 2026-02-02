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
    class SpecialOffer
    {
        public string OfferCode { get; set; }
        public string OfferDesc { get; set; }
        public double Discount { get; set; }
        public SpecialOffer() { } // Default
        public SpecialOffer(string offerCode, string offerDesc, double discount)
        {
            OfferCode = offerCode;
            OfferDesc = offerDesc;
            Discount = discount;
        }
        public override string ToString()
        {
            return "Offer Code: " + OfferCode + ", Description: " + OfferDesc + ", Discount: " + Discount + "%";
        }
    }
}
