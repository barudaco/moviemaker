using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 바루다_무비메이커.Models;

namespace 바루다_무비메이커.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region [영역] 프로퍼티 체인지 구현
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion


        #region DEBUG_MESSAGE
        // 디버그 메시지
        private DebugMessageList _DebugMessage = new DebugMessageList();
        public DebugMessageList DebugMessage
        {
            get { return _DebugMessage; }
            set
            {
                _DebugMessage = value;
                OnPropertyChanged("DebugMessage");
            }
        }

        // 로그창 추가 (일반)
        public void Append_Log(string message)
        {
            var msg = DateTime.UtcNow.ToString("[MM/dd HH:mm:ss] ") + message;
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                DebugMessage.Add(new Models.DebugMessage() { Message = msg, MessageColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black) });
            }));
        }

        // 로그창 추가 (에러)
        public void Append_Error(string message)
        {
            var msg = DateTime.UtcNow.ToString("[MM/dd HH:mm:ss] ") + message;
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                DebugMessage.Add(new Models.DebugMessage() { Message = msg, MessageColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red) });
            }));
        }

        // 로그창 추가 (파랑)
        public void Append_Blue(string message)
        {
            var msg = DateTime.UtcNow.ToString("[MM/dd HH:mm:ss] ") + message;
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                DebugMessage.Add(new Models.DebugMessage() { Message = msg, MessageColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue) });
            }));
        }

        public void RemoveLast_Log()
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                if (0 < DebugMessage.Count)
                {
                    DebugMessage.RemoveAt(DebugMessage.Count-1);
                }
            }));
        }

        public void AppendLast_Log_String(string message)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                if (0 < DebugMessage.Count)
                {
                    var last = DebugMessage.Last();
                    last.Message += message;
                    DebugMessage.RemoveAt(DebugMessage.Count - 1);
                    DebugMessage.Add(last);
                }
            }));
        }


        #endregion
    }
}
