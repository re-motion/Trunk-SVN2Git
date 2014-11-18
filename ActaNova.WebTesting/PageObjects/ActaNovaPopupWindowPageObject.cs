using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.PageObjects
{
  /// <summary>
  /// Represents an ActaNova popup window.
  /// </summary>
  // ReSharper disable once ClassNeverInstantiated.Global
  public class ActaNovaPopupWindowPageObject : ActaNovaPageObject
  {
    public ActaNovaPopupWindowPageObject ([NotNull] PageObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public override string GetTitle ()
    {
      return Scope.FindId ("TitleLabel").Text;
    }

    /// <summary>
    /// Closes the popup window.
    /// </summary>
    public void Close ()
    {
      Context.CloseWindow();
    }
  }
}