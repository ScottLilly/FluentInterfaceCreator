using System;
using System.Collections.Generic;
using Engine.Resources;
using Engine.Shared;
using PropertyChanged;

namespace Engine.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Parameter
    {
        public Datatype Datatype { get; set; }
        public string Name { get; set; }

        public IEnumerable<string> ValidationErrors()
        {
            if(Datatype == null)
            {
                yield return ErrorMessages.DatatypeIsRequired;
            }

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

                if (Name.ContainsInvalidCharacter())
                {
                    yield return ErrorMessages.NameCannotContainSpecialCharacters;
                }
            }
        }

        public bool Matches(Parameter parameter, bool isCaseSensitive = true)
        {
            StringComparison comparisonMethod = isCaseSensitive
                                                    ? StringComparison.CurrentCulture
                                                    : StringComparison.CurrentCultureIgnoreCase;

            return Name.Equals(parameter.Name.Trim(), comparisonMethod);
        }
    }
}