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
using System.Globalization;
using JetBrains.Annotations;
using Remotion.Globalization.Implementation;
using Remotion.Utilities;

namespace Remotion.Globalization
{
  /// <summary>
  /// The <see cref="MultiLingualNameAttribute"/> can be applied to types, properties, enum values, and methods (e.g. for extensible enum values) 
  /// to specify the <see cref="LocalizedName"/> for a specific <see cref="Culture"/>.
  /// </summary>
  /// <remarks>
  /// <list type="bullet">
  /// <item>For <see cref="MultiLingualNameAttribute"/> must be applied to the original declaration of a member. </item>
  /// <item>For types, the derived type does not inherit the base type's localization.</item>
  /// <item>
  ///   One of the attributes should always specifiy the localization for the invariant culture. Otherwise, a <see cref="MissingLocalizationException"/> 
  ///   is thrown if a localziation look-up is performed for a UI-culture not part of the defined cultures for this reflection object.
  /// </item>
  /// </list>
  /// </remarks>
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = true)]
  public class MultiLingualNameAttribute : Attribute
  {
    private readonly string _localizedName;
    private readonly CultureInfo _culture;

    /// <summary>
    /// Initializes an instance of the <see cref="MultiLingualNameAttribute"/>.
    /// </summary>
    /// <param name="localizedName">The localized representation of the member name. Must not be <see langword="null" /> or empty.</param>
    /// <param name="culture">
    /// The identifier of the culture for which the localization is applied. Use an empty string for the invariant culture. Must not be <see langword="null" />.
    /// </param>
    public MultiLingualNameAttribute ([NotNull] string localizedName, [NotNull] string culture)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("localizedName", localizedName);
      ArgumentUtility.CheckNotNull ("culture", culture);

      _localizedName = localizedName;
      _culture = CultureInfo.GetCultureInfo (culture);
    }

    [NotNull]
    public string LocalizedName
    {
      get { return _localizedName; }
    }

    [NotNull]
    public CultureInfo Culture
    {
      get { return _culture; }
    }
  }
}