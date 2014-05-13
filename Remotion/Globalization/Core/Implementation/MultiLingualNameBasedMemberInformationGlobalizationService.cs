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
using System.Threading;
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Retrieves the human-readable localized representation of reflection objects based on the <see cref="MultiLingualNameAttribute"/> 
  /// applied to the respective reflection object.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  public class MultiLingualNameBasedMemberInformationGlobalizationService : IMemberInformationGlobalizationService
  {
    private readonly ConcurrentDictionary<ITypeInformation, Lazy<Dictionary<CultureInfo, string>>> _localizedTypeNamesForTypeInformation =
        new ConcurrentDictionary<ITypeInformation, Lazy<Dictionary<CultureInfo, string>>>(); 

    public MultiLingualNameBasedMemberInformationGlobalizationService ()
    {
    }

    public bool TryGetTypeDisplayName (ITypeInformation typeInformation, ITypeInformation typeInformationForResourceResolution, out string result)
    {
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      for (var currentTypeInformation = typeInformation; currentTypeInformation != null; currentTypeInformation = currentTypeInformation.BaseType)
      {
        var localizedName = GetLocalizedNameForCurrentUICulture (currentTypeInformation);
        if (localizedName != null)
        {
          result = localizedName;
          return true;
        }
      }

      result = null;
      return false;
    }

    public bool TryGetPropertyDisplayName (
        IPropertyInformation propertyInformation,
        ITypeInformation typeInformationForResourceResolution,
        out string result)
    {
      throw new NotImplementedException();
    }

    [CanBeNull]
    private string GetLocalizedNameForCurrentUICulture (ITypeInformation typeInformation)
    {
      var attributes = GetMultiLingualNameAttributesFromCache (typeInformation);
      if (!attributes.Any())
        return null;

      var currentUICulture = CultureInfo.CurrentUICulture;
      foreach (var cultureInfo in currentUICulture.GetCultureHierarchy())
      {
        string localizedName;
        if (attributes.TryGetValue (cultureInfo, out localizedName))
          return localizedName;
      }

      throw new MissingLocalizationException (
          string.Format (
              "The type '{0}' has one or more MultiLingualNameAttributes applied "
              + "but does not define a localization for the current UI culture '{1}' or a valid fallback culture "
              + "(i.e. there is no localization defined for the invariant culture).",
              typeInformation.FullName,
              currentUICulture));
    }

    private Dictionary<CultureInfo, string> GetMultiLingualNameAttributesFromCache (ITypeInformation typeInformation)
    {
      var lazyAttributes = _localizedTypeNamesForTypeInformation.GetOrAdd (
          typeInformation,
          new Lazy<Dictionary<CultureInfo, string>> (
              () => GetMultiLingualNameAttributes (typeInformation),
              LazyThreadSafetyMode.ExecutionAndPublication));

      return lazyAttributes.Value;
    }

    private Dictionary<CultureInfo, string> GetMultiLingualNameAttributes (ITypeInformation typeInformation)
    {
      var attributes = new Dictionary<CultureInfo, string>();
      foreach (var attribute in typeInformation.GetCustomAttributes<MultiLingualNameAttribute> (false))
      {
        if (attributes.ContainsKey (attribute.Culture))
        {
          throw new InvalidOperationException (
              string.Format (
                  "The type '{0}' has more than one MultiLingualNameAttribute for the culture '{1}' applied. "
                  + "The used cultures must be unique within the set of MultiLingualNameAttributes for a type.",
                  typeInformation.FullName,
                  attribute.Culture));
        }
        attributes.Add (attribute.Culture, attribute.LocalizedName);
      }
      return attributes;
    }
  }
}