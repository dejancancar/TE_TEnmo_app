using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient.DAL
{
    public class TransferApiDAO : ITransferDAO
    {
        private readonly RestClient client;

        public TransferApiDAO(string api_url)
        {
            client = new RestClient(api_url);
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
        }


        //PROBABLY SHOULD BE IN A USER DAO
        public List<API_User> GetUsers()
        {
            RestRequest request = new RestRequest($"users");
            //users

            IRestResponse<List<API_User>> response = client.Get<List<API_User>>(request);
            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                throw new Exception("Error occurred - unable to reach server: " + (int)response.StatusCode);
            }
            else
            {
                return response.Data;
            }
        }

        public List<Transfer> GetTransfers(string username)
        {
            RestRequest request = new RestRequest($"users/{username}/transfers");
            //users

            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);
            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                throw new Exception("Error occurred - unable to reach server: " + (int)response.StatusCode);
            }
            else
            {
                return response.Data;
            }
        }

        public bool SendMoney(int fromUserId, int toUserId, decimal amount)
        {
            RestRequest request = new RestRequest("transfers");

            //create new transfer with the info passed in
            Transfer newTransfer = new Transfer();
            newTransfer.AccountFrom = fromUserId;
            newTransfer.AccountTo = toUserId;
            newTransfer.Amount = amount;

            //pass through
            request.AddJsonBody(newTransfer);
            IRestResponse<Transfer> response = client.Post<Transfer>(request);
            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                throw new Exception("Error occurred - unable to reach server: " + (int)response.StatusCode);
                // TODO we need to change this to return false after we know this is successfully connecting to the server
                //return false;
            }
            else
            {
                return true;
            }
        }


    }
}
