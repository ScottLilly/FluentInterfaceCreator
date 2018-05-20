using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Engine.Resources;
using Engine.Shared;
using PropertyChanged;

namespace Engine.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Method
    {
        #region Enums

        // TODO: See if we can add descriptions, and get values from resource file
        public enum MethodGroup
        {
            Instantiating,
            Chaining,
            Executing
        }

        // Needed, to bind enum to combobox in WPF UI
        public IEnumerable<MethodGroup> MethodGroups =>
            Enum.GetValues(typeof(MethodGroup)).Cast<MethodGroup>();

        #endregion

        public MethodGroup Group { get; set; }

        public string Name { get; set; }

        // Only used for Executing methods
        public bool RequiresReturnDatatype => Group == MethodGroup.Executing;
        public Datatype ReturnDatatype { get; set; }

        public ObservableCollection<Parameter> Parameters { get; } =
            new ObservableCollection<Parameter>();

        public ObservableCollection<CallableMethodIndicator> MethodsCallableNext { get; } =
            new ObservableCollection<CallableMethodIndicator>();

        #region Derived properties

        public bool IsChainStarting => Group == MethodGroup.Instantiating || Group == MethodGroup.Chaining;
        public bool IsChainEnding => Group == MethodGroup.Chaining || Group == MethodGroup.Executing;

        public string Signature => $"{Name}({ParameterList})";

        public string DatatypeSignature => $"{Name}({ParameterDatatypeList})";

        public string ParameterList =>
            string.Join(", ", Parameters.Select(p => $"{p.Datatype.Name} {p.Name}"));

        public string ParameterDatatypeList =>
            string.Join(", ", Parameters.Select(p => $"{p.Datatype.Name}"));

        // Used to determine methods that can call the same 'next' methods,
        // to eliminate duplicate InterfaceData objects that identify the same interface.
        public string CallableMethodsSignature =>
            string.Join("|", MethodsCallableNext
                .Where(x => x.CanCall)
                .OrderBy(x => x.Group)
                .ThenBy(x => x.Name)
                .Select(x => $"{x.Group}:{x.Name}"));

        public List<string> NamespacesNeeded
        {
            get
            {
                List<string> namespacesNeeded = new List<string>();

                namespacesNeeded.AddRange(Parameters.Select(parameter => parameter.Datatype.ContainingNamespace));
                namespacesNeeded.Add(ReturnDatatype?.ContainingNamespace);

                return namespacesNeeded.Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().ToList();
            }
        }

        #endregion

        public Method(MethodGroup group, string name, Datatype returnDatatype = null)
        {
            Group = group;
            Name = name;
            ReturnDatatype = returnDatatype;
        }

        // For serialization only
        internal Method()
        {
        }

        public IEnumerable<string> ValidationErrors()
        {
            if(Group != MethodGroup.Instantiating &&
               Group != MethodGroup.Chaining &&
               Group != MethodGroup.Executing)
            {
                yield return ErrorMessages.GroupIsNotValid;
            }

            if(Group == MethodGroup.Executing && ReturnDatatype == null)
            {
                yield return ErrorMessages.ReturnTypeIsRequired;
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

        public bool Matches(Method method, bool isCaseSensitive = true)
        {
            StringComparison comparisonMethod = isCaseSensitive
                                                    ? StringComparison.CurrentCulture
                                                    : StringComparison.CurrentCultureIgnoreCase;

            return Name.Equals(method.Name.Trim(), comparisonMethod) &&
                   ParameterDatatypeList == method.ParameterDatatypeList;
        }
    }
}