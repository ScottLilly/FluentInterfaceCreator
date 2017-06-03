namespace Engine.Models
{
    public class Parameter : NotificationClassBase
    {
        private string _dataType;
        private string _name;

        public string DataType
        {
            get { return _dataType; }
            set
            {
                _dataType = value;

                NotifyPropertyChanged(nameof(DataType));
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

                NotifyPropertyChanged(nameof(Name));
            }
        }
    }
}