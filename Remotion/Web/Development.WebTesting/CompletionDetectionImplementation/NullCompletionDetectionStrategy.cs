using System;

namespace Remotion.Web.Development.WebTesting.CompletionDetectionImplementation
{
  /// <summary>
  /// Null implementation of <see cref="ICompletionDetectionStrategy"/>, which does not block.
  /// </summary>
  public class NullCompletionDetectionStrategy : ICompletionDetectionStrategy
  {
    public object PrepareWaitForCompletion (TestObjectContext context)
    {
      return null;
    }

    public void WaitForCompletion (TestObjectContext context, object state)
    {
    }
  }
}