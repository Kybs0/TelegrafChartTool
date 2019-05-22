using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace TelegrafChart.Business
{
    public class InfluxDbClientHelper
    {
        //创建InfluxDbClient实例
        public static InfluxDbClient InfluxDbClientInstance { get; } = new InfluxDbClient(DbCustomText.InfuxUrl, DbCustomText.InfuxUser, DbCustomText.InfuxPwd, InfluxDbVersion.Latest);


        /// <summary>
        /// 从InfluxDB中读取数据
        /// </summary>
        public static async Task<IList<IList<object>>> QueryDiskAsync()
        {
            //传入查询命令，支持多条
            var sqlString = "SELECT * FROM win_disk WHERE time> now() -  5m";
            var serie = await QueryAsync(sqlString);
            var list = serie.Values;
            return list;
        }

        /// <summary>
        /// 从InfluxDB中读取数据
        /// </summary>
        public static async Task<Serie> QueryAsync(string sqlString)
        {
            //传入查询命令，支持多条
            var queries = new[] { sqlString };

            //从指定库中查询数据
            var response = await InfluxDbClientInstance.Client.QueryAsync(queries, DbCustomText.InfuxDbName);
            //得到Serie集合对象（返回执行多个查询的结果）
            var series = response.ToList();
            //取出第一条命令的查询结果，是一个集合
            return series[0];
        }
    }
}
