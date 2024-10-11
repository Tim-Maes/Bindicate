using Bindicate.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Bindicate.Tests.Decorator;

public class RegisterDecoratorAttributeTests
{
    public interface IOperation
    {
        int Perform(int a, int b);
    }

    [AddSingleton(typeof(IOperation))]
    public class Operation : IOperation
    {
        public int Perform(int a, int b)
        {
            return a + b;
        }
    }

    [RegisterDecorator(typeof(IOperation))]
    public class LoggingOperationDecorator : IOperation
    {
        private readonly IOperation _innerOperation;

        public LoggingOperationDecorator(IOperation innerOperation)
        {
            _innerOperation = innerOperation;
        }

        public int Perform(int a, int b)
        {
            Console.WriteLine($"Logging before operation: {a}, {b}");
            var result = _innerOperation.Perform(a, b);
            Console.WriteLine($"Logging after operation: result {result}");
            return result;
        }
    }

    // Test class to verify the decorator implementation using xUnit and FluentAssertions
    public class DecoratorTests
    {
        private readonly IServiceProvider _serviceProvider;

        public DecoratorTests()
        {
            var services = new ServiceCollection();

            // Use the AutowiringBuilder to register services and decorators
            services.AddAutowiringForAssembly(typeof(IOperation).Assembly)
                    .Register();

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void Operation_ShouldBeDecoratedWithLogging()
        {
            var operation = _serviceProvider.GetRequiredService<IOperation>();

            using var consoleOutput = new ConsoleOutput();

            var result = operation.Perform(5, 7);

            result.Should().Be(12);

            var expectedOutput = $"Logging before operation: 5, 7{Environment.NewLine}Logging after operation: result 12{Environment.NewLine}";
            consoleOutput.GetOutput().Should().Be(expectedOutput);

            operation.Should().BeOfType<LoggingOperationDecorator>();
        }
    }

    public class ConsoleOutput : IDisposable
    {
        private readonly StringWriter _stringWriter;
        private readonly TextWriter _originalOutput;

        public ConsoleOutput()
        {
            _stringWriter = new StringWriter();
            _originalOutput = Console.Out;
            Console.SetOut(_stringWriter);
        }

        public string GetOutput()
        {
            return _stringWriter.ToString();
        }

        public void Dispose()
        {
            Console.SetOut(_originalOutput);
            _stringWriter.Dispose();
        }
    }
}