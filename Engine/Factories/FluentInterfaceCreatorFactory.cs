using System;
using Engine.Factories.FIC;
using Engine.Models;
using Engine.Resources;

namespace Engine.Factories
{
    internal static class FluentInterfaceCreatorFactory
    {
        internal static IFluentInterfaceCreator GetFluentInterfaceFileCreator(Project project)
        {
            FluentInterfaceCreator creator = new FluentInterfaceCreator(project);

            switch(project.OutputLanguage.Name)
            {
                case "C#":
                    return new CSharpFluentInterfaceFileCreator(project);
                default:
                    throw new ArgumentException(ErrorMessages.InvalidLanguage, project.OutputLanguage.Name);
            }
        }
    }
}