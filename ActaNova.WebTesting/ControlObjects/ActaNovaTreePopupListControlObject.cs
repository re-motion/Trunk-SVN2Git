using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.WebTestActions;

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

    /// <summary>
    /// Clicks an item in the popup list given by its display text <paramref name="item"/>.
    /// </summary>
    public UnspecifiedPageObject ClickItem ([NotNull] string item, [CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("item", item);

      var itemScope = Scope.FindXPath (string.Format (".//a[normalize-space(.)='{0}']", item));

      var actualActionOptions = MergeWithDefaultActionOptions (itemScope, actionOptions);
      new ClickAction (this, itemScope).Execute (actualActionOptions);

      Scope.Unhover();
      EnsurePopupIsClosed();

      return UnspecifiedPage();
    }

    /// <summary>
    /// Checks an item in the popup list given by its display text <paramref name="item"/>.
    /// </summary>
    public UnspecifiedPageObject CheckItem ([NotNull] string item, [CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("item", item);

      var itemScope = Scope.FindXPath (string.Format (".//a[normalize-space(../../td/span)='{0}']", item));

      // Note: without hover before the click, the check item action does not work, we don't know why yet.
      itemScope.Hover();

      var actualActionOptions = MergeWithDefaultActionOptions (itemScope, actionOptions);
      new ClickAction (this, itemScope).Execute (actualActionOptions);

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