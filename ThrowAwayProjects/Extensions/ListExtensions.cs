using System;
using System.Collections.Generic;
namespace CustomExtensions
{
    public static class ListExtensions
    {
        public static Type GetListType<T>(this List<T> _)
        {
            return typeof(T);
        }
    }
}