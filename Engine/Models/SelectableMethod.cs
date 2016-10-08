namespace Engine.Models
{
    public class SelectableMethod
    {
        public Method Method { get; set; }
        public bool IsSelected { get; set; }

        public SelectableMethod(Method method, bool isSelected = false)
        {
            Method = method;
            IsSelected = isSelected;
        }
    }
}
