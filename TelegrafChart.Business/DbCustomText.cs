using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegrafChart.Business
{
    public static class DbCustomText
    {            
        //连接InfluxDb的API地址、账号、密码
        public static string InfuxUrl = "http://localhost:8086/";
        public static string InfuxUser = "admin";
        public static string InfuxPwd = "admin";

        public static string InfuxDbName = "telegraf";
    }
}
