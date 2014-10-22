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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Implementation infrastructure for resolving the localized name of the <typeparamref name="TReflectionObject"/> 
  /// based on the applied <see cref="MultiLingualNameAttribute"/>s.
  /// </summary>
  /// <typeparam name="TReflectionObject">An <see cref="IMemberInformation"/> or <see cref="MemberInfo"/> to retrieve the localized name for.</typeparam>
  public abstract class LocalizedNameProviderBase<TReflectionObject>
      where TReflectionObject : class
  {
    private readonly ConcurrentDictionary<TReflectionObject, Lazy<Dictionary<CultureInfo, string>>> _localizedTypeNamesForTypeInformation =
        new ConcurrentDictionary<TReflectionObject, Lazy<Dictionary<CultureInfo, string>>>();

    protected LocalizedNameProviderBase ()
    {
    }

    [NotNull]
    protected abstract IEnumerable<MultiLingualNameAttribute> GetCustomAttributes ([NotNull] TReflectionObject reflectionObject);

    [NotNull]
    protected abstract string GetContextForExceptionMessage ([NotNull] TReflectionObject reflectionObject);

    public bool TryGetLocalizedNameForCurrentUICulture ([NotNull] TReflectionObject reflectionObject, [CanBeNull] out string result)
    {
      ArgumentUtility.CheckNotNull ("reflectionObject", reflectionObject);

      var attributes = GetMultiLingualNameAttributesFromCache (reflectionObject);
      if (!attributes.Any())
      {
        result = null;
        return false;
      }

      var currentUICulture = CultureInfo.CurrentUICulture;
      foreach (var cultureInfo in currentUICulture.GetCultureHierarchy())
      {
        if (attributes.TryGetValue (cultureInfo, out result))
          return true;
      }

      throw new MissingLocalizationException (
          string.Format (
              "{0} has one or more MultiLingualNameAttributes applied "
              + "but does not define a localization for the current UI culture '{1}' or a valid fallback culture "
              + "(i.e. there is no localization defined for the invariant culture).",
              GetContextForExceptionMessage (reflectionObject),
              currentUICulture));
    }

    private Dictionary<CultureInfo, string> GetMultiLingualNameAttributesFromCache (TReflectionObject reflectionObject)
    {
      var lazyAttributes = _localizedTypeNamesForTypeInformation.GetOrAdd (
          reflectionObject,
          new Lazy<Dictionary<CultureInfo, string>> (
              () => GetMultiLingualNameAttributes (reflectionObject),
              LazyThreadSafetyMode.ExecutionAndPublication));

      return lazyAttributes.Value;
    }

    private Dictionary<CultureInfo, string> GetMultiLingualNameAttributes (TReflectionObject reflectionObject)
    {
      var attributes = new Dictionary<CultureInfo, string>();
      foreach (var attribute in GetCustomAttributes (reflectionObject))
      {
        if (attributes.ContainsKey (attribute.Culture))
        {
          throw new InvalidOperationException (
              string.Format (
                  "{0} has more than one MultiLingualNameAttribute for the culture '{1}' applied. "
                  + "The used cultures must be unique within the set of MultiLingualNameAttributes for a type.",
                  GetContextForExceptionMessage (reflectionObject),
                  attribute.Culture));
        }
        attributes.Add (attribute.Culture, attribute.LocalizedName);
      }
      return attributes;
    }
  }
}