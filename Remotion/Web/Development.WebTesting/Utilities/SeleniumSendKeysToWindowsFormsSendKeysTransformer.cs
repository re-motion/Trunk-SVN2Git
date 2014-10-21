using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Keys = OpenQA.Selenium.Keys;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Utilities to convert a Selenium <see cref="IWebElement.SendKeys"/> call to a <see cref="SendKeys"/> call.
  /// </summary>
  public static class SeleniumSendKeysToWindowsFormsSendKeysTransformer
  {
    /// <summary>
    /// Converts a Selenium <see cref="IWebElement.SendKeys"/> text parameter to a <see cref="SendKeys.SendWait"/> keys parameter.
    /// </summary>
    /// <remarks>
    /// See http://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys.aspx for more information.
    /// </remarks>
    public static string Convert ([NotNull] string value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);

      value = EncloseSpecialCharacters (value);
      value = TransformKeys (value);
      value = TransformModifierKeys (value);
      return value;
    }

    private static string EncloseSpecialCharacters (string value)
    {
      var charactersToEncloseForSendKeys = new[]
                                           {
                                               Regex.Escape ("+"), Regex.Escape ("^"), Regex.Escape ("%"), Regex.Escape ("~"), Regex.Escape ("("),
                                               Regex.Escape (")"), Regex.Escape ("'"), Regex.Escape ("["), Regex.Escape ("]"), Regex.Escape ("{"),
                                               Regex.Escape ("}")
                                           };

      var charactersToEncloseForSendKeysRegex = string.Join ("|", charactersToEncloseForSendKeys);
      return Regex.Replace (value, charactersToEncloseForSendKeysRegex, match => "{" + match.Value + "}");
    }

    private static string TransformKeys (string value)
    {
      var replacementDictionary = new Dictionary<string, string>
                                  {
                                      { Keys.Backspace, "{BS}" },
                                      { Keys.Pause, "{BREAK}" },
                                      { Keys.Delete, "{DEL}" },
                                      { Keys.Down, "{DOWN}" },
                                      { Keys.End, "{END}" },
                                      { Keys.Enter, "{ENTER}" },
                                      { Keys.Escape, "{ESC}" },
                                      { Keys.Help, "{HELP}" },
                                      { Keys.Home, "{HOME}" },
                                      { Keys.Insert, "{INS}" },
                                      { Keys.Left, "{LEFT}" },
                                      { Keys.PageDown, "{PGDN}" },
                                      { Keys.PageUp, "{PGUP}" },
                                      { Keys.Right, "{RIGHT}" },
                                      { Keys.Tab, "{TAB}" },
                                      { Keys.Up, "{UP}" },
                                      { Keys.F1, "{F1}" },
                                      { Keys.F2, "{F2}" },
                                      { Keys.F3, "{F3}" },
                                      { Keys.F4, "{F4}" },
                                      { Keys.F5, "{F5}" },
                                      { Keys.F6, "{F6}" },
                                      { Keys.F7, "{F7}" },
                                      { Keys.F8, "{F8}" },
                                      { Keys.F9, "{F9}" },
                                      { Keys.F10, "{F10}" },
                                      { Keys.F11, "{F11}" },
                                      { Keys.F12, "{F12}" },
                                      { Keys.Add, "{ADD}" },
                                      { Keys.Subtract, "{SUBTRACT}" },
                                      { Keys.Multiply, "{MULTIPLY}" },
                                      { Keys.Divide, "{DIVIDE}" }
                                  };

      foreach(var replacement in replacementDictionary)
        value = value.Replace (replacement.Key, replacement.Value);

      return value;
    }

    private static string TransformModifierKeys (string value)
    {
      var replacementDictionary = new Dictionary<string, string>
                                  {
                                      { Keys.Shift, "+" },
                                      { Keys.Control, "^" },
                                      { Keys.Alt, "%" }
                                  };

      foreach(var replacement in replacementDictionary)
      {
        // Todo RM-6297: TransformModifierKeys
        
        // Replace: Keys.Shift + ABC + Keys.Shift + DEF + Keys.Shift + GHI
        //    with: +(ABC)DEF+(GHI)
        // etc. pp.
      }

      return value;
    }
  }
}