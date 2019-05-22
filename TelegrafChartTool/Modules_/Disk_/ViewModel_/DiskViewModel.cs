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
    class DiskViewModel : INotifyPropertyChanged
    {
        public DiskViewModel()
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
            var response = await InfluxDbClientHelper.QueryAsync(" SELECT * FROM win_disk WHERE time> now() -  120s");
            //从集合中取出第一条数据
            Dictionary<string, ObservableCollection<TelegrafChartTool.DiskTimeInfo>> dictionary = new Dictionary<string, ObservableCollection<DiskTimeInfo>>();
            foreach (var valueList in response.Values)
            {
                if (!dictionary.ContainsKey(valueList[9].ToString()))
                {
                    dictionary.Add(valueList[9].ToString(), new ObservableCollection<DiskTimeInfo>());
                }
                dictionary[valueList[9].ToString()].Add(new TelegrafChartTool.DiskTimeInfo()
                {
                    Category = $"{(int)(DateTime.UtcNow - (DateTime)valueList[0]).TotalSeconds}s",
                    Value = 100.0 - double.Parse(valueList[6].ToString())
                });
            }

            var diskTimeInfos = dictionary[response.Values[0][9].ToString()];
            DiskTimeInfos = new ObservableCollection<TelegrafChartTool.DiskTimeInfo>(diskTimeInfos);
            if (diskTimeInfos.Any(i => i.Value > MaxCpuValue))
            {
                ErrorText = $"当前最高{diskTimeInfos.Max(i => i.Value)}超过阈值{MaxCpuValue}";
            }
        }
        private const double MaxCpuValue = 90;

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

        private ObservableCollection<DiskTimeInfo> _DiskTimeInfos;
        public ObservableCollection<DiskTimeInfo> DiskTimeInfos
        {
            get => _DiskTimeInfos;
            set
            {
                _DiskTimeInfos = value;
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

    public class DiskTimeInfo
    {
        public string Category { get; set; }
        public double Value { get; set; }
    }
}
