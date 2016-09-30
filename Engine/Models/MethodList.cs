using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Engine.Annotations;
using Engine.Resources;

namespace Engine.Models
{
    public class MethodList : INotifyPropertyChanged
    {
        private readonly Dictionary<ulong, Method> _methods = new Dictionary<ulong, Method>();

        public ObservableCollection<Method> Methods => new ObservableCollection<Method>(_methods.Values);

        public event PropertyChangedEventHandler PropertyChanged;

        internal void AddMethod(Method method)
        {
            // TODO: Throw exception if duplicate (by name, or by name and parameters?)

            int numberOfMethods = _methods.Count;

            if(numberOfMethods >= 64)
            {
                throw new IndexOutOfRangeException(ErrorMessages.CannotAddMoreThan64MethodsToAProject);
            }

            _methods.Add(Convert.ToUInt64(2 ^ numberOfMethods), method);

            OnPropertyChanged(nameof(Methods));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}