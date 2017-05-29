using System.Collections.Generic;
using System.Linq;

namespace TestIntendedOutput
{
    public class FluentCalculator : ICanAddNumber, ICanAddNumberOrComputeResults
    {
        private readonly List<decimal> _numbers = new List<decimal>();

        public static ICanAddNumber CreateCalculator()
        {
            return new FluentCalculator();
        }

        public ICanAddNumberOrComputeResults AddNumber(decimal number)
        {
            _numbers.Add(number);

            return this;
        }

        public decimal GetLowestNumber()
        {
            return _numbers.Min(x => x);
        }

        public decimal GetHighestNumber()
        {
            return _numbers.Max(x => x);
        }

        public decimal GetAverage()
        {
            return _numbers.Average(x => x);
        }
    }

    public interface ICanAddNumber
    {
        ICanAddNumberOrComputeResults AddNumber(decimal number);
    }

    public interface ICanAddNumberOrComputeResults
    {
        ICanAddNumberOrComputeResults AddNumber(decimal number);
        decimal GetLowestNumber();
        decimal GetHighestNumber();
        decimal GetAverage();
    }
}
