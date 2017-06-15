using System;
using Engine.Resources;

namespace Engine.FluentInterfaceCreators
{
    internal static class FluentInterfaceCreatorFactory
    {
        internal static IFluentInterfaceCreator GetCreatorForLanguage(string language)
        {
            switch(language)
            {
                case "C#":
                    return new CSharpSingleFileCreator();
                default:
                    throw new ArgumentException(ErrorMessages.InvalidLanguage, language);
            }
        }
    }
}