using IntelligentAssignment.Services;
using IntelligentAssignment.Services.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static IntelligentAssignment.Services.SvBase;

namespace IntelligentAssignment.Console.YingLekPro2
{
    class Program
    {

        static BaseConfig _BaseConfigs = new BaseConfig();  
        static int _curyeeid = 0;
        static void Main(string[] args)
        {
            LoadConfig();
            var curdate = DateTime.Now;
            //System.Console.WriteLine("Start1 " + curdate.ToLocalTime().ToString("0:HH:mm:ss.fff"));
            curdate = new DateTime(curdate.Year, curdate.Month, curdate.Day, curdate.Hour, curdate.Minute, curdate.Second);

            TimeSpan interval = new TimeSpan(curdate.Day, 5, 45, 0, 0);
            TimeSpan interval2 = new TimeSpan(curdate.Day, curdate.Hour, curdate.Minute, 0, 0);      
            _curyeeid = (int)Math.Round((interval2.TotalMinutes - interval.TotalMinutes) / 15.00);
            _curyeeid = (_curyeeid < 0) ? 96 + _curyeeid : _curyeeid;
            if (_curyeeid > 0 && _curyeeid <= 88)
            {
                Random rnd = new Random();
                string rs = "";
                System.Threading.Thread.Sleep(_BaseConfigs.YeekeeThreadDelay);
                Parallel.ForEach(_BaseConfigs.AccountConfigs, item =>
                {
                    int num = rnd.Next(0, 99999);
                  
                    rs = yingMWin(item.CookieMoveWin, item.TokenMoveWin, _curyeeid.ToString(), num.ToString("00000"));

                });
            }

        }

        private static void LoadConfig()
        {
            using (StreamReader r = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "UserConfig.json"))
            {
                string json = r.ReadToEnd();
                _BaseConfigs = JsonConvert.DeserializeObject<BaseConfig>(json);
            }
        }

        private static string yingMWin(string cookiekey, string tokenMoveWin, string numid, string numying)
        {
            var handler = new HttpClientHandler();
            handler.UseCookies = false;

            // If you are using .NET Core 3.0+ you can replace `~DecompressionMethods.None` to `DecompressionMethods.All`
            handler.AutomaticDecompression = ~DecompressionMethods.None;

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://new.movewinbet.com/member/yeekee/" + numid + "/bet-number"))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "*/*");
                    request.Headers.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.5");
                    request.Headers.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");
                    request.Headers.TryAddWithoutValidation("Origin", "https://new.movewinbet.com");
                    request.Headers.TryAddWithoutValidation("Connection", "keep-alive");
                    request.Headers.TryAddWithoutValidation("TE", "Trailers");
                    request.Headers.TryAddWithoutValidation("Cookie", cookiekey);
                    request.Headers.TryAddWithoutValidation("X-CSRF-TOKEN", tokenMoveWin);
                    request.Content = new StringContent(@"number=" + numying + "");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded; charset=UTF-8");

                    var response = httpClient.SendAsync(request).Result;
                    //System.Console.WriteLine(response.Headers.Date.Value.LocalDateTime);
                    //System.Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    System.Console.WriteLine("Finish " + numying + response.Headers.Date.Value.ToString("0:HH:mm:ss.fff"));
                    //System.Console.WriteLine("Finish " + numying + DateTime.Now.ToString("0:HH:mm:ss.fff"));
                    return response.Content.ReadAsStringAsync().Result;
                }
            }

        }

    }
}
