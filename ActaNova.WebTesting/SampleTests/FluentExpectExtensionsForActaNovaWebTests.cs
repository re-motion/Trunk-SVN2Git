using System;
using ActaNova.WebTesting.PageObjects;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.SampleTests
{
  /// <summary>
  /// Fluent selection extension methods.
  /// </summary>
  public static class FluentExpectExtensionsForActaNovaWebTests
  {
    public static ActaNovaMainPageObject ExpectActaNova ([NotNull] this UnspecifiedPageObject unspecifiedPageObject)
    {
      ArgumentUtility.CheckNotNull ("unspecifiedPageObject", unspecifiedPageObject);
      return unspecifiedPageObject.Expect<ActaNovaMainPageObject>();
    }
  }
}