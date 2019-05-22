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
    class MemViewModel : INotifyPropertyChanged
    {
        public MemViewModel()
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
            var response = await InfluxDbClientHelper.QueryAsync(" SELECT * FROM win_mem WHERE time> now() -  60s");
            //从集合中取出第一条数据
            Dictionary<string, ObservableCollection<MemTimeInfo>> dictionary = new Dictionary<string, ObservableCollection<MemTimeInfo>>();
            foreach (var valueList in response.Values)
            {
                if (!dictionary.ContainsKey(valueList[12].ToString()))
                {
                    dictionary.Add(valueList[12].ToString(), new ObservableCollection<MemTimeInfo>());
                }
                dictionary[valueList[12].ToString()].Add(new MemTimeInfo()
                {
                    Category = $"{(int)(DateTime.UtcNow - (DateTime)valueList[0]).TotalSeconds}s",
                    Value = double.Parse(valueList[1].ToString())
                });
            }
            var cpuTimeInfos = dictionary[response.Values[0][12].ToString()];
            CpuTimeInfos = new ObservableCollection<TelegrafChartTool.MemTimeInfo>(cpuTimeInfos);
            if (cpuTimeInfos.Any(i => i.Value > MaxCpuValue))
            {
                ErrorText = $"当前最高86%，超过阈值{MaxCpuValue}";
            }
        }
        private const double MaxCpuValue = 85;

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

        private ObservableCollection<MemTimeInfo> _cpuTimeInfos;
        public ObservableCollection<MemTimeInfo> CpuTimeInfos
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

    public class MemTimeInfo
    {
        public string Category { get; set; }
        public double Value { get; set; }
    }
}
