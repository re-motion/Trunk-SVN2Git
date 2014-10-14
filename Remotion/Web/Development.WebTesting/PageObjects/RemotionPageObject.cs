using System;

namespace Remotion.Web.Development.WebTesting.PageObjects
{
  /// <summary>
  /// Page object representing an arbitrary re-motion-based page.
  /// </summary>
  public class RemotionPageObject : PageObject
  {
    // ReSharper disable once MemberCanBeProtected.Global
    public RemotionPageObject (TestObjectContext context)
        : base (context)
    {
    }
  }
}