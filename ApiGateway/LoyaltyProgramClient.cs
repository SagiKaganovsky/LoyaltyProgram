using Newtonsoft.Json;
using Polly;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway
{
    public class LoyaltyProgramClient
    {
        private static Policy exponentialRetryPolicy =
          Policy
            .Handle<Exception>()
            .WaitAndRetry(
              3,
              attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
              (_, __) => Console.WriteLine("retrying..." + _));

        private string hostName;

        public LoyaltyProgramClient(string loyalProgramMicroserviceHostName)
        {
            this.hostName = loyalProgramMicroserviceHostName;
        }

        public async Task<HttpResponseMessage> QueryUser(int userId)
        {
            return await exponentialRetryPolicy.Execute(() => DoUserQuery(userId));
        }

        private async Task<HttpResponseMessage> DoUserQuery(int userId)
        {
            var userResource = $"/users/{userId}";
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri($"http://{this.hostName}");
                httpClient.DefaultRequestHeaders.Add("Accept" , "*/*");
               var response = await httpClient.GetAsync(userResource);
                ThrowOnTransientFailure(response);
                return response;
            }
        }

        private static void ThrowOnTransientFailure(HttpResponseMessage response)
        {
            if (((int)response.StatusCode) < 200 || ((int)response.StatusCode) > 499) throw new Exception(response.StatusCode.ToString());
        }

        public async Task<HttpResponseMessage> RegisterUser(LoyaltyProgramUser newUser)
        {
            return await exponentialRetryPolicy.Execute(() => DoRegisterUser(newUser));
        }

        private async Task<HttpResponseMessage> DoRegisterUser(LoyaltyProgramUser newUser)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri($"http://{this.hostName}");
                httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
                var response = await httpClient.PostAsync("/users/", new StringContent(JsonConvert.SerializeObject(newUser), Encoding.UTF8, "application/json"));
                ThrowOnTransientFailure(response);
                return response;
            }
        }

        public async Task<HttpResponseMessage> UpdateUser(LoyaltyProgramUser user)
        {
            return await exponentialRetryPolicy.Execute(() => DoUpdateUser(user));
        }

        private async Task<HttpResponseMessage> DoUpdateUser(LoyaltyProgramUser user)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri($"http://{this.hostName}");
                httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
                var response = await httpClient.PutAsync($"/users/{user.Id}", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
                ThrowOnTransientFailure(response);
                return response;
            }
        }
    }
}