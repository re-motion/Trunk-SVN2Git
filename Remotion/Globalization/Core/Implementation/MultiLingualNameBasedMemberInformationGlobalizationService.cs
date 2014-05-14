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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Retrieves the human-readable localized representation of reflection objects based on the <see cref="MultiLingualNameAttribute"/> 
  /// applied to the respective reflection object.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  public sealed class MultiLingualNameBasedMemberInformationGlobalizationService : IMemberInformationGlobalizationService
  {
    private class LocalizedNameForTypeInformationProvider : LocalizedNameProviderBase<ITypeInformation>
    {
      protected override MultiLingualNameAttribute[] GetCustomAttributes (ITypeInformation reflectionObject)
      {
        ArgumentUtility.CheckNotNull ("reflectionObject", reflectionObject);

        return reflectionObject.GetCustomAttributes<MultiLingualNameAttribute> (false);
      }

      protected override string GetContextForExceptionMessage (ITypeInformation reflectionObject)
      {
        ArgumentUtility.CheckNotNull ("reflectionObject", reflectionObject);

        return string.Format ("The type '{0}'", reflectionObject.FullName);
      }
    }

    private class LocalizedNameForPropertyInformationProvider : LocalizedNameProviderBase<IPropertyInformation>
    {
      protected override MultiLingualNameAttribute[] GetCustomAttributes (IPropertyInformation reflectionObject)
      {
        ArgumentUtility.CheckNotNull ("reflectionObject", reflectionObject);

        return reflectionObject.GetCustomAttributes<MultiLingualNameAttribute> (false);
      }

      protected override string GetContextForExceptionMessage (IPropertyInformation reflectionObject)
      {
        ArgumentUtility.CheckNotNull ("reflectionObject", reflectionObject);

        return string.Format ("The property '{0}' declared on type '{1}'", reflectionObject.Name, reflectionObject.DeclaringType.FullName);
      }
    }


    private readonly LocalizedNameForTypeInformationProvider _localizedNameForTypeInformationProvider
        = new LocalizedNameForTypeInformationProvider();

    private readonly LocalizedNameForPropertyInformationProvider _localizedNameForPropertyInformationProvider =
        new LocalizedNameForPropertyInformationProvider();

    public MultiLingualNameBasedMemberInformationGlobalizationService ()
    {
    }

    public bool TryGetTypeDisplayName (ITypeInformation typeInformation, ITypeInformation typeInformationForResourceResolution, out string result)
    {
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      for (var currentTypeInformation = typeInformation; currentTypeInformation != null; currentTypeInformation = currentTypeInformation.BaseType)
      {
        var localizedName = _localizedNameForTypeInformationProvider.GetLocalizedNameForCurrentUICulture (currentTypeInformation);
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
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      var localizedName = _localizedNameForPropertyInformationProvider.GetLocalizedNameForCurrentUICulture (propertyInformation);
      if (localizedName != null)
      {
        result = localizedName;
        return true;
      }

      result = null;
      return false;
    }
  }
}