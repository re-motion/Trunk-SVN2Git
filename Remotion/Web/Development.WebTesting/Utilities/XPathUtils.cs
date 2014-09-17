using System;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Utilities class for various XPath-related tasks.
  /// </summary>
  public static class XPathUtils
  {
    /// <summary>
    /// Creates an XPath check constraint for a specific CSS class.
    /// </summary>
    /// <remarks>
    /// See http://stackoverflow.com/a/9133579/1400869 for the source of the check constraint.
    /// </remarks>
    /// <param name="cssClass">The CSS class to check for.</param>
    /// <returns>The XPath check constraint.</returns>
    public static string CreateContainsClassCheck (string cssClass)
    {
      return string.Format ("[contains(concat(' ', normalize-space(@class), ' '), ' {0} ')]", cssClass);
    }
  }
}