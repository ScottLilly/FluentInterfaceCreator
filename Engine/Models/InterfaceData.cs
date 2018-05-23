using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Resources;
using Engine.Shared;
using PropertyChanged;

namespace Engine.Models
{
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class InterfaceData
    {
        public string Name { get; set; }
        public string CallableMethodsSignature { get; set; }
        public List<Method> CalledByMethods { get; set; } = new List<Method>();
        public List<Method> CallableMethods { get; set; } = new List<Method>();

        public IEnumerable<string> ValidationErrors()
        {
            if(Name.IsEmpty())
            {
                yield return ErrorMessages.NameIsRequired;
            }
            else
            {
                if(Name.HasAnInternalSpace())
                {
                    yield return ErrorMessages.NameCannotContainAnInternalSpace;
                }

                if(Name.ContainsInvalidCharacter())
                {
                    yield return ErrorMessages.NameCannotContainSpecialCharacters;
                }
            }
        }

        public bool Matches(InterfaceData interfaceData, bool isCaseSensitive = true)
        {
            if(Name.IsEmpty() || interfaceData.Name.IsEmpty())
            {
                return false;
            }

            StringComparison comparisonMethod = isCaseSensitive
                                                    ? StringComparison.CurrentCulture
                                                    : StringComparison.CurrentCultureIgnoreCase;

            return Name.Equals(interfaceData.Name.Trim(), comparisonMethod);
        }

        internal List<string> NamespacesNeeded()
        {
            return CallableMethods.SelectMany(x => x.NamespacesNeeded).Distinct().OrderBy(n => n).ToList();
        }
    }
}