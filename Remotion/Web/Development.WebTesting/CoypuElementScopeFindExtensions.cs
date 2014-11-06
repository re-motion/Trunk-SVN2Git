// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

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
    /// Find an element with the given <paramref name="tagSelector"/> bearing one or more given diagnostic metadata attribute name/value combinations.
    /// </summary>
    /// <param name="scope">The parent <see cref="ElementScope"/> which serves as the root element for the search.</param>
    /// <param name="tagSelector">The CSS selector for the HTML tags to check for the diagnostic metadata attributes.</param>
    /// <param name="diagnosticMetadata">The diagnostic metadata attribute name/value pairs to check for.</param>
    /// <returns>The <see cref="ElementScope"/> of the found element.</returns>
    public static ElementScope FindDMA (
        [NotNull] this ElementScope scope,
        [NotNull] string tagSelector,
        [NotNull] IDictionary<string, string> diagnosticMetadata)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("tagSelector", tagSelector);
      ArgumentUtility.CheckNotNull ("diagnosticMetadata", diagnosticMetadata);

      const string dmaCheckPattern = "[{0}='{1}']";
      var dmaCheck = string.Concat (diagnosticMetadata.Select (dm => string.Format (dmaCheckPattern, dm.Key, dm.Value)));
      var cssSelector = tagSelector + dmaCheck;
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