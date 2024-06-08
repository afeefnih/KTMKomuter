using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace KTMKomuter.Models
{
    public class KtmUsers
    {
        [Display(Name = "Komuter Id")]
        public string Id
        {
            get
            {
                string hexTicks = DateTime.Now.Ticks.ToString("X");
                return hexTicks.Substring(hexTicks.Length - 15, 10);
            }
            set { }
        }

        [Display(Name = "Komuter Id")]
        public string? ViewId { get; set; }

        // Sender
        [Required(ErrorMessage = "Enter Your name")]
        [Display(Name = "Name")]
        public string? PurchaserName { get; set; }

        [Required(ErrorMessage = "Enter Your IC or Passport No")]
        [Display(Name = "IC / Passport No")]
        public string? IdentityCardOrPassportNumber { get; set; }

        [Required(ErrorMessage = "Enter Your Email")]
        [Display(Name = "Email")]
        public string? EmailAddress { get; set; }

        // Parcel
        [Required(ErrorMessage = "Current Destination is required.")]
        [Display(Name = "Current Destination")]
        public int IndexCurrentDestination { get; set; }

        [Required(ErrorMessage = "To Destination is required.")]
        [Display(Name = "To Destination")]
        public int IndexToDestination { get; set; }

        [DisplayFormat(DataFormatString = "{0:c2}")]
        [Display(Name = "Paid Amount")]
        public double Amount
        {
            get
            {
                return rates[IndexCurrentDestination, IndexToDestination];
            }
            set { }
        }

        [Required(ErrorMessage = "Select Your Category")]
        [Display(Name = "Category")]
        public int Category { get; set; }

        [Required(ErrorMessage = "Select Your Category")]
        [Display(Name = "Category")]
        public IDictionary<int, string> DictCategoryDiscount
        {
            get
            {
                return new Dictionary<int, string>()
                {
                {0, "Senior Citizens"},
                {1, "disabled"},
                {2, "students"},
                {3, "None"}

             };
            }
            set { }
        }

        [DisplayFormat(DataFormatString = "{0:c2}")]
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

        // Delivery Rate Table
        [Required]
        [Display(Name = "Current Destination")]
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
        [Display(Name = "Desire Destination")]
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
    {0, 1.40, 1.70, 1.80, 1.80, 2.10, 2.10, 2.50, 2.90, 3.40, 3.70, 4.00, 4.20, 4.20, 4.60, 4.70, 4.80, 5.00, 5.40, 5.60, 5.70, 5.90, 6.10, 6.10, 6.20, 6.30, 6.60},
    {1.40, 0, 1.20, 1.80, 1.80, 2.10, 2.10, 2.50, 2.90, 3.20, 3.50, 3.80, 3.90, 4.00, 4.30, 4.40, 4.60, 4.80, 5.20, 5.30, 5.50, 5.70, 5.90, 6.00, 6.00, 6.10, 6.40},
    {1.70, 1.20, 0, 1.80, 1.70, 2.00, 2.00, 2.30, 2.70, 3.00, 3.30, 3.50, 3.80, 3.90, 4.20, 4.30, 4.40, 4.60, 5.00, 5.10, 5.30, 5.50, 5.70, 5.80, 5.90, 6.00, 6.30},
    {1.80, 1.80, 1.80, 0, 1.60, 1.80, 1.80, 2.20, 2.60, 3.10, 3.40, 3.60, 3.80, 4.00, 4.40, 4.50, 4.60, 4.80, 5.20, 5.30, 5.50, 5.70, 5.90, 6.00, 6.10, 6.20, 6.50},
    {1.80, 1.80, 1.70, 1.60, 0, 1.80, 1.80, 2.20, 2.60, 3.10, 3.40, 3.60, 3.80, 4.00, 4.40, 4.50, 4.60, 4.80, 5.20, 5.30, 5.50, 5.70, 5.90, 6.00, 6.10, 6.20, 6.50},
    {2.10, 2.10, 2.00, 1.80, 1.80, 0, 1.60, 2.00, 2.40, 2.90, 3.20, 3.40, 3.60, 3.80, 4.20, 4.30, 4.40, 4.60, 5.00, 5.10, 5.30, 5.50, 5.70, 5.80, 5.90, 6.00, 6.30},
    {2.10, 2.10, 2.00, 1.80, 1.80, 1.60, 0, 2.00, 2.40, 2.90, 3.20, 3.40, 3.60, 3.80, 4.20, 4.30, 4.40, 4.60, 5.00, 5.10, 5.30, 5.50, 5.70, 5.80, 5.90, 6.00, 6.30},
    {2.50, 2.30, 2.30, 2.20, 2.20, 2.00, 2.00, 0, 2.00, 2.40, 2.70, 2.90, 3.10, 3.30, 3.70, 3.80, 3.90, 4.10, 4.50, 4.60, 4.80, 5.00, 5.20, 5.30, 5.40, 5.50, 5.80},
    {2.90, 2.90, 2.70, 2.60, 2.60, 2.40, 2.40, 2.00, 0, 2.00, 2.30, 2.50, 2.70, 2.90, 3.30, 3.40, 3.50, 3.70, 4.10, 4.20, 4.40, 4.60, 4.80, 4.90, 5.00, 5.10, 5.40},
    {3.40, 3.20, 3.00, 2.90, 2.90, 2.70, 2.70, 2.40, 2.00, 0, 1.70, 1.90, 2.10, 2.30, 2.70, 2.80, 2.90, 3.10, 3.50, 3.60, 3.80, 4.00, 4.20, 4.30, 4.40, 4.50, 4.80},
    {3.70, 3.50, 3.30, 3.40, 3.40, 3.20, 3.20, 2.70, 2.30, 1.70, 0, 1.70, 1.90, 2.10, 2.50, 2.60, 2.70, 2.90, 3.30, 3.40, 3.60, 3.80, 4.00, 4.10, 4.20, 4.30, 4.60},
    {4.00, 3.80, 3.50, 3.60, 3.60, 3.40, 3.40, 2.90, 2.50, 1.90, 1.70, 0, 1.60, 1.80, 2.20, 2.30, 2.40, 2.60, 3.00, 3.10, 3.30, 3.50, 3.70, 3.80, 3.90, 4.00, 4.30},
    {4.20, 3.90, 3.80, 3.80, 3.80, 3.60, 3.60, 3.10, 2.70, 2.10, 1.90, 1.60, 0, 1.70, 2.10, 2.20, 2.30, 2.50, 2.90, 3.00, 3.20, 3.40, 3.60, 3.70, 3.80, 3.90, 4.20},
    {4.20, 4.00, 3.90, 4.00, 4.00, 3.80, 3.80, 3.30, 2.90, 2.30, 2.10, 1.80, 1.70, 0, 1.60, 1.70, 1.80, 2.00, 2.40, 2.50, 2.70, 2.90, 3.10, 3.20, 3.30, 3.40, 3.70},
    {4.60, 4.30, 4.20, 4.40, 4.40, 4.20, 4.20, 3.70, 3.30, 2.70, 2.50, 2.20, 2.10, 1.60, 0, 1.40, 1.50, 1.70, 2.10, 2.20, 2.40, 2.60, 2.80, 2.90, 3.00, 3.10, 3.40},
    {4.70, 4.40, 4.30, 4.50, 4.50, 4.30, 4.30, 3.80, 3.40, 2.80, 2.60, 2.30, 2.20, 1.70, 1.40, 0, 1.40, 1.60, 2.00, 2.10, 2.30, 2.50, 2.70, 2.80, 2.90, 3.00, 3.30},
    {4.80, 4.60, 4.40, 4.60, 4.60, 4.40, 4.40, 3.90, 3.50, 2.90, 2.70, 2.40, 2.30, 1.80, 1.50, 1.40, 0, 1.40, 1.80, 1.90, 2.10, 2.30, 2.50, 2.60, 2.70, 2.80, 3.10},
    {5.00, 4.80, 4.60, 4.80, 4.80, 4.60, 4.60, 4.10, 3.70, 3.10, 2.90, 2.60, 2.50, 2.00, 1.70, 1.60, 1.40, 0, 1.40, 1.50, 1.70, 1.90, 2.10, 2.20, 2.30, 2.40, 2.70},
    {5.40, 5.20, 5.00, 5.20, 5.20, 5.00, 5.00, 4.50, 4.10, 3.50, 3.30, 3.00, 2.90, 2.40, 2.10, 2.00, 1.80, 1.40, 0, 1.30, 1.50, 1.70, 1.90, 2.00, 2.10, 2.20, 2.50},
    {5.60, 5.30, 5.10, 5.30, 5.30, 5.10, 5.10, 4.60, 4.20, 3.60, 3.40, 3.10, 3.00, 2.50, 2.20, 2.10, 1.90, 1.50, 1.30, 0, 1.40, 1.60, 1.80, 1.90, 2.00, 2.10, 2.40},
    {5.70, 5.50, 5.30, 5.50, 5.50, 5.30, 5.30, 4.80, 4.40, 3.80, 3.60, 3.30, 3.20, 2.70, 2.40, 2.30, 2.10, 1.70, 1.50, 1.40, 0, 1.40, 1.60, 1.70, 1.80, 1.90, 2.20},
    {5.90, 5.70, 5.50, 5.70, 5.70, 5.50, 5.50, 5.00, 4.60, 4.00, 3.80, 3.50, 3.40, 2.90, 2.60, 2.50, 2.30, 1.90, 1.70, 1.60, 1.40, 0, 1.40, 1.50, 1.60, 1.70, 2.00},
    {6.10, 5.90, 5.70, 5.90, 5.90, 5.70, 5.70, 5.20, 4.80, 4.20, 4.00, 3.70, 3.60, 3.10, 2.80, 2.70, 2.50, 2.10, 1.90, 1.80, 1.60, 1.40, 0, 1.40, 1.50, 1.60, 1.90},
    {6.10, 6.00, 5.80, 6.00, 6.00, 5.80, 5.80, 5.30, 4.90, 4.30, 4.10, 3.80, 3.70, 3.20, 2.90, 2.80, 2.60, 2.20, 2.00, 1.90, 1.70, 1.50, 1.40, 0, 1.40, 1.50, 1.80},
    {6.20, 6.10, 5.90, 6.10, 6.10, 5.90, 5.90, 5.40, 5.00, 4.40, 4.20, 3.90, 3.80, 3.30, 3.00, 2.90, 2.70, 2.30, 2.10, 2.00, 1.80, 1.60, 1.50, 1.40, 0, 1.40, 1.70},
    {6.30, 6.10, 6.00, 6.20, 6.20, 6.00, 6.00, 5.50, 5.10, 4.50, 4.30, 4.00, 3.90, 3.40, 3.10, 3.00, 2.80, 2.40, 2.20, 2.10, 1.90, 1.70, 1.60, 1.50, 1.40, 0, 1.60},
    {6.60, 6.40, 6.30, 6.50, 6.50, 6.30, 6.30, 5.80, 5.40, 4.80, 4.60, 4.30, 4.20, 3.70, 3.40, 3.30, 3.10, 2.70, 2.50, 2.40, 2.20, 2.00, 1.90, 1.80, 1.70, 1.60, 0},
};

    }
}

