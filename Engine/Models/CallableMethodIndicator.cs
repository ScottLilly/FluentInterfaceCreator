using System;

namespace Engine.Models
{
    [Serializable]
    public class CallableMethodIndicator
    {
        #region Properties

        public string Group { get; set; }
        public string Name { get; set; }
        public bool CanCall { get; set; }

        #endregion

        public CallableMethodIndicator(Method method, bool canCall = false)
        {
            Group = method.Group.ToString();
            Name = method.Name;
            CanCall = canCall;
        }

        // For serialization
        private CallableMethodIndicator()
        {
        }
    }
}