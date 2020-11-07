using System;
using System.Numerics;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Cms;

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

        private bool WinForSomeOne(string status, char number)
        {
            for (var i = 0; i < 3; i++)
            {
                if (status[i] == number && status[i + 3] == number && status[i + 6] == number)
                {
                    return true;
                }
            }

            for (var i = 0; i < 3; i++)
            {
                if (status[3 * i] == number && status[3 * i + 1] == number && status[3 * i + 2] == number)
                {
                    return true;
                }
            }

            if (status[0] == number && status[4] == number && status[8] == number)
            {
                return true;
            }

            if (status[2] == number && status[4] == number && status[6] == number)
            {
                return true;
            }

            return false;
        }

        private int Win(string status)
        {
            if (WinForSomeOne(status, '1'))
            {
                return 1;
            }
            else if (WinForSomeOne(status, '2'))
            {
                return 2;
            }
            else if (status.IndexOf("0", StringComparison.Ordinal) == -1)
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }

        public async void DeleteGame(int gid)
        {
        }

        [HttpPost]
        public string Post([FromForm] string uid, [FromForm] string password, [FromForm] int seat)
        {
            var connection = Connection.GetConn();
            var sql = "select status,did,next,ready,gid from AllView where uid=@uid and password=@password";
            MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.Add(new MySqlParameter("@uid", uid));
            command.Parameters.Add(new MySqlParameter("@password", password));
            connection.Open();
            var result = command.ExecuteReader();
            var response = "";
            if (result.Read())
            {
                var did = result.GetString("did");
                var gid = result.GetString("gid");
                var s = result.GetString("status");
                response += result.GetInt32("did");
                response += ":";
                response += result.GetString("status");
                response += ":";
                response += result.GetInt32("next");
                response += ":";
                response += result.GetString("ready");
                response += ":";
                if (Win(s) != 0)
                {
                    result.Close();
                    sql = "update Desk set ready='00' where did=@did";
                    command=new MySqlCommand(sql,connection);
                    command.Parameters.Add(new MySqlParameter("@did", did));
                    command.ExecuteNonQuery();
                    Thread.Sleep(3000);
                    sql = "update Game set status='000000000',next=@next where gid=@gid";
                    command = new MySqlCommand(sql, connection);
                    command.Parameters.Add(new MySqlParameter("@next", new Random().Next(1, 2).ToString()));
                    command.Parameters.Add(new MySqlParameter("@gid", gid));
                    command.ExecuteNonQuery();
                    sql = "update User set score=score+@score where uid=@uid";
                    command = new MySqlCommand(sql, connection);
                    if (Win(s) == 3)
                    {
                        command.Parameters.Add(new MySqlParameter("@score", 1));
                    }
                    else if (Win(s) == seat)
                    {
                        command.Parameters.Add(new MySqlParameter("@score", -3));
                    }
                    else
                    {
                        command.Parameters.Add(new MySqlParameter("@score", 3));
                    }

                    command.Parameters.Add(new MySqlParameter("@uid", uid));
                    command.ExecuteNonQuery();
                }
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