using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestIntendedOutput
{
    [TestClass]
    public class TestFluentCalculator
    {
        [TestMethod]
        public void Test_FluentInterface()
        {
            FluentCalculator
                .CreateCalculator()
                .AddNumber(123)
                .AddNumber(234)
                .AddNumber(345)
                .GetAverage();
        }
    }
}