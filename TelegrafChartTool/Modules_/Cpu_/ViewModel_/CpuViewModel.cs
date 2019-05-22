using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelegrafChart.Business;
using TelegrafChartTool.Annotations;
using Telerik.Windows.Controls;
using Timer = System.Timers.Timer;

namespace TelegrafChartTool
{
    class CpuViewModel : INotifyPropertyChanged
    {
        public CpuViewModel()
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
            var response = await InfluxDbClientHelper.QueryAsync(" SELECT * FROM win_disk WHERE time> now() -  60s");
            //从集合中取出第一条数据
            Dictionary<string, ObservableCollection<TelegrafChartTool.CpuTimeInfo>> dictionary = new Dictionary<string, ObservableCollection<CpuTimeInfo>>();
            foreach (var valueList in response.Values)
            {
                if (!dictionary.ContainsKey(valueList[9].ToString()))
                {
                    dictionary.Add(valueList[9].ToString(), new ObservableCollection<CpuTimeInfo>());
                }
                dictionary[valueList[9].ToString()].Add(new TelegrafChartTool.CpuTimeInfo()
                {
                    Category = $"{(int)(DateTime.UtcNow - (DateTime)valueList[0]).TotalSeconds}s",
                    Value = 100.0 - double.Parse(valueList[7].ToString())
                });
            }

            var cpuTimeInfos = dictionary[response.Values[0][9].ToString()];
            CpuTimeInfos = new ObservableCollection<TelegrafChartTool.CpuTimeInfo>(cpuTimeInfos);
            if (cpuTimeInfos.Any(i => i.Value > MaxCpuValue))
            {
                ErrorText = $"当前最高{cpuTimeInfos.Max(i => i.Value)}超过阈值{MaxCpuValue}";
            }
        }
        private const double MaxCpuValue = 80;

        private string _errorText;
        public string ErrorText
        {
            get => _errorText;
            set
            {
                _errorText = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CpuTimeInfo> _cpuTimeInfos;
        public ObservableCollection<CpuTimeInfo> CpuTimeInfos
        {
            get => _cpuTimeInfos;
            set
            {
                _cpuTimeInfos = value;
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

    public class CpuTimeInfo
    {
        public string Category { get; set; }
        public double Value { get; set; }
    }
}
