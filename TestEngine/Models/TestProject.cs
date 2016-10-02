using Engine;
using Engine.Models;
using Xunit;

namespace TestEngine.Models
{
    public class TestProject
    {
        [Fact]
        public void Test_CreateProject()
        {
            Project project = new Project("SQLHydra", Language.CSharp);

            Method constructor = new Method("InsertIntoTable", "Instantiate");
            constructor.AddParameter("tableName", "string");
            project.AddMethod(constructor);

            Method setColumn = new Method("SetColumn", "Continue");
            setColumn.AddParameter("columnName", "string");
            project.AddMethod(setColumn);

            Method toValue = new Method("ToValue", "Continue");
            toValue.AddParameter("value", "object");
            project.AddMethod(toValue);

            Method runQuery = new Method("RunQuery", "Execute");
            project.AddMethod(runQuery);
        }
    }
}