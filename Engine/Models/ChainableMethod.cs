using System;

namespace Engine.Models
{
    public class ChainableMethod
    {
        public Method Method { get; }
        public bool IsSelected { get; set; }

        public ulong MaskValue => Convert.ToUInt64(Math.Pow(2, Method.ChainIndex));

        public ChainableMethod(Method method, bool isSelected = false)
        {
            Method = method;
            IsSelected = isSelected;
        }
    }
}
