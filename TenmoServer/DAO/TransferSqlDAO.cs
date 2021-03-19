﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDAO : ITransferDAO
    {
        private readonly string connectionString;

        public TransferSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }



        //create a transfer method, return a tranfer 
        public Transfer CreateTransfer(Transfer transfer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES (2, 2, @accountfrom, @accountto, @amount); SELECT @@IDENTITY;", conn);
                    cmd.Parameters.AddWithValue("@accountfrom", transfer.AccountFrom);
                    cmd.Parameters.AddWithValue("@accountto", transfer.AccountTo);
                    cmd.Parameters.AddWithValue("@amount", transfer.Amount);
                    transfer.TransferId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return transfer;
        }

        //list of transfers
        public List<Transfer> GetTransfers()
        {
            List<Transfer> transfers = new List<Transfer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM transfers", conn);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Transfer transfer = RowToObject(rdr);
                        transfers.Add(transfer);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return transfers;
        }

        public List<Transfer> GetTransfers(string username)
        {
            List<Transfer> transfers = new List<Transfer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM transfers WHERE account_from = (SELECT user_id from users WHERE username = @username) Or account_to =(SELECT user_id from users WHERE username = @username)", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Transfer transfer = RowToObject(rdr);
                        transfers.Add(transfer);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return transfers;
        }

        private static Transfer RowToObject(SqlDataReader rdr)
        {
            Transfer transfer = new Transfer();
            transfer.AccountFrom = Convert.ToInt32(rdr["account_from"]);
            transfer.AccountTo = Convert.ToInt32(rdr["account_to"]);
            transfer.Amount = Convert.ToDecimal(rdr["amount"]);
            transfer.TransferId = Convert.ToInt32(rdr["transfer_id"]);
            transfer.TransferStatusId = Convert.ToInt32(rdr["transfer_status_id"]);
            transfer.TransferTypeId = Convert.ToInt32(rdr["transfer_type_id"]);
            return transfer;
        }
    }
}
