using System.Collections.Generic;
using Engine.Models;

namespace Engine.Factories.FIC
{
    public interface IFluentInterfaceCreator
    {
        FluentInterfaceFile CreateInSingleFile();
        IEnumerable<FluentInterfaceFile> CreateInMultipleFiles();
    }
}