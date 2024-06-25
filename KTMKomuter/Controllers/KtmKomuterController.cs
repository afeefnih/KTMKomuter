using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Collections.Generic;
using KTMKomuter.Models;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Security.Policy;
using System.Collections;
using KTMKomuter.MailSettings;

namespace KTMKomuter.Controllers
{
    public class KtmKomuterController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<KtmKomuterController> logger;

        public KtmKomuterController(IConfiguration config, ILogger<KtmKomuterController> logger)
        {
            this.configuration = config;
            this.logger = logger;
        }

        IList<KtmUsers> GetDbList(out string errorMessage)
        {
            errorMessage = null;
            IList<KtmUsers> dbList = new List<KtmUsers>();
            SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ParcelConnStr"));

            string sql = @"SELECT * FROM KtmUser";
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dbList.Add(new KtmUsers()
                    {
                        ViewId = reader.GetString(0),
                        PurchaserName = reader.GetString(1),
                        IdentityCardOrPassportNumber = reader.GetString(2),
                        EmailAddress = reader.GetString(3),
                        IndexCurrentDestination = reader.GetInt32(4),
                        IndexToDestination = reader.GetInt32(5),
                        Amount = reader.GetDouble(6),
                        AfterDiscount = reader.GetDouble(7),
                        Category = reader.GetInt32(8),
                        TicketType = reader.GetInt32(9),
                        NumberOfTickets = reader.GetInt32(10),
                        ViewDateTime = reader.GetDateTime(11),
                    });
                }
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "SQL error occurred while retrieving the KtmUser list.");
                errorMessage = "SQL error: " + ex.Message;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving the KtmUser list.");
                errorMessage = "Error: " + ex.Message;
            }
            finally
            {
                conn.Close();
            }
            return dbList;
        }

        public IActionResult Index()
        {
            string errorMessage;
            var dbList = GetDbList(out errorMessage);
            if (errorMessage != null)
            {
                return RedirectToAction("Error", new { message = errorMessage });
            }

            return View(dbList);
        }

        public IActionResult Report()
        {
            string errorMessage;
            var dbList = GetDbList(out errorMessage);
            if (errorMessage != null)
            {
                return RedirectToAction("Error", new { message = errorMessage });
            }

            // Total number of tickets sold
            var totalTicketsSold = dbList.Sum(user => user.NumberOfTickets);
            ViewBag.TotalTicketsSold = totalTicketsSold;

            // Average discounted amount per user
            var averageDiscountedAmountPerUser = dbList.Any() ? dbList.Average(user => user.AfterDiscount) : 0;
            ViewBag.AverageDiscountedAmountPerUser = averageDiscountedAmountPerUser;

            // Total amount collected today
            var today = DateTime.Today;
            var totalAmountCollectedToday = dbList
                .Where(user => user.ViewDateTime.Date == today)
                .Sum(user => user.AfterDiscount);
            ViewBag.TotalAmountCollectedToday = totalAmountCollectedToday;

            // Top purchaser by amount
            var topPurchaser = dbList
                .OrderByDescending(user => user.AfterDiscount)
                .FirstOrDefault();
            ViewBag.TopPurchaser = topPurchaser;

            // Number of transactions today
            var numberOfTransactionsToday = dbList
                .Count(user => user.ViewDateTime.Date == today);
            ViewBag.NumberOfTransactionsToday = numberOfTransactionsToday;

            // Most popular destination
            var mostPopularDestination = dbList
                .GroupBy(user => user.IndexToDestination)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .FirstOrDefault();
            ViewBag.MostPopularDestination = mostPopularDestination;

            // Count of different ticket types
            var ticketTypeCounts = dbList
                .GroupBy(user => user.TicketType)
                .Select(group => new { TicketType = group.Key, Count = group.Count() })
                .ToList();
            ViewBag.TicketTypeCounts = ticketTypeCounts;


            return View(dbList);
        }

        public IActionResult Error(string message)
        {
            ViewBag.ErrorMessage = message;
            return View();
        }

        [HttpGet]
        public IActionResult TrainTicket()
        {
            KtmUsers ktm = new KtmUsers();
            ktm.IndexCurrentDestination = ktm.IndexToDestination = ktm.Category = -1;
            ktm.TicketType = -1;
            return View(ktm);
        }
        [HttpPost]
        public IActionResult TrainTicket(KtmUsers ktm)
        {
            if (ModelState.IsValid)
            {
                ktm.ViewDateTime = DateTime.Now;
                SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ParcelConnStr"));
                SqlCommand cmd = new SqlCommand("spInsertIntoTable", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", ktm.Id);
                cmd.Parameters.AddWithValue("@PurchaserName", ktm.PurchaserName);
                cmd.Parameters.AddWithValue("@IdentityCardOrPassportNumber", ktm.IdentityCardOrPassportNumber);
                cmd.Parameters.AddWithValue("@EmailAddress", ktm.EmailAddress);
                cmd.Parameters.AddWithValue("@IndexCurrentDestination", ktm.IndexCurrentDestination);
                cmd.Parameters.AddWithValue("@IndexToDestination", ktm.IndexToDestination);
                cmd.Parameters.AddWithValue("@Amount", ktm.Amount);
                cmd.Parameters.AddWithValue("@AfterDiscount", ktm.AfterDiscount);
                cmd.Parameters.AddWithValue("@Category", ktm.Category);
                cmd.Parameters.AddWithValue("@Type", ktm.TicketType);
                cmd.Parameters.AddWithValue("@NumberOfTicket", ktm.NumberOfTickets);
                cmd.Parameters.AddWithValue("@DateOfPurchase", ktm.ViewDateTime);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    // Send the email after inserting the data successfully
                    if (SendMail(ktm))
                    {
                        ViewBag.Message = "Mail successfully sent to " + ktm.EmailAddress;
                    }
                    else
                    {
                        ViewBag.Message = "Sending Mail Failed";
                    }
                }
                catch (SqlException ex)
                {
                    logger.LogError(ex, "SQL error occurred while inserting into the table.");
                    return RedirectToAction("Error", new { message = "SQL error: " + ex.Message });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while inserting into the table.");
                    return RedirectToAction("Error", new { message = "Error: " + ex.Message });
                }
                finally
                {
                    conn.Close();
                }

                return View("TrainTicketInvoice", ktm);
            }

            return View(ktm);
        }


        private bool SendMail(KtmUsers ktm)
        {
            var currentDestinationIndex = ktm.IndexCurrentDestination;
            var currentDestination = ktm.DictCurrentDestination.ContainsKey(currentDestinationIndex) ? ktm.DictCurrentDestination[currentDestinationIndex] : "Unknown";

            var subject = "Ticket Information " + ktm.Id;

            var body = "<table style=\"font-size:16px;\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">" +
                        "<tr>" +
                            "<td style=\"font-weight:700; padding-right: 20px;\">Ticket ID</td>" +
                            "<td>" + ktm.Id + "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td style=\"font-weight:700; padding-right: 20px;\">Name</td>" +
                            "<td>" + ktm.PurchaserName + "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td style=\"font-weight:700; padding-right: 20px;\">IC/Passport</td>" +
                            "<td>" + ktm.IdentityCardOrPassportNumber + "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td style=\"font-weight:700; padding-right: 20px;\">Email</td>" +
                            "<td>" + ktm.EmailAddress + "</td>" +
                        "</tr>" +
                    "</table>" +
                    "<br>" +
                    "<table style=\"font-size:16px; \" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">" +
                        "<tr style=\"font-weight:700;\">" +
                            "<td style=\"padding-right: 100px;\">From</td>" +
                            "<td>To</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td style=\"padding-right: 10px;\">" + currentDestination + "</td>" +
                            "<td>" + ktm.DictToDestination[ktm.IndexToDestination] + "</td>" +
                        "</tr>" +
                    "</table>" +
                    "<br>" +
                    "<table style=\"font-size:16px;\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">" +
                        "<tr>" +
                            "<td style=\"font-weight:700; padding-right: 20px;\">Regular Price</td>" +
                            "<td>" + ktm.Amount.ToString("RM0.00") + "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td style=\"font-weight:700; padding-right: 20px;\">Final Price</td>" +
                            "<td>" + ktm.AfterDiscount.ToString("RM0.00") + "</td>" +
                        "</tr>" +

                        "<tr>" +
                            "<td style=\"font-weight:700; padding-right: 20px;\">Date time transaction</td>" +
                            "<td>" + ktm.TicketDateTime + "</td>" +

                    "</table>";

            var mail = new Mail(configuration);

            return mail.Send(configuration["Gmail:Username"], ktm.EmailAddress, subject, body);
        }
        public IActionResult SearchIndex(string searchString = "")
        {
            IList<KtmUsers> dbList = GetDbList(out string errorMessage);
            var result = dbList.Where(x => x.Id.ToLower().Contains(searchString.ToLower()) ||
            x.PurchaserName.ToLower().Contains(searchString.ToLower()))
                .OrderBy(x => x.PurchaserName).ThenByDescending(x => x.NumberOfTickets);

            return View("Index", result);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            IList<KtmUsers> dbList = new List<KtmUsers>();
            SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ParcelConnStr"));
            string sql = @"SELECT * FROM KtmUser";
            SqlCommand cmd = new SqlCommand(sql, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dbList.Add(new KtmUsers()
                    {
                        ViewId = reader.GetString(0),
                        PurchaserName = reader.GetString(1),
                        IdentityCardOrPassportNumber = reader.GetString(2),
                        EmailAddress = reader.GetString(3),

                    });
                }
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "SQL error occurred while retrieving the KtmUser list.");
                return RedirectToAction("Error", new { message = "SQL error: " + ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving the KtmUser list.");
                return RedirectToAction("Error", new { message = "Error: " + ex.Message });
            }
            finally
            {
                conn.Close();
            }

            var ktm = dbList.FirstOrDefault(x => x.ViewId == id);
            if (ktm == null)
            {
                return RedirectToAction("Error", new { message = "User not found" });
            }

            return View(ktm);
        }

        [HttpPost]
        public IActionResult Edit(string id, KtmUsers ktm)
        {
            if (ModelState.IsValid)
            {
                SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ParcelConnStr"));
                SqlCommand cmd = new SqlCommand("spUpdateIntoTable", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", id); // Ensure ktm.Id is correctly populated
                cmd.Parameters.AddWithValue("@PurchaserName", ktm.PurchaserName);
                cmd.Parameters.AddWithValue("@IdentityCardOrPassportNumber", ktm.IdentityCardOrPassportNumber);
                cmd.Parameters.AddWithValue("@EmailAddress", ktm.EmailAddress);

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        logger.LogWarning("No rows were updated in the database. Check the provided Id: " + ktm.Id);
                        return RedirectToAction("Error", new { message = "No rows were updated. Please check the provided Id: " + ktm.Id });
                    }
                }
                catch (SqlException ex)
                {
                    logger.LogError(ex, "SQL error occurred while updating the table.");
                    return RedirectToAction("Error", new { message = "SQL error: " + ex.Message });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while updating the table.");
                    return RedirectToAction("Error", new { message = "Error: " + ex.Message });
                }
                finally
                {
                    conn.Close();
                }

                return RedirectToAction("Index");
            }
            return View(ktm);
        }

        [HttpGet]
        public IActionResult Delete(string id)
        {
            IList<KtmUsers> dblist = GetDbList(out string errorMessage);

            var result = dblist.First(x => x.ViewId == id);
            return View(result);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult ConfirmDelete(string id)
        {
            using (SqlConnection conn = new SqlConnection(configuration.GetConnectionString("ParcelConnStr")))
            using (SqlCommand cmd = new SqlCommand("spDeleteTicket", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while updating the table.");
                    return RedirectToAction("Error", new { message = "Error: " + ex.Message });
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public IActionResult Details(string id)
        {
            IList<KtmUsers> dblist = GetDbList(out string errorMessage);

            var result = dblist.First(x => x.ViewId == id);
            return View(result);
        }
    }
}
