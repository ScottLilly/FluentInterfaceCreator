using System;
using Engine.Models;
using Engine.Resources;

namespace Engine.FluentInterfaceCreators
{
    internal static class FluentInterfaceCreatorFactory
    {
        internal static FluentInterfaceFileCreatorBase GetFluentInterfaceFileCreator(
            string language, Project project)
        {
            switch(language)
            {
                case "C#":
                    return new CSharpFluentInterfaceFileCreator(project);
                default:
                    throw new ArgumentException(ErrorMessages.InvalidLanguage, language);
            }
        }
    }
}