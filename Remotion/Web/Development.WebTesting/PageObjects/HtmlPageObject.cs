using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.PageObjects
{
  /// <summary>
  /// Represents a simple HTML page.
  /// </summary>
  public class HtmlPageObject : PageObject
  {
    public HtmlPageObject ([NotNull] PageObjectContext context)
        : base(context)
    {
    }

    public void Close ()
    {
      Context.CloseWindow();
    }
  }
}