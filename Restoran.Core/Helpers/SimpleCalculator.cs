// Restoran.Core/Helpers/SimpleCalculator.cs
namespace Restoran.Core.Helpers
{
    public class SimpleCalculator
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Subtract(int a, int b)
        {
            return a - b;
        }

        public int Multiply(int a, int b)
        {
            return a * b;
        }

        public int Divide(int a, int b)
        {
            if (b == 0)
                throw new DivideByZeroException("Sıfıra bölme hatası!");
            return a / b;
        }
    }
}