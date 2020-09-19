using CommonFramework;
using CommonFramework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMFramework.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        #region 初始化
        public LoginViewModel()
        {
            //初始化数据 - 请不要这里做太多任务，如果有需要可以使用Task
            User.UserName = "admin";
            User.UserPwd = "admin";
        }
        #endregion

        #region 属性
        private User user;
        /// <summary>
        /// 当前用户
        /// </summary>
        public User User
        {
            get
            {
                if (user == null)
                {
                    user = new User();
                }
                return user;
            }
            set
            {
                user = value;
                OnPropertyChanged(nameof(User));
            }
        }
        #endregion

        #region 命令
        /// <summary>
        /// 登录操作
        /// </summary>    
        public ICommand LoginCommand => new RelayCommand(obj =>
        {
            if (User.UserName == "admin" && User.UserPwd == "admin")
            {
                //Auth.Login = true;
                MVVMFramework.Common.WindowPageHelper.ShowPageHome();
            }
        });
        #endregion
    }
}
