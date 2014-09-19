using System;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// XPath utility methods.
  /// </summary>
  public static class XPathUtils
  {
    /// <summary>
    /// Creates an XPath predicate, checking for a specific CSS class.
    /// </summary>
    /// <remarks>
    /// See http://stackoverflow.com/a/9133579/1400869 for more information on the implementation.
    /// </remarks>
    /// <param name="cssClass">The CSS class to check for.</param>
    /// <returns>The XPath predicate.</returns>
    public static string CreateContainsClassCheck (string cssClass)
    {
      return string.Format ("[contains(concat(' ', normalize-space(@class), ' '), ' {0} ')]", cssClass);
    }
  }
}