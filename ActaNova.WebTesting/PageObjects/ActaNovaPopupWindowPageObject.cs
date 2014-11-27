using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.PageObjects
{
  /// <summary>
  /// Represents an ActaNova popup window.
  /// </summary>
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

    /// <inheritdoc/>
    protected override ICompletionDetectionStrategy GetDefaultCompletionDetectionForPerform ()
    {
      return Wxe.PostBackCompletedInParent (this);
    }
  }
}