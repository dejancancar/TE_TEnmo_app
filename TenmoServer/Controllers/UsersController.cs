using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private IAccountDAO AccountDAO;
        private IUserDAO UserDAO;
        private ITransferDAO TransferDAO;

        public UsersController(IAccountDAO accountDAO, IUserDAO userDAO, ITransferDAO transferDAO)
        {
            this.AccountDAO = accountDAO;
            this.UserDAO = userDAO;
            this.TransferDAO = transferDAO;
        }

        [HttpGet]
        //admin
    //    [Authorize(Roles = "Admin")]
        public List<User> GetUsers()
        {
            return UserDAO.GetUsers();
        }

        //[HttpGet("userId")]
        //public User GetUserById(int userId)
        //{
        //    return UserDAO.GetUserById(userId);
        //}



        [HttpGet("{username}/accounts")]
      //  [Authorize(Roles = "User, Admin")]
        public ActionResult<List<Account>> GetAccountsByUsername(string username)
        {
             //todo if it they don't have access, return an empty list IF WE WANT
            if (username.ToLower() != User.Identity.Name && !User.IsInRole("Admin")) // <- this is magic
            {
                return NotFound();
            }
            List<Account> accounts = AccountDAO.GetAccounts(username);
            if(accounts == null)
            {
               return NotFound();
            }
            return accounts;
        }



        //[HttpGet("{username}/transfers/{userId}")]
        ////[Authorize(Roles = "User, Admin")]
        //public ActionResult<Account> GetAccountInfo(string username, int userId)
        //{
        //    if (username.ToLower() != User.Identity.Name && !User.IsInRole("Admin")) // <- this is magic
        //    {
        //        return NotFound();
        //    }
        //    Account userAccount = AccountDAO.GetAccount(username, userId);
        //    if (userAccount == null)
        //    {
        //        return NotFound();
        //    }
        //    return userAccount;
        //}

        [HttpGet("{username}/transfers")]
        public ActionResult<List<Transfer>> GetTransfers(string username)
        {
            //if (username.ToLower() != User.Identity.Name && !User.IsInRole("Admin"))
            //{
            //    return NotFound();
            //}
            List<Transfer> userTransfers = TransferDAO.GetTransfers(username);
            if (userTransfers == null)
            {
                return NoContent();
            }
            return userTransfers;
        }
    }
}
