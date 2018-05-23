using System;
using System.Collections.Generic;
using Engine.Resources;
using Engine.Shared;

namespace Engine.Models
{
    [Serializable]
    public class Datatype
    {
        public string Name { get; set; }
        public string ContainingNamespace { get; set; }
        public bool IsNative { get; set; }

        #region Constructor(s)

        public static Datatype BuildNativeDatatype(string name, string containingNamespace)
        {
            return new Datatype(name, containingNamespace, true);
        }

        public static Datatype BuildCustomDatatype(string name, string containingNamespace)
        {
            return new Datatype(name, containingNamespace, false);
        }

        private Datatype(string name, string containingNamespace, bool isNative)
        {
            Name = name;
            ContainingNamespace = containingNamespace;
            IsNative = isNative;
        }

        // For deserialization
        internal Datatype()
        {
        }

        #endregion

        public IEnumerable<string> ValidationErrors()
        {
            if(Name.IsEmpty())
            {
                yield return ErrorMessages.DatatypeIsRequired;
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

            if(ContainingNamespace.HasAnInternalSpace())
            {
                yield return ErrorMessages.NamespaceCannotContainAnInternalSpace;
            }

            if (ContainingNamespace.ContainsInvalidCharacter())
            {
                yield return ErrorMessages.NamespaceCannotContainSpecialCharacters;
            }
        }

        public bool Matches(Datatype datatype, bool isCaseSensitive = true)
        {
            StringComparison comparisonMethod = isCaseSensitive
                                                    ? StringComparison.CurrentCulture
                                                    : StringComparison.CurrentCultureIgnoreCase;

            return Name.Equals(datatype.Name.Trim(), comparisonMethod) &&
                   ContainingNamespace.Equals(datatype.ContainingNamespace.Trim(), comparisonMethod);
        }
    }
}