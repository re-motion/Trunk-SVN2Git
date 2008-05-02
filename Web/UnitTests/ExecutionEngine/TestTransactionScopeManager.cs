using Remotion.Data;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  public class TestTransactionScopeManager : ITransactionScopeManager<TestTransaction, TestTransactionScope>
  {
    public TestTransactionScope ActiveScope
    {
      get { return TestTransactionScope.CurrentScope; }
    }

    public TestTransactionScope EnterScope (TestTransaction transaction)
    {
      return new TestTransactionScope (transaction);
    }
  }
}