using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KTMKomuter.Models
{
    public class KtmUsers
    {
        [Display(Name = "ID")]
        public string Id
        {
            get
            {
                string hexTicks = DateTime.Now.Ticks.ToString("X");
                return hexTicks.Substring(hexTicks.Length - 15, 10);
            }
            set { }
        }

        [Display(Name = "ID")]
        public string? ViewId { get; set; }

        // Sender
        [Required(ErrorMessage = "Enter Your name")]
        [Display(Name = "Name")]
        public string? PurchaserName { get; set; }

        [Required(ErrorMessage = "Enter Your IC or Passport No")]
        [Display(Name = "IC/Passport")]
        public string? IdentityCardOrPassportNumber { get; set; }

        [Required(ErrorMessage = "Enter Your Email")]
        [Display(Name = "Email")]
        public string? EmailAddress { get; set; }

        // Parcel
        [Required(ErrorMessage = "Origin is required.")]
        [Display(Name = "From")]
        public int IndexCurrentDestination { get; set; }

        [Required(ErrorMessage = "Destination is required.")]
        [Display(Name = "To")]
        public int IndexToDestination { get; set; }

        [DisplayFormat(DataFormatString = "{0:RM0.00}")]
        [Display(Name = "Regular Price")]
        public double Amount
        {
            get
            {
                if (TicketType == 0)
                {
                    return rates[IndexCurrentDestination, IndexToDestination] * NumberOfTickets.GetValueOrDefault(0);
                }
                else
                {
                    return rates[IndexCurrentDestination, IndexToDestination] * NumberOfTickets.GetValueOrDefault(0) * 2;
                }
            }
            set { }
        }

        [Required(ErrorMessage = "Select Your Category")]
        [Display(Name = "Category")]
        public int Category { get; set; }

        [Display(Name = "Category")]
        public IDictionary<int, string> DictCategoryDiscount
        {
            get
            {
                return new Dictionary<int, string>()
                {
                    {0, "Senior Citizens"},
                    {1, "Disabled"},
                    {2, "Students"},
                    {3, "None"}
                };
            }
            set { }
        }

        public string CategoryString
        {
            get
            {
                return Category == 0 ? "Senior Citizens" : Category == 1 ? "Disabled" : Category == 2 ? "Students" : "None";
            }
            set { }
        }

        [Required(ErrorMessage = "Enter Number of Tickets")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of tickets must be greater than 0.")]
        [Display(Name = "Number of Tickets")]
        public int? NumberOfTickets { get; set; }

        [Required(ErrorMessage = "Select Ticket Type")]
        [Display(Name = "Ticket Type")]
        public int TicketType { get; set; }

        [Display(Name = "Ticket Type")]
        public IDictionary<int, string> DictTicketType
        {
            get
            {
                return new Dictionary<int, string>()
                {
                    { 0, "One Way"},
                    { 1, "Return"}
                };
            }
            set { }
        }

        public string TicketTypeString
        {
            get
            {
                return TicketType == 0 ? "One Way" : "Return";
            }
            set { }
        }

        [DisplayFormat(DataFormatString = "{0:RM0.00}")]
        [Display(Name = "Final Price")]
        public double AfterDiscount
        {
            get
            {
                double discountRate = 0.0;

                if (Category == 0) // Senior Citizens
                {
                    discountRate = 0.3; // 30% discount
                }
                else if (Category == 1) // Disabled
                {
                    discountRate = 0.35; // 35% discount
                }
                else if (Category == 2) // Students
                {
                    discountRate = 0.25; // 25% discount
                }
                else if (Category == 3) // None
                {
                    discountRate = 0.0; // 0% discount
                }

                return Amount * (1 - discountRate); // Calculate discounted amount
            }
            set { }
        }

        [Required]
        [Display(Name = "From")]
        public IDictionary<int, string> DictCurrentDestination
        {
            get
            {
                return new Dictionary<int, string>()
                {
                    { 0, "PEL. KLANG"},
                    { 1, "JLN.KASTAM"},
                    { 2, "KG.RAJA UDA"},
                    { 3, "TELUK GADONG"},
                    { 4, "TELUK PULAI"},
                    { 5, "KLANG"},
                    { 6, "BUKIT BADAK"},
                    { 7, "PADANG JAWA"},
                    { 8, "SHAH ALAM"},
                    { 9, "BATU TIGA"},
                    {10, "SUBANG JAYA"},
                    {11, "SETIA JAYA" },
                    {12, "SERI SETIA" },
                    {13, "KG. DATO HARUN" },
                    {14, "JLN TEMPLER" },
                    {15, "PETALING" },
                    {16, "PANTAI DALAM" },
                    {17, "ANGKASAPURI" },
                    {18, "KL SENTRAL" },
                    {19, "KUALA LUMPUR" },
                    {20, "BANK NEGARA" },
                    {21, "PUTRA" },
                    {22, "SENTUL" },
                    {23, "BATU KENTONMEN" },
                    {24, "KG BATU" },
                    {25, "TAMAN WAHYU" },
                    {26, "BATU CAVES" }
                };
            }
            set { }
        }

        [Required]
        [Display(Name = "To")]
        public IDictionary<int, string> DictToDestination
        {
            get
            {
                return new Dictionary<int, string>()
                {
                    { 0, "PEL. KLANG"},
                    { 1, "JLN.KASTAM"},
                    { 2, "KG.RAJA UDA"},
                    { 3, "TELUK GADONG"},
                    { 4, "TELUK PULAI"},
                    { 5, "KLANG"},
                    { 6, "BUKIT BADAK"},
                    { 7, "PADANG JAWA"},
                    { 8, "SHAH ALAM"},
                    { 9, "BATU TIGA"},
                    {10, "SUBANG JAYA"},
                    {11, "SETIA JAYA" },
                    {12, "SERI SETIA" },
                    {13, "KG. DATO HARUN" },
                    {14, "JLN TEMPLER" },
                    {15, "PETALING" },
                    {16, "PANTAI DALAM" },
                    {17, "ANGKASAPURI" },
                    {18, "KL SENTRAL" },
                    {19, "KUALA LUMPUR" },
                    {20, "BANK NEGARA" },
                    {21, "PUTRA" },
                    {22, "SENTUL" },
                    {23, "BATU KENTONMEN" },
                    {24, "KG BATU" },
                    {25, "TAMAN WAHYU" },
                    {26, "BATU CAVES" }
                };
            }
            set { }
        }

        static double[,] rates =
{
            {0.00, 1.40, 1.70, 1.80, 1.80, 2.10, 2.10, 2.50, 2.90, 3.40, 3.70, 4.00, 4.20, 4.20, 4.60, 4.70, 4.80, 5.00, 5.40, 5.60, 5.70, 5.90, 6.10, 6.10, 6.20, 6.30, 6.60 },
            {1.40, 0.00, 1.20, 1.80, 1.80, 1.80, 2.10, 2.30, 2.70, 3.20, 3.50, 3.80, 3.90, 4.00, 4.30, 4.40, 4.60, 4.80, 5.20, 5.30, 5.50, 5.70, 5.90, 6.00, 6.00, 6.10, 6.40 },
            {1.70, 1.20, 0.00, 1.50, 1.70, 1.70, 2.00, 2.20, 2.60, 3.00, 3.40, 3.70, 3.80, 3.90, 4.20, 4.30, 4.50, 4.70, 5.10, 5.20, 5.40, 5.50, 5.80, 6.00, 6.00, 6.00, 6.30 },
            {1.80, 1.80, 1.50, 0.00, 1.20, 1.70, 1.70, 2.20, 2.30, 2.80, 3.20, 3.50, 3.60, 3.70, 4.00, 4.10, 4.20, 4.50, 4.90, 5.00, 5.20, 5.30, 5.60, 5.80, 5.90, 6.00, 6.10 },
            {1.80, 1.80, 1.70, 1.20, 0.00, 1.50, 1.70, 2.10, 2.20, 2.70, 3.10, 3.40, 3.50, 3.60, 3.90, 4.00, 4.10, 4.40, 4.80, 4.90, 5.10, 5.20, 5.50, 5.70, 5.80, 5.90, 6.00 },
            {2.10, 1.80, 1.70, 1.70, 1.50, 0.00, 1.60, 1.80, 2.20, 2.50, 2.80, 3.10, 3.20, 3.30, 3.60, 3.70, 3.90, 4.10, 4.50, 4.60, 4.80, 5.00, 5.20, 5.40, 5.50, 5.70, 6.00 },
            {2.10, 2.10, 2.00, 1.70, 1.70, 1.60, 0.00, 1.80, 1.90, 2.20, 2.50, 2.80, 2.90, 3.00, 3.30, 3.50, 3.60, 3.80, 4.20, 4.30, 4.50, 4.70, 4.90, 5.10, 5.20, 5.40, 5.70 },
            {2.50, 2.30, 2.20, 2.20, 2.10, 1.80, 1.80, 0.00, 1.80, 2.00, 2.10, 2.40, 2.50, 2.60, 2.90, 3.10, 3.20, 3.40, 3.80, 3.90, 4.10, 4.30, 4.50, 4.70, 4.80, 5.00, 5.30 },
            {2.90, 2.70, 2.60, 2.30, 2.20, 2.20, 1.90, 1.80, 0.00, 2.00, 2.00, 2.00, 2.20, 2.20, 2.60, 2.70, 2.80, 3.00, 3.50, 3.60, 3.70, 3.90, 4.10, 4.40, 4.50, 4.60, 4.90 },
            {3.40, 3.20, 3.00, 2.80, 2.70, 2.50, 2.20, 2.00, 2.00, 0.00, 1.70, 1.70, 1.90, 2.00, 2.10, 2.20, 2.30, 2.50, 3.00, 3.10, 3.30, 3.40, 3.70, 3.90, 4.00, 4.10, 4.40 },
            {3.70, 3.50, 3.40, 3.20, 3.10, 2.80, 2.50, 2.10, 2.00, 1.70, 0.00, 1.60, 1.90, 1.90, 1.90, 2.10, 2.20, 2.20, 2.60, 2.70, 2.90, 3.00, 3.30, 3.50, 3.60, 3.70, 4.00 },
            {4.00, 3.80, 3.70, 3.50, 3.40, 3.10, 2.80, 2.40, 2.00, 1.70, 1.60, 0.00, 1.20, 1.40, 1.70, 1.70, 1.90, 2.20, 2.30, 2.40, 2.60, 2.70, 3.00, 3.20, 3.30, 3.50, 3.80 },
            {4.20, 3.90, 3.80, 3.60, 3.50, 3.20, 2.90, 2.50, 2.20, 1.90, 1.90, 1.20, 0.00, 1.10, 1.70, 1.70, 1.90, 2.00, 2.20, 2.30, 2.50, 2.60, 2.90, 3.10, 3.20, 3.30, 3.60 },
            {4.20, 4.00, 3.90, 3.70, 3.60, 3.30, 3.00, 2.60, 2.20, 2.00, 1.90, 1.40, 1.10, 0.00, 1.60, 1.60, 1.60, 1.90, 2.10, 2.20, 2.40, 2.50, 2.80, 3.00, 3.10, 3.30, 3.60 },
            {4.60, 4.30, 4.20, 4.00, 3.90, 3.60, 3.30, 2.90, 2.60, 2.10, 1.90, 1.70, 1.70, 1.60, 0.00, 1.20, 1.50, 1.90, 2.00, 2.10, 2.10, 2.20, 2.50, 2.70, 2.80, 2.90, 3.20 },
            {4.70, 4.40, 4.30, 4.10, 4.00, 3.70, 3.50, 3.10, 2.70, 2.20, 2.10, 1.70, 1.70, 1.60, 1.20, 0.00, 1.30, 1.80, 1.90, 2.00, 2.10, 2.10, 2.40, 2.60, 2.70, 2.80, 3.10 },
            {4.80, 4.60, 4.50, 4.20, 4.10, 3.90, 3.60, 3.20, 2.80, 2.30, 2.20, 1.90, 1.90, 1.60, 1.50, 1.30, 0.00, 1.40, 1.70, 1.90, 2.10, 2.10, 2.20, 2.40, 2.50, 2.70, 3.00 },
            {5.00, 4.80, 4.70, 4.50, 4.40, 4.10, 3.80, 3.40, 3.00, 2.50, 2.20, 2.20, 2.00, 1.90, 1.90, 1.80, 1.40, 0.00, 1.70, 1.80, 1.80, 2.00, 2.00, 2.20, 2.30, 2.50, 2.80 },
            {5.40, 5.20, 5.10, 4.90, 4.80, 4.50, 4.20, 3.80, 3.50, 3.00, 2.60, 2.30, 2.20, 2.10, 2.00, 1.90, 1.70, 1.70, 0.00, 1.20, 1.60, 1.80, 1.80, 2.00, 2.00, 2.00, 2.30 },
            {5.60, 5.30, 5.20, 5.00, 4.90, 4.60, 4.30, 3.90, 3.60, 3.10, 2.70, 2.40, 2.30, 2.20, 2.10, 2.00, 1.90, 1.80, 1.20, 0.00, 1.40, 1.60, 1.60, 1.90, 2.00, 2.00, 2.20 },
            {5.70, 5.50, 5.40, 5.20, 5.10, 4.80, 4.50, 4.10, 3.70, 3.30, 2.90, 2.60, 2.50, 2.40, 2.10, 2.10, 2.10, 1.80, 1.60, 1.40, 0.00, 1.20, 1.60, 1.70, 1.80, 2.00, 2.00 },
            {5.90, 5.70, 5.50, 5.30, 5.20, 5.00, 4.70, 4.30, 3.90, 3.40, 3.00, 2.70, 2.60, 2.50, 2.20, 2.10, 2.10, 2.00, 1.80, 1.60, 1.20, 0.00, 1.50, 1.70, 1.80, 1.80, 2.00 },
            {6.10, 5.90, 5.80, 5.60, 5.50, 5.20, 4.90, 4.50, 4.10, 3.70, 3.30, 3.00, 2.90, 2.80, 2.50, 2.40, 2.20, 2.00, 1.80, 1.60, 1.60, 1.50, 0.00, 1.40, 1.70, 1.80, 2.00 },
            {6.10, 6.00, 6.00, 5.80, 5.70, 5.40, 5.10, 4.70, 4.40, 3.90, 3.50, 3.20, 3.10, 3.00, 2.70, 2.60, 2.40, 2.20, 2.00, 1.90, 1.70, 1.70, 1.40, 0.00, 1.10, 1.50, 2.00 },
            {6.20, 6.00, 6.00, 5.90, 5.80, 5.50, 5.20, 4.80, 4.50, 4.00, 3.60, 3.30, 3.20, 3.10, 2.80, 2.70, 2.50, 2.30, 2.00, 2.00, 1.80, 1.80, 1.70, 1.10, 0.00, 1.20, 2.00 },
            {6.30, 6.10, 6.00, 6.00, 5.90, 5.70, 5.40, 5.00, 4.60, 4.10, 3.70, 3.50, 3.30, 3.30, 2.90, 2.80, 2.70, 2.50, 2.00, 2.00, 2.00, 1.80, 1.80, 1.50, 1.20, 0.00, 1.60 },
            {6.60, 6.40, 6.30, 6.10, 6.00, 6.00, 5.70, 5.30, 4.90, 4.40, 4.00, 3.80, 3.60, 3.60, 3.20, 3.10, 3.00, 2.80, 2.30, 2.20, 2.00, 2.00, 2.00, 2.00, 2.00, 1.60, 0.00 }
};

    }
}

