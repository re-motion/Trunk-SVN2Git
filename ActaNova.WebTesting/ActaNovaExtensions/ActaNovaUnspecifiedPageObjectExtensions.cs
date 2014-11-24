using System;
using ActaNova.WebTesting.PageObjects;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.ActaNovaExtensions
{
  /// <summary>
  /// ActaNova-specific Expect*() extension methods.
  /// </summary>
  public static class ActaNovaUnspecifiedPageObjectExtensions
  {
    public static ActaNovaMainPageObject ExpectMainPage ([NotNull] this UnspecifiedPageObject unspecifiedPageObject)
    {
      ArgumentUtility.CheckNotNull ("unspecifiedPageObject", unspecifiedPageObject);

      return unspecifiedPageObject.Expect<ActaNovaMainPageObject>();
    }

    public static ActaNovaWindowPageObject ExpectNewActaNovaWindow (
        [NotNull] this UnspecifiedPageObject unspecifiedPageObject,
        [NotNull] string windowLocator)
    {
      ArgumentUtility.CheckNotNull ("unspecifiedPageObject", unspecifiedPageObject);
      ArgumentUtility.CheckNotNullOrEmpty ("windowLocator", windowLocator);

      return unspecifiedPageObject.ExpectNewWindow<ActaNovaWindowPageObject> (windowLocator);
    }

    public static ActaNovaPopupWindowPageObject ExpectNewActaNovaPopupWindow (
        [NotNull] this UnspecifiedPageObject unspecifiedPageObject,
        [NotNull] string windowLocator)
    {
      ArgumentUtility.CheckNotNull ("unspecifiedPageObject", unspecifiedPageObject);
      ArgumentUtility.CheckNotNullOrEmpty ("windowLocator", windowLocator);

      return unspecifiedPageObject.ExpectNewPopupWindow<ActaNovaPopupWindowPageObject> (windowLocator);
    }
  }
}