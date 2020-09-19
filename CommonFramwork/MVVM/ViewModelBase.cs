using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonFramework.MVVM
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region UI更新接口
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            this.VerifyPropertyName(propertyName);
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 无须传递名字.
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChangedNoName([CallerMemberName] string propertyName = null)
        {
            this.VerifyPropertyName(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>      
        /// 触发属性发生变更事件      
        /// </summary>      
        /// <typeparam name="T">泛型标记，会匹配函数返回类型，不必手动填写</typeparam>      
        /// <param name="action">以函数表达式方式传入属性名称，表达式如下即可：()=>YourViewModelProperty</param>
        /// OnPropertyChanged<int>(()=>this.NB);      
        protected void OnPropertyChanged<T>(Expression<Func<T>> action)
        {
            var propertyName = GetPropertyName(action);
            this.VerifyPropertyName(propertyName);
            OnPropertyChanged(propertyName);
        }

        private static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }

        /// <summary>
        /// 验证属性是否存在
        /// </summary>
        /// <param name="propertyName"></param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;
                throw new Exception(msg);
            }
        }

        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] String propName=null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propName);
            return true;
        }

        /// <summary>
        /// Tell bound controls (via WPF binding) to refresh their display.
        /// 
        /// Sample call: this.NotifyPropertyChanged(() => this.IsSelected);
        /// where 'this' is derived from <seealso cref="ViewModelBase"/>
        /// and IsSelected is a property.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        public void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            var lambda = (LambdaExpression)property;
            MemberExpression memberExpression;

            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
                memberExpression = (MemberExpression)lambda.Body;

            this.OnPropertyChanged(memberExpression.Member.Name);
        }
        #endregion

        #region 是否正在加载
        private bool isLoad;

        /// <summary>
        /// 是否加载
        /// </summary>
        public bool IsLoad
        {
            get { return isLoad; }
            set
            {
                isLoad = value;
                OnPropertyChanged(nameof(IsLoad));
            }
        }
        #endregion

        #region 是否需要刷新
        private bool update;
        /// <summary>
        /// 刷新
        /// </summary>
        public bool Update
        {
            get { return update; }
            set
            {
                update = value;
                OnPropertyChanged(nameof(Update));
            }
        }
        #endregion
    }

    public static class NotifyPropertyChangedEx
    {
        public static void AddPropertyChanged<TObj, TProp>(
            this TObj _this,
            Expression<Func<TObj, TProp>> propertyName,
            Action handler)
            where TObj : INotifyPropertyChanged
        {
            var name = ((MemberExpression)propertyName.Body).Member.Name;

            _this.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == name)
                {
                    handler();
                }
            };
        }
    }
}
