using System;

namespace Remotion.Web.Development.WebTesting.CompletionDetectionImplementation
{
  /// <summary>
  /// Null implementation of <see cref="ICompletionDetectionStrategy"/>, which does not block.
  /// </summary>
  public class NullCompletionDetectionStrategy : ICompletionDetectionStrategy
  {
    public object PrepareWaitForCompletion (PageObjectContext context)
    {
      return null;
    }

    public void WaitForCompletion (PageObjectContext context, object state)
    {
    }
  }
}