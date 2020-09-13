using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMFramework.Model
{
    class Model_Data
    {
        string _data = "这只是普通的数据绑定。";
        string _dataTwoWay = "";//双向绑定
        string _valueScrollBar = "1";//滚动条值

        /// <summary>
        /// 单项绑定的数据测试
        /// </summary>
        public string Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        /// <summary>
        /// 双向绑定的数据测试
        /// </summary>
        public string DataTwoWay
        {
            get
            {
                return _dataTwoWay;
            }
            set
            {
                _dataTwoWay = value;
            }
        }

        /// <summary>
        /// 滚动条的绑定
        /// </summary>
        public string ScrollBar
        {
            get
            {
                return _valueScrollBar;
            }
            set
            {
                _valueScrollBar = value;
            }
        }
    }
}
