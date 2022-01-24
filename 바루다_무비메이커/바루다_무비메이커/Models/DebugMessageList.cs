using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 바루다_무비메이커.Models
{
    public class DebugMessageList : ObservableCollection<DebugMessage>
    {
        public void append(string _Message, System.Windows.Media.SolidColorBrush _MessageColor)
        {
            Add(new DebugMessage() { Message = _Message, MessageColor = _MessageColor });
        }
    }

    public class DebugMessage
    {
        public string Message { get; set; }
        public System.Windows.Media.SolidColorBrush MessageColor { get; set; }
    }
}
