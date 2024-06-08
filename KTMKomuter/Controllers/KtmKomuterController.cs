﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Collections.Generic;
using KTMKomuter.Models;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Security.Policy;
using System.Collections;

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
                        AfterDiscount = reader.GetDouble(7)
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
                        AfterDiscount = reader.GetDouble(7)
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

                cmd.Parameters.AddWithValue("@Id", id);  // Ensure the correct Id is passed
                cmd.Parameters.AddWithValue("@PurchaserName", ktm.PurchaserName);
                cmd.Parameters.AddWithValue("@IdentityCardOrPassportNumber", ktm.IdentityCardOrPassportNumber);
                cmd.Parameters.AddWithValue("@EmailAddress", ktm.EmailAddress);

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



    }
}
