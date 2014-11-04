using System;
using ActaNova.WebTesting.PageObjects;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.IntegrationTests
{
  /// <summary>
  /// Parameter values for <see cref="FluentExpectExtensionsForActaNovaWebTests.ExpectActaNova"/>'s <c>messageBoxHandling</c> parameter.
  /// </summary>
  public class ActaNovaMessageBox
  {
    public static readonly ActaNovaMessageBox Okay = new ActaNovaMessageBox (mb => mb.Confirm());
    public static readonly ActaNovaMessageBox Cancel = new ActaNovaMessageBox (mb => mb.Cancel());

    private readonly Func<MessageBoxPageObject, UnspecifiedPageObject> _func;

    private ActaNovaMessageBox (Func<MessageBoxPageObject, UnspecifiedPageObject> func)
    {
      _func = func;
    }

    public UnspecifiedPageObject Apply ([NotNull] UnspecifiedPageObject unspecifiedPageObject)
    {
      var messageBox = unspecifiedPageObject.Expect<MessageBoxPageObject>();
      return _func (messageBox);
    }
  }

  /// <summary>
  /// Fluent selection extension methods.
  /// </summary>
  public static class FluentExpectExtensionsForActaNovaWebTests
  {
    public static ActaNovaMainPageObject ExpectActaNova (
        [NotNull] this UnspecifiedPageObject unspecifiedPageObject,
        [CanBeNull] ActaNovaMessageBox messageBoxHandling = null)
    {
      ArgumentUtility.CheckNotNull ("unspecifiedPageObject", unspecifiedPageObject);

      if (messageBoxHandling != null)
        unspecifiedPageObject = messageBoxHandling.Apply (unspecifiedPageObject);

      return unspecifiedPageObject.Expect<ActaNovaMainPageObject>();
    }
  }
}