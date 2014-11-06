using System;
using ActaNova.WebTesting.PageObjects;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.IntegrationTests
{
  /// <summary>
  /// Fluent selection extension methods.
  /// </summary>
  public static class FluentExpectExtensionsForActaNovaWebTests
  {
    public static ActaNovaMainPageObject ExpectMainPage ([NotNull] this UnspecifiedPageObject unspecifiedPageObject)
    {
      ArgumentUtility.CheckNotNull ("unspecifiedPageObject", unspecifiedPageObject);

      return unspecifiedPageObject.Expect<ActaNovaMainPageObject>();
    }
  }
}