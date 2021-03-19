using MenuFramework;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.DAL;
using TenmoClient.Data;

namespace TenmoClient.Views
{
    public class MainMenu : ConsoleMenu
    {

        // TODO: INITILIZAE DAO'S HERE, AND SET THEM IN THE CONSTRUCTOR
        private IAccountDAO accountDao;
        private ITransferDAO transferDao;

        public MainMenu(string api_url)
        {
            this.accountDao = new AccountApiDAO(api_url);
            this.transferDao = new TransferApiDAO(api_url);

            // TODO: NEED TO UPDATE THE CONSTRUCTOR TO HAVE THE DAO'S PASSED IN, AND SET THEM IN THE CONSTRUCTOR
            AddOption("View your current balance", ViewBalance)
                .AddOption("View your past transfers", ViewTransfers) // case 5 and 6
                .AddOption("View your pending requests", ViewRequests) // case 8 and 9
                .AddOption("Send TE bucks", SendTEBucks) // case 4
                .AddOption("Request TE bucks", RequestTEBucks) // case 7
                .AddOption("Log in as different user", Logout)
                .AddOption("Exit", Exit);
        }

        protected override void OnBeforeShow()
        {
            Console.WriteLine($"TE Account Menu for User: {UserService.GetUserName()}");
        }

        private MenuOptionResult ViewBalance()
        {
            try
            {
                // create a rest request to the /users/username/account# url, get back a balance
                int accountId = MainMenu.GetInteger("Please enter your account Id: ");

                // TODO: THIS WILL CALL THE ACCOUNTDAO AND IT WILL RETURN AN ACCOUNT (GETACCOUNT METHOD).  WE WILL USE THAT ACCOUNT TO REFERENCE THE BALANCE BY ACCOUNT.BALANCE
                // UserService.GetUserName
                Account account = accountDao.GetAccount(accountId);
                //Account account1 = accountDao.GetAccount(UserService.GetUserName(), accountId);
                Console.WriteLine($"Your account {accountId} has the balance of: {account.Balance}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult ViewTransfers()
        {
            try
            {
                string toUsername = "";
                string fromUsername = "";
                string type = "";

                List<Transfer> transfers = transferDao.GetTransfers(UserService.GetUserName());
                List<API_User> users = transferDao.GetUsers();

                Console.WriteLine("--------------------------------------------------------------");
                Console.WriteLine("Transfers");
                Console.WriteLine($"{"ID",-5}          {"From/To",-20}                 {"Amount",-10}");
                Console.WriteLine("--------------------------------------------------------------");

                foreach (API_User user in users)
                {
                    foreach (Transfer transfer in transfers)
                    {
                        if (user.UserId == transfer.AccountTo && user.Username != UserService.GetUserName())
                        {
                            type = ($"Type: Send");
                            fromUsername = UserService.GetUserName();
                            toUsername =  user.Username;

                            Console.WriteLine($"{transfer.TransferId,-5}            {$"To: {toUsername}",-20}              {transfer.Amount,-10}");
                        }
                        if (user.UserId == transfer.AccountFrom && user.Username != UserService.GetUserName())
                        {
                            type = ($"Type: Receive");
                            fromUsername = user.Username;
                            toUsername = UserService.GetUserName();
                            
                            Console.WriteLine($"{transfer.TransferId,-5}            {$"From: {fromUsername}",-20}              {transfer.Amount,-10}");
                        }
                    }

                }

                Console.WriteLine("--------------------------------------------------------------");
                Console.WriteLine("");
                bool badInput = true;
                int transferId = -1;
                //loop until we find a user id that is actually in the list
                while (badInput)
                {

                    transferId = GetInteger("Enter Transfer ID to view (0 to cancel): ");
                    if (transferId == 0)
                    {
                        return MenuOptionResult.WaitAfterMenuSelection;
                    }
                    foreach (Transfer transfer in transfers)
                    {
                        if (transfer.TransferId == transferId)
                        {
                            Console.Clear();
                            Console.WriteLine("--------------------------------------------------------------");
                            Console.WriteLine("Transfer Details");
                            Console.WriteLine("--------------------------------------------------------------");
                            Console.WriteLine($"Id: {transfer.TransferId}");
                            Console.WriteLine($"From: {fromUsername}");
                            Console.WriteLine($"To: {toUsername}");
                            Console.WriteLine($"{type}");
                            Console.WriteLine($"Status: Approved");
                            Console.WriteLine($"Amount: {transfer.Amount:C2}");
                            badInput = false;
                        }
                    }
                    if (badInput)
                    {
                        Console.WriteLine("Please enter a valid transfer ID!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult ViewRequests()
        {
            Console.WriteLine("Not yet implemented!");
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult SendTEBucks()
        {
            try
            {
                List<API_User> users = transferDao.GetUsers();
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine("Users");
                Console.WriteLine("ID                           Name");
                Console.WriteLine("-------------------------------------------");
                foreach (API_User user in users)
                {
                    Console.WriteLine($"{user.UserId}                           {user.Username}");
                }
                Console.WriteLine("---------");
                Console.WriteLine("");


                bool badInput = true;
                int toUserId = -1;
                //loop until we find a user id that is actually in the list
                while (badInput)
                {
                    toUserId = GetInteger("Enter ID of user you are sending to (0 to cancel): ");
                    if (toUserId == 0)
                    {
                        return MenuOptionResult.WaitAfterMenuSelection;
                    }
                    foreach (API_User user in users)
                    {
                        if (user.UserId == toUserId && toUserId != UserService.GetUserId())
                        {
                            badInput = false;
                        }
                    }
                    if (badInput)
                    {
                        Console.WriteLine("Please enter a valid User Id");
                    }
                }

                // make sure amount is greater than 0
                badInput = true;
                decimal amount = -1;
                while (badInput)
                {
                    amount = GetInteger("Enter amount: ");
                    if (amount <= 0)
                    {
                        Console.WriteLine("Please enter an amount greater than 0");
                    }
                    else
                    {
                        badInput = false;
                    }
                }

                // check to make sure your balance has enough money in it to transfer to the other account
                Account fromAccount = accountDao.GetAccount(UserService.GetUserId());
                if (amount > fromAccount.Balance)
                {
                    Console.WriteLine("Insufficient balance");
                    return MenuOptionResult.WaitAfterMenuSelection;
                }
                else
                {
                    bool transferSuccessful = transferDao.SendMoney(fromAccount.AccountId, toUserId, amount);
                    if (transferSuccessful)
                    {
                        Console.WriteLine("TRANSACTION APPROVED!");
                    }
                    else
                    {
                        Console.WriteLine("TRANSACTION FAILED");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return MenuOptionResult.WaitAfterMenuSelection;
        }


        // THIS NEEDS TO BE UPDATED - JUST COPY AND PASTED STUFF
        private MenuOptionResult RequestTEBucks()
        {
            try
            {
                List<API_User> users = transferDao.GetUsers();
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine("Users");
                Console.WriteLine("ID                           Name");
                Console.WriteLine("-------------------------------------------");
                foreach (API_User user in users)
                {
                    Console.WriteLine($"{user.UserId}                           {user.Username}");
                }
                Console.WriteLine("---------");
                Console.WriteLine("");
                bool badInput = true;
                int userId = -1;

                //loop until we find a user id that is actually in the list
                while (badInput)
                {
                    userId = GetInteger("Enter ID of user you are requesting from (0 to cancel): ");
                    if (userId == 0)
                    {
                        return MenuOptionResult.WaitAfterMenuSelection;
                    }
                    foreach (API_User user in users)
                    {
                        if (user.UserId == userId)
                        {
                            badInput = false;
                        }
                    }
                    if (badInput)
                    {
                        Console.WriteLine("Please enter a valid User Id");
                    }
                }

                // TODO: PUT IN LOGIC HERE TO MAKE SURE AMOUNT IS GREATER THAN 0
                decimal amount = GetInteger("Enter amount: ");
                Account account = accountDao.GetAccount(userId);
                if (amount > account.Balance)
                {
                    Console.WriteLine("Insufficient balance");
                    return MenuOptionResult.WaitAfterMenuSelection;
                }
                else
                {

                }



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return MenuOptionResult.WaitAfterMenuSelection;

        }

        private MenuOptionResult Logout()
        {
            UserService.SetLogin(new API_User()); //wipe out previous login info
            return MenuOptionResult.CloseMenuAfterSelection;
        }

    }
}
