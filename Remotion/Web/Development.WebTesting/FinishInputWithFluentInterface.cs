using System;
using OpenQA.Selenium;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Fluent interface for <see cref="FinishInputWithAction"/>s which are directly supported by the framework.
  /// </summary>
  public static class FinishInput
  {
    public static readonly FinishInputWithAction Promptly = s => { };
    public static readonly FinishInputWithAction WithTab = s => s.SendKeysFixed (Keys.Tab);
    // Todo RM-6297: Why does PressEnter not trigger an auto postback in IE? Is this a bug? See BocListCO.GoToSpecificPage().
    public static readonly FinishInputWithAction WithEnter = s => s.SendKeysFixed (Keys.Enter);
  }
}