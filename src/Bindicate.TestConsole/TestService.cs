namespace Bindicate.TestConsole
{
    internal class TestService : ITestService
    {
        public int add(int x, int y) => x + y;
    }

    internal interface ITestService
    {
        int add(int x, int y);
    }
}
