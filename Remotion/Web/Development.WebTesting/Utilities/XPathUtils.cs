using System;
using System.Linq;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// XPath utility methods.
  /// </summary>
  public static class XPathUtils
  {
    /// <summary>
    /// Creates an XPath predicate, checking that a given <paramref name="attribtueName"/> has the given <paramref name="attributeValue"/>.
    /// </summary>
    /// <param name="attribtueName">The name of the attribute to check.</param>
    /// <param name="attributeValue">The value, the attribute should have.</param>
    /// <returns>The XPath predicate.</returns>
    public static string CreateContainsAttributeCheck (string attribtueName, string attributeValue)
    {
      return string.Format ("[@{0}='{1}']", attribtueName, attributeValue);
    }

    /// <summary>
    /// Creates an XPath predicate, checking for a specific CSS class.
    /// </summary>
    /// <param name="cssClass">The CSS class to check for.</param>
    /// <returns>The XPath predicate.</returns>
    public static string CreateContainsClassCheck (string cssClass)
    {
      return string.Format ("[{0}]", CreateClassCheckClause (cssClass));
    }

    /// <summary>
    /// Creates an XPath predicate, checking for "at least one" of the given CSS classes.
    /// </summary>
    /// <param name="cssClasses">The CSS classes to check for.</param>
    /// <returns>The XPath predicate.</returns>
    public static string CreateContainsOneOfClassesCheck (params string[] cssClasses)
    {
      var checkClauses = cssClasses.Select (CreateClassCheckClause);
      return "[" + string.Join (" or ", checkClauses) + "]";
    }

    /// <summary>
    /// Creates an XPath predicate clause, checking for a specific CSS class.
    /// </summary>
    /// <remarks>
    /// See http://stackoverflow.com/a/9133579/1400869 for more information on the implementation.
    /// </remarks>
    /// <param name="cssClass">The CSS class to check for.</param>
    /// <returns>The XPath predicate clause.</returns>
    private static string CreateClassCheckClause (string cssClass)
    {
      return string.Format ("contains(concat(' ', normalize-space(@class), ' '), ' {0} ')", cssClass);
    }
  }
}