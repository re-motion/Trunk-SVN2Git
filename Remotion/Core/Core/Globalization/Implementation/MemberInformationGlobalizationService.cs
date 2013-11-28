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
  /// Retrieving the human-readable localized representation of reflection objects.
  /// </summary>
  public class MemberInformationGlobalizationService : IMemberInformationGlobalizationService
  {
    private readonly IGlobalizationService _globalizationService;
    private readonly IMemberInfoNameResolver _memberInfoNameResolver;

    public MemberInformationGlobalizationService (ICompoundGlobalizationService globalizationService, IMemberInfoNameResolver memberInfoNameResolver)
    {
      ArgumentUtility.CheckNotNull ("globalizationService", globalizationService);
      ArgumentUtility.CheckNotNull ("memberInfoNameResolver", memberInfoNameResolver);

      _globalizationService = globalizationService;
      _memberInfoNameResolver = memberInfoNameResolver;
    }

    public string GetPropertyDisplayName (IPropertyInformation propertyInformation, ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      return GetString (typeInformationForResourceResolution, propertyInformation.Name, _memberInfoNameResolver.GetPropertyName (propertyInformation), "property:");
    }

    public string GetTypeDisplayName (ITypeInformation typeInformation, ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      return GetString (typeInformationForResourceResolution, typeInformation.Name, _memberInfoNameResolver.GetTypeName (typeInformation), "type:");
    }

    private string GetString (ITypeInformation typeInformation, string shortMemberName, string longMemberName, string resourcePrefix)
    {
      var resourceManager = _globalizationService.GetResourceManager (typeInformation);

      return resourceManager.GetStringOrDefault (resourcePrefix + longMemberName)
             ?? resourceManager.GetStringOrDefault (resourcePrefix + shortMemberName)
             ?? shortMemberName;
    }
  }
}