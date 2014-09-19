using System;

namespace Remotion.Web.Development.WebTesting.WaitingStrategies
{
  /// <summary>
  /// <see cref="IWaitingStrategy"/> implementation which does nothing.
  /// </summary>
  public class NullWaitingStrategy : IWaitingStrategy
  {
    public object OnBeforeActionPerformed (TestObjectContext context)
    {
      return null;
    }

    public void PerformWaitAfterActionPerformed (TestObjectContext context, object state)
    {
    }
  }
}