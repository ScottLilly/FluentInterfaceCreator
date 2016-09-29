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

            Method constructor = new Method("InsertIntoTable", Method.Action.Instantiate);
            constructor.AddParameter("tableName", "string");
            project.AddMethod(constructor);

            Method setColumn = new Method("SetColumn", Method.Action.Continue);
            setColumn.AddParameter("columnName", "string");
            project.AddMethod(setColumn);

            Method toValue = new Method("ToValue", Method.Action.Continue);
            toValue.AddParameter("value", "object");
            project.AddMethod(toValue);

            Method runQuery = new Method("RunQuery", Method.Action.Execute);
            project.AddMethod(runQuery);
        }
    }
}