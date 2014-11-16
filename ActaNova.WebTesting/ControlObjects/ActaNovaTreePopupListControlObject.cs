using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing an ActaNova popup table control, filled with a list.
  /// </summary>
  public class ActaNovaTreePopupListControlObject : WebFormsControlObject
  {
    public ActaNovaTreePopupListControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public UnspecifiedPageObject ClickItem (
        [NotNull] string item,
        [CanBeNull] ICompletionDetection completionDetection = null,
        [CanBeNull] IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("item", item);

      var itemScope = Scope.FindXPath (string.Format (".//a[contains(., '{0}')]", item));

      var actualCompletionDetector = GetActualCompletionDetector (itemScope, completionDetection);
      itemScope.ClickAndWait (Context, actualCompletionDetector, modalDialogHandler);

      Scope.Unhover();
      EnsurePopupIsClosed();

      return UnspecifiedPage();
    }

    public UnspecifiedPageObject CheckItem (
        [NotNull] string item,
        [CanBeNull] ICompletionDetection completionDetection = null,
        [CanBeNull] IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("item", item);

      var itemScope = Scope.FindXPath (string.Format (".//a[contains(../../td/span, '{0}')]", item));

      // Note: without hover before the click, the check item action does not work, we don't know why yet.
      itemScope.Hover();

      var actualCompletionDetector = GetActualCompletionDetector (itemScope, completionDetection);
      itemScope.ClickAndWait (Context, actualCompletionDetector, modalDialogHandler);

      Scope.Unhover();
      EnsurePopupIsClosed();

      return UnspecifiedPage();
    }

    private void EnsurePopupIsClosed ()
    {
      const string closeAllPopupsScript = @"
        CloseAllPopups = function() {
          for (var key in window.DynamicContentPopupContainer.Instances) {
            if (window.DynamicContentPopupContainer.Instances.hasOwnProperty(key)) {
              var instance = window.DynamicContentPopupContainer.Instances[key];
              if (instance != this) {
                instance.closeImmediately();
              }
            }
          }
        };
        CloseAllPopups();";
      Context.Browser.Driver.ExecuteScript (closeAllPopupsScript, Context.Scope);
    }
  }
}