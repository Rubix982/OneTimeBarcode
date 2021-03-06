using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared;
using Shared.Models;

using Android.Util;

namespace ExpiringBarcodesAndroidAppNew.Handler
{
    public class Api
    {
        private const string serverUrl = "http://192.168.1.5:60203/";

        private readonly User user;

        public Api(User user)
        {
            this.user = user;

            Console.WriteLine("API Created");
            Log.Info("Barcode", "API has been created");
        }

        private void AddAuthHeader(HttpClient client)
        {
            if (user.IsLoggedIn)
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
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
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return false;
                }
                var strResponse = await response.Content.ReadAsStringAsync();
                var resp = JsonConvert.DeserializeObject<LoginResultModel>(strResponse);

                user.SetProperty(UserConstants.Email, username);
                user.SetProperty(UserConstants.Token, resp.access_token);
                return true;
            }
            catch (System.Net.WebException ex)
            {
                HandleWebException(ex);
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
            catch (System.Net.WebException e)
            {
                HandleWebException(e);
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
            catch (System.Net.WebException e)
            {
                HandleWebException(e);
            }
        }

        public async Task<RegisterResultModel> Register(string username, string password, string confirmpassword)
        {
            Log.Info("MK", $"Request payload: {username}");

            RegisterResultModel result = null;
            try
            {
                result = await PostApi<RegisterResultModel>("api/Account/Register", new { email = username, password, confirmpassword });
            } catch (Exception ex)
            {
                Log.Error("MK", $"Error occured while registering: {ex.Message}");
            }
            
            Log.Info("MK", $"Recieved response: {result}");


            return result;
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

            if (authenticate && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var strResponse = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != System.Net.HttpStatusCode.OK && throwIfNotOK)
            {
                throw new System.Net.WebException(response.StatusCode + " " + strResponse);
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
            if (response.StatusCode != System.Net.HttpStatusCode.OK && throwIfNotOK)
            {
                throw new System.Net.WebException(response.StatusCode + " " + strResponse);
            }
            return JsonConvert.DeserializeObject<T>(strResponse);
        }

        private void HandleWebException(System.Net.WebException ex)
        {
            try
            {
                using var stream = ex.Response.GetResponseStream();
                using var reader = new StreamReader(stream);
                var err = reader.ReadToEnd();
                Console.WriteLine(err);
            }
            catch (Exception)
            {
                Console.WriteLine("could not write response error from server");
                Console.WriteLine(ex.Message);
            }
        }
    }
}