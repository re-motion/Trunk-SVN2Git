using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.PageObjects
{
  /// <summary>
  /// Represents an ActaNova window.
  /// </summary>
  // ReSharper disable once ClassNeverInstantiated.Global
  public class ActaNovaWindowPageObject : ActaNovaPageObject
  {
    public ActaNovaWindowPageObject ([NotNull] PageObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Closes the window.
    /// </summary>
    public void Close ()
    {
      Context.CloseWindow();
    }
  }
}