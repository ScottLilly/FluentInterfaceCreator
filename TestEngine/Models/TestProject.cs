using Microsoft.VisualStudio.TestTools.UnitTesting;
using Engine.Models;

namespace TestEngine.Models
{
    [TestClass]
    public class TestProject
    {
        [TestMethod]
        public void Test_CreateProject()
        {
            Project project = new Project("SQLHydra");
        }
    }
}
