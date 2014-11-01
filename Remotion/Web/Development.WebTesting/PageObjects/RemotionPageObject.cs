using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.PageObjects
{
  /// <summary>
  /// Page object representing an arbitrary re-motion-based page.
  /// </summary>
  public class RemotionPageObject : PageObject
  {
    public RemotionPageObject ([NotNull] PageObjectContext context)
        : base (context)
    {
    }
  }
}