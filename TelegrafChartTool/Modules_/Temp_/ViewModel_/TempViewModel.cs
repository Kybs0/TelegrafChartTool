using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TelegrafChart.Business;
using TelegrafChartTool.Annotations;
using Timer = System.Timers.Timer;

namespace TelegrafChartTool
{
    class TempViewModel : INotifyPropertyChanged
    {
        public TempViewModel()
        {
            var timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += (s, e) => { GetData(); };
            timer.Start();
        }

        /// <summary>
        /// 从InfluxDB中读取数据
        /// </summary>
        public void GetData()
        {
            var cpuTimeInfos = new List<TempTimeInfo>();
            //try
            //{
            //    //从指定库中查询数据
            //    var response = await InfluxDbClientHelper.QueryAsync(" SELECT * FROM win_temp WHERE time> now() -  120s");
            //    //从集合中取出第一条数据
            //    Dictionary<string, ObservableCollection<CpuTimeInfo>> dictionary = new Dictionary<string, ObservableCollection<CpuTimeInfo>>();
            //    foreach (var valueList in response.Values)
            //    {
            //        if (!dictionary.ContainsKey(valueList[9].ToString()))
            //        {
            //            dictionary.Add(valueList[9].ToString(), cpuTimeInfos);
            //        }
            //        dictionary[valueList[9].ToString()].Add(new CpuTimeInfo()
            //        {
            //            Category = $"{(int)(DateTime.UtcNow - (DateTime)valueList[0]).TotalSeconds}s",
            //            Value = 100.0 - double.Parse(valueList[6].ToString())
            //        });
            //    }
            //    CpuTimeInfos = new ObservableCollection<CpuTimeInfo>(dictionary[response.Values[0][9].ToString()]);
            //}
            //catch (Exception e)
            //{
            //}
            var random = new Random(28);
            for (int i = 20; i > 0; i--)
            {
                cpuTimeInfos.Add(new TempTimeInfo()
                {
                    Category = (i * 3).ToString(),
                    Value = random.Next(25, 32)
                });
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                CpuTimeInfos = new ObservableCollection<TempTimeInfo>(); ;
                CpuTimeInfos = new ObservableCollection<TempTimeInfo>(cpuTimeInfos);
            });
        }

        private ObservableCollection<TempTimeInfo> _cpuTimeInfos = new ObservableCollection<TempTimeInfo>();
        public ObservableCollection<TempTimeInfo> CpuTimeInfos
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

    public class TempTimeInfo : INotifyPropertyChanged
    {
        private string _category;
        public string Category
        {
            get => _category;
            set
            {
                _category = value;
                OnPropertyChanged();
            }
        }

        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
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
}
