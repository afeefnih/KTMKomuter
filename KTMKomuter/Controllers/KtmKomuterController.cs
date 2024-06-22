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
            return View(ktm);
        }


        [HttpPost]
        public IActionResult TrainTicket(KtmUsers ktm)
        {
            if (ModelState.IsValid)
            {
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

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
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
                        IndexCurrentDestination = reader.GetInt32(4),
                        IndexToDestination = reader.GetInt32(5),
                        Amount = reader.GetDouble(6),
                        AfterDiscount = reader.GetDouble(7),
                        Category = reader.GetInt32(8),
                        TicketType = reader.GetInt32(9),
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

                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@PurchaserName", ktm.PurchaserName);
                cmd.Parameters.AddWithValue("@IdentityCardOrPassportNumber", ktm.IdentityCardOrPassportNumber);
                cmd.Parameters.AddWithValue("@EmailAddress", ktm.EmailAddress);
                cmd.Parameters.AddWithValue("@indexCurrentDestination", ktm.IndexCurrentDestination);
                cmd.Parameters.AddWithValue("@indexToDestination", ktm.IndexToDestination);
                cmd.Parameters.AddWithValue("@amount", ktm.Amount);
                cmd.Parameters.AddWithValue("@afterdiscount", ktm.AfterDiscount);
                cmd.Parameters.AddWithValue("@category", ktm.Category);
                cmd.Parameters.AddWithValue("@type", ktm.TicketType);

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        logger.LogWarning("No rows were updated in the database. Check the provided Id.");
                        return RedirectToAction("Error", new { message = "No rows were updated. Please check the provided Id." });
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

        public IActionResult SendMail(string id)
        {
            string errorMessage;
            IList<KtmUsers> dbList = GetDbList(out errorMessage);

            if (errorMessage != null)
            {
                return RedirectToAction("Error", new { message = errorMessage });
            }

            var ktm = dbList.FirstOrDefault(x => x.ViewId == id);

            if (ktm == null)
            {
                return RedirectToAction("Error", new { message = "User not found." });
            }

            var currentDestinationIndex = ktm.IndexCurrentDestination;
            var currentDestination = ktm.DictCurrentDestination.ContainsKey(currentDestinationIndex) ? ktm.DictCurrentDestination[currentDestinationIndex] : "Unknown";

            var subject = "Ticket Information " + ktm.ViewId;


            var body = "<table style=\"font-size:16px;\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">" +
                        "<tr>" +
                            "<td style=\"font-weight:700; padding-right: 20px;\">Ticket ID</td>" +
                            "<td>" + ktm.ViewId + "</td>" +
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
                            "<td>" + ktm.Amount.ToString("c2") + "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td style=\"font-weight:700; padding-right: 20px;\">Final Price</td>" +
                            "<td>" + ktm.AfterDiscount.ToString("c2") + "</td>" +
                        "</tr>" +
                    "</table>";

            var mail = new Mail(configuration);

            if (mail.Send(configuration["Gmail:Username"], ktm.EmailAddress, subject, body))
            {
                ViewBag.Message = "Mail successfully sent to " + ktm.EmailAddress;
                ViewBag.Body = body;
            }
            else
            {
                ViewBag.Message = "Sending Mail Failed";
                ViewBag.Body = "";
            }

            return View(ktm);
        }


        public IActionResult Details(string id)
        {
            IList<KtmUsers> dblist = GetDbList(out string errorMessage);

            var result = dblist.First(x => x.ViewId == id);
            return View(result);
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


    }
}
