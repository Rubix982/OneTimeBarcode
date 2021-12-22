using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared;
using Shared.Models;
using Xamarin.Auth;

namespace ExpiringBarcodeDemo.Handler
{
    public class Api
    {
        private const string serverUrl = "http://expiringbarcodedemo.gear.host/";

        private string token;
        private User user;

        public Api(User user)
        {
            this.user = user;
        }

        private void AddAuthHeader(HttpClient client)
        {
            if (user.IsLoggedIn)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                    user.GetProperty(UserConstants.Token));
            }
        }

        public async Task<bool> Login(string username, string password)
        {
            try
            {
                var info = new[]
                {
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("grant_type", "password")
                };

                var content = new FormUrlEncodedContent(info);
                var client = new HttpClient();
                var response = await client.PostAsync(serverUrl + "token", content);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }
                var strResponse = await response.Content.ReadAsStringAsync();
                var resp = JsonConvert.DeserializeObject<LoginResultModel>(strResponse);

                user.SetProperty(UserConstants.Email, username);
                user.SetProperty(UserConstants.Token, resp.access_token);
                return true;
            }
            catch (WebException ex)
            {
                handleWebException(ex);
                throw;
            }
        }

        public async Task LogOut()
        {
            try
            {
                await PostApi<string>("api/Account/Logout", null, authenticate: true);
            }
            catch (UnauthorizedAccessException)
            {
                //loggin out. so unauthorizedaccess doesn't matter.
            }
        }

        public async Task RequestSharedKey()
        {
            try
            {
                DiffieHellman DH = new DiffieHellman();
                var mypublic = DH.GetMyPublic();
                var serverKey =
                    await PostApi<string>("api/barcode", new { key = mypublic }, authenticate: true, throwIfNotOK: true);

                var key = DH.getFinalKey(BigInteger.Parse(serverKey));

                // assume logged in if you're calling this.
                // not checking here.
                var stringKey = Convert.ToBase64String(key);
                user.SetProperty(UserConstants.SECRET, stringKey);
            }
            catch (WebException e)
            {
                handleWebException(e);
            }
        }

        public async Task GetMembershipId()
        {
            try
            {
                var memberId =
                    await GetApi<string>("api/member", authenticate: true, throwIfNotOK: true);
                user.SetProperty(UserConstants.MemberID, memberId);
            }
            catch (WebException e)
            {
                handleWebException(e);
            }
        }

        public async Task<RegisterResultModel> Register(string username, string password, string confirmpassword)
        {
            return await PostApi<RegisterResultModel>("api/account/register", new { email = username, password, confirmpassword });
        }

        private async Task<T> PostApi<T>(string invoke, dynamic param, bool throwIfNotOK = false, bool authenticate = false)
        {
            var json = JsonConvert.SerializeObject(param);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var client = new HttpClient();
            if (authenticate)
            {
                AddAuthHeader(client);
            }
            var response = await client.PostAsync(serverUrl + invoke, content);

            if (authenticate && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var strResponse = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK && throwIfNotOK)
            {
                throw new WebException(response.StatusCode + " " + strResponse);
            }
            return JsonConvert.DeserializeObject<T>(strResponse);
        }

        private async Task<T> GetApi<T>(string invoke, bool throwIfNotOK = false, bool authenticate = false)
        {
            var client = new HttpClient();
            if (authenticate)
            {
                AddAuthHeader(client);
            }
            var response = await client.GetAsync(serverUrl + invoke);

            var strResponse = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK && throwIfNotOK)
            {
                throw new WebException(response.StatusCode + " " + strResponse);
            }
            return JsonConvert.DeserializeObject<T>(strResponse);
        }

        private void handleWebException(WebException ex)
        {
            try
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var err = reader.ReadToEnd();
                    Console.WriteLine(err);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("could not write response error from server");
                Console.WriteLine(ex.Message);
            }
        }
    }
}