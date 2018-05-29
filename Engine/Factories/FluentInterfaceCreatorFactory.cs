using System;
using Engine.Factories.FIC;
using Engine.Models;
using Engine.Resources;

namespace Engine.Factories
{
    public static class FluentInterfaceCreatorFactory
    {
        public static IFluentInterfaceCreator GetFluentInterfaceFileCreator(Project project)
        {
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