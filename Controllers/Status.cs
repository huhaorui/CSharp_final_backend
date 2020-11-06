using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace backend.Controllers
{
    [Route("api/game/status")]
    [ApiController]
    public class Status : ControllerBase
    {
        private readonly ILogger<Status> _logger;

        public Status(ILogger<Status> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public string Post([FromForm] string uid, [FromForm] string password)
        {
            var connection = Connection.GetConn();
            var sql = "select status,did,next,ready from AllView where uid=@uid and password=@password";
            MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.Add(new MySqlParameter("@uid", uid));
            command.Parameters.Add(new MySqlParameter("@password", password));
            connection.Open();
            var result = command.ExecuteReader();
            var response = "";
            if (result.Read())
            {
                response += result.GetInt32("did");
                response += ":";
                response += result.GetString("status");
                response += ":";
                response += result.GetInt32("next");
                response += ":";
                response += result.GetString("ready");
            }
            else
            {
                response += "NG"; //No Game
            }

            result.Close();
            return response;
        }
    }
}