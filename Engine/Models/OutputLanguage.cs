using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Engine.Models
{
    public class OutputLanguage
    {
        public string Name { get; set; }
        public string FileExtension { get; set; }
        public bool IsCaseSensitive { get; set; }

        public ObservableCollection<Datatype> NativeDatatypes { get; } =
            new ObservableCollection<Datatype>();

        internal OutputLanguage(string name, string fileExtension, bool isCaseSensitive,
                                IEnumerable<Datatype> nativeDatatypes)
        {
            Name = name;
            FileExtension = fileExtension;
            IsCaseSensitive = isCaseSensitive;

            foreach(Datatype datatype in nativeDatatypes)
            {
                NativeDatatypes.Add(datatype);
            }
        }

        // For deserialization
        internal OutputLanguage()
        {
        }
    }
}