using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TelegrafChart.Business;
using TelegrafChartTool.Annotations;

namespace TelegrafChartTool
{
    class NetViewModel : INotifyPropertyChanged
    {
        public NetViewModel()
        {
            var timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += (s, e) => { GetData(); };
            timer.Start();
        }

        /// <summary>
        /// 从InfluxDB中读取数据
        /// </summary>
        public async void GetData()
        {
            //从指定库中查询数据
            var response = await InfluxDbClientHelper.QueryAsync(" SELECT * FROM win_net WHERE time> now() -  120s");
            //从集合中取出第一条数据
            Dictionary<string, ObservableCollection<TelegrafChartTool.NetTimeInfo>> sendDictionary = new Dictionary<string, ObservableCollection<NetTimeInfo>>();
            Dictionary<string, ObservableCollection<TelegrafChartTool.NetTimeInfo>> receiveDictionary = new Dictionary<string, ObservableCollection<NetTimeInfo>>();
            foreach (var valueList in response.Values)
            {
                if (!sendDictionary.ContainsKey(valueList[9].ToString()))
                {
                    sendDictionary.Add(valueList[9].ToString(), new ObservableCollection<NetTimeInfo>());
                }
                sendDictionary[valueList[9].ToString()].Add(new TelegrafChartTool.NetTimeInfo()
                {
                    Category = $"{(int)(DateTime.UtcNow - (DateTime)valueList[0]).TotalSeconds}s",
                    Value = double.Parse(valueList[1].ToString())
                });

                if (!receiveDictionary.ContainsKey(valueList[9].ToString()))
                {
                    receiveDictionary.Add(valueList[9].ToString(), new ObservableCollection<NetTimeInfo>());
                }
                receiveDictionary[valueList[9].ToString()].Add(new TelegrafChartTool.NetTimeInfo()
                {
                    Category = $"{(int)(DateTime.UtcNow - (DateTime)valueList[0]).TotalSeconds}s",
                    Value = double.Parse(valueList[2].ToString())
                });
            }
            SendTimeInfos = new ObservableCollection<TelegrafChartTool.NetTimeInfo>(sendDictionary[response.Values[0][9].ToString()]);
            ReceiveTimeInfos = new ObservableCollection<TelegrafChartTool.NetTimeInfo>(receiveDictionary[response.Values[0][9].ToString()]);
        }
        private ObservableCollection<NetTimeInfo> _sendTimeInfos;
        public ObservableCollection<NetTimeInfo> SendTimeInfos
        {
            get => _sendTimeInfos;
            set
            {
                _sendTimeInfos = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<NetTimeInfo> _receiveTimeInfos;
        public ObservableCollection<NetTimeInfo> ReceiveTimeInfos
        {
            get => _receiveTimeInfos;
            set
            {
                _receiveTimeInfos = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class NetTimeInfo
    {
        public string Category { get; set; }
        public double Value { get; set; }
    }
}
