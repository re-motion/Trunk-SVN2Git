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
    public static readonly ActaNovaMessageBox OkayWithoutPostback = new ActaNovaMessageBox (mb => mb.Confirm (Continue.Immediately()));
    public static readonly ActaNovaMessageBox Cancel = new ActaNovaMessageBox (mb => mb.Cancel());
    public static readonly ActaNovaMessageBox Yes = new ActaNovaMessageBox (mb => mb.Yes());
    public static readonly ActaNovaMessageBox No = new ActaNovaMessageBox (mb => mb.No());

    private readonly Func<ActaNovaMessageBoxPageObject, UnspecifiedPageObject> _func;

    private ActaNovaMessageBox (Func<ActaNovaMessageBoxPageObject, UnspecifiedPageObject> func)
    {
      _func = func;
    }

    public UnspecifiedPageObject Apply ([NotNull] UnspecifiedPageObject unspecifiedPageObject)
    {
      var messageBox = unspecifiedPageObject.Expect<ActaNovaMessageBoxPageObject>();
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