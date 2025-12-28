// Restoran.Tests/UnitTests/SimpleCalculatorTests.cs
using Restoran.Core.Helpers;
using Xunit;

namespace Restoran.Tests.UnitTests
{
    public class SimpleCalculatorTests
    {
        [Fact]
        public void Add_TwoNumbers_ReturnsSum()
        {
            // Arrange (Hazırlık)
            var calculator = new SimpleCalculator();
            int a = 5;
            int b = 3;
            int expected = 8;

            // Act (Eylem)
            int result = calculator.Add(a, b);

            // Assert (Doğrulama)
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Divide_ByZero_ThrowsDivideByZeroException()
        {
            // Arrange
            var calculator = new SimpleCalculator();
            int a = 10;
            int b = 0;

            // Act & Assert
            Assert.Throws<DivideByZeroException>(() => calculator.Divide(a, b));
        }
    }
}