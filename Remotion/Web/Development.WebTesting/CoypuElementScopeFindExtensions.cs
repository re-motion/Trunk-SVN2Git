using System;
using System.Collections.Generic;
using System.Linq;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Various additional Find*() extension methods for Coypu's <see cref="ElementScope"/> class.
  /// </summary>
  public static class CoypuElementScopeFindExtensions
  {
    /// <summary>
    /// Find an element with the given <paramref name="tagSelector"/> bearing a given diagnostic metadata attribute name/value combination.
    /// </summary>
    /// <param name="scope">The parent <see cref="ElementScope"/> which serves as the root element for the search.</param>
    /// <param name="tagSelector">The CSS selector for the HTML tags to check for the diagnostic metadata attributes.</param>
    /// <param name="diagnosticMetadataAttributeName">The diagnostic metadata attribute name.</param>
    /// <param name="diagnosticMetadataAttributeValue">The diagnostic metadata attribute value.</param>
    /// <returns>The <see cref="ElementScope"/> of the found element.</returns>
    public static ElementScope FindDMA (
        [NotNull] this ElementScope scope,
        [NotNull] string tagSelector,
        [NotNull] string diagnosticMetadataAttributeName,
        [NotNull] string diagnosticMetadataAttributeValue)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNullOrEmpty ("tagSelector", tagSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("diagnosticMetadataAttributeName", diagnosticMetadataAttributeName);
      ArgumentUtility.CheckNotNullOrEmpty ("diagnosticMetadataAttributeValue", diagnosticMetadataAttributeValue);

      var cssSelector = string.Format ("{0}[{1}='{2}']", tagSelector, diagnosticMetadataAttributeName, diagnosticMetadataAttributeValue);
      return scope.FindCss (cssSelector);
    }

    /// <summary>
    /// Find an element with one of the given <paramref name="tagSelectors"/> bearing a given diagnostic metadata attribute name/value combination.
    /// </summary>
    /// <param name="scope">The parent <see cref="ElementScope"/> which serves as the root element for the search.</param>
    /// <param name="tagSelectors">The CSS selectors for the HTML tags to check for the diagnostic metadata attributes.</param>
    /// <param name="diagnosticMetadata">The diagnostic metadata attribute name/value pairs to check for.</param>
    /// <returns>The <see cref="ElementScope"/> of the found element.</returns>
    public static ElementScope FindDMA (
        [NotNull] this ElementScope scope,
        [NotNull] ICollection<string> tagSelectors,
        [NotNull] IDictionary<string, string> diagnosticMetadata)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("tagSelectors", tagSelectors);
      ArgumentUtility.CheckNotNull ("diagnosticMetadata", diagnosticMetadata);

      const string dmaCheckPattern = "[{0}='{1}']";
      var dmaCheck = string.Concat (diagnosticMetadata.Select (dm => string.Format (dmaCheckPattern, dm.Key, dm.Value)));
      var cssSelector = string.Join (",", tagSelectors.Select (ts => ts + dmaCheck));
      return scope.FindCss (cssSelector);
    }

    /// <summary>
    /// Finds the first &lt;a&gt; element within the given <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The parent <see cref="ElementScope"/> which serves as the root element for the search.</param>
    /// <returns>The <see cref="ElementScope"/> of the found element.</returns>
    public static ElementScope FindLink ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      return scope.FindCss ("a");
    }
  }
}