﻿using System;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using static System.Console;
namespace ApiGateway
{
    class Program
    {
        private static LoyaltyProgramClient client;
        static void Main(string[] args)
        {
            client = new LoyaltyProgramClient("localhost:5000");
            WriteLine("Welcome to the API Gateway Mock.");

            var cont = true;
            while (cont)
            {
                WriteLine();
                WriteLine();
                WriteLine("********************");
                WriteLine("Choose one of:");
                WriteLine("q <userid> - to query the Loyalty Program Microservice for a user with id <userid>.");
                WriteLine("r <userid> - to register a user with id <userid> with the Loyalty Program Microservice.");
                WriteLine("u <userid> <interests> - to update a user with new comman separated interests");
                WriteLine("exit - to exit");
                WriteLine("********************");
                var cmd = ReadLine();
                cont = ProcessCommand(cmd);
            }
        }

        private static bool ProcessCommand(string cmd)
        {
            if ("exit".Equals(cmd))
                return false;
            if (cmd.StartsWith("q"))
                ProcessUserQuery(cmd);
            else if (cmd.StartsWith("r"))
                ProcessUserRegistration(cmd);
            else if (cmd.StartsWith("u"))
                ProcessUpdateUser(cmd);
            else
                WriteLine("Did not understand command :(");
            return true;
        }

        private static void ProcessUserQuery(string cmd)
        {
            int userId;
            if (!int.TryParse(cmd.Substring(1), out userId))
            {
                WriteLine("Please specify user id as an int");
            }
            else
            {
                var response = client.QueryUser(userId).Result;
                PrettyPrintResponse(response);
            }
        }

        private static void ProcessUserRegistration(string cmd)
        {
            var newUser = new LoyaltyProgramUser { Name = cmd.Substring(1).Trim() };
            var response = client.RegisterUser(newUser).Result;
            PrettyPrintResponse(response);
        }

        private static async void PrettyPrintResponse(HttpResponseMessage response)
        {
            WriteLine("Status code: " + response?.StatusCode.ToString() ?? "command failed");
            WriteLine("Headers: " + response?.Headers.Aggregate("", (acc, h) => acc + "\n\t" + h.Key + ": " + h.Value) ?? "");
            WriteLine("Body: " + await response?.Content.ReadAsStringAsync() ?? "");
        }

        private static async void ProcessUpdateUser(string cmd)
        {
            int userId;
            if (!int.TryParse(cmd.Split(' ').Skip(1).First(), out userId))
            {
                WriteLine("Plaese speciffy user id as an int");
            }
            else
            {
                var response = client.QueryUser(userId).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var user = JsonConvert.DeserializeObject<LoyaltyProgramUser>(await response.Content.ReadAsStringAsync());
                    var newInterests = cmd.Substring(cmd.IndexOf(' ', 2)).Split(',').Select(i => i.Trim());
                    user.Settings =
                      new LoyaltyProgramSettings
                      {
                          Interests =
                          user.Settings?.Interests.Union(newInterests).ToArray()
                          ?? newInterests.ToArray()
                      };
                    PrettyPrintResponse(client.UpdateUser(user).Result);
                }
            }
        }
    }
}
