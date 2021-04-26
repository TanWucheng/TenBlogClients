using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TenBlogCoreLib.Extensions
{
    /// <summary>
    /// INotifyPropertyChanged接口扩展
    /// </summary>
    public static class NotifyPropertyChangedExtension
    {
        /// <summary>
        /// MutateVerbose
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="_"></param>
        /// <param name="field"></param>
        /// <param name="newValue"></param>
        /// <param name="raise"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool MutateVerbose<TField>(this INotifyPropertyChanged _, ref TField field, TField newValue, Action<PropertyChangedEventArgs> raise, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TField>.Default.Equals(field, newValue)) return false;
            field = newValue;
            raise?.Invoke(new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
