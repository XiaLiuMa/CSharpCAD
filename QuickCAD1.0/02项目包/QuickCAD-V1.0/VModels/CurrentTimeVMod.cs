using System;
using System.ComponentModel;

namespace IsolatorTester.VModels
{
    public class CurrentTimeVMod : INotifyPropertyChanged
    {
        private string currentTime;

        public string CurrentTime
        {
            get { return currentTime; }
            set
            {
                if (currentTime != value)
                {
                    currentTime = value;
                    OnPropertyChanged("CurrentTime");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // 构造函数
        public CurrentTimeVMod()
        {
            UpdateTime();
        }

        // 更新系统时间
        public void UpdateTime()
        {
            CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
