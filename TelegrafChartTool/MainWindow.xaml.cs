using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models;
using TelegrafChart.Business;
using Point = InfluxData.Net.InfluxDb.Models.Point;

namespace TelegrafChartTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            //AddData();
            GetData();
        }


        /// <summary>
        /// 从InfluxDB中读取数据
        /// </summary>
        public async void GetData()
        {
            //从指定库中查询数据
            var response = await InfluxDbClientHelper.QueryAsync(" SELECT * FROM win_disk WHERE time> now() -  5m ");
            //从集合中取出第一条数据
            //var info_model = response.FirstOrDefault();
        }
    }
}
