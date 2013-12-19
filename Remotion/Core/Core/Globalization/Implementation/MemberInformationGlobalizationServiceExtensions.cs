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
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  public static class MemberInformationGlobalizationServiceExtensions
  {
    [NotNull]
    public static string GetTypeDisplayName (
        this IMemberInformationGlobalizationService memberInformationGlobalizationService,
        ITypeInformation typeInformation,
        ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("memberInformationGlobalizationService", memberInformationGlobalizationService);
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      return GetTypeDisplayNameOrDefault (memberInformationGlobalizationService, typeInformation, typeInformationForResourceResolution)
             ?? typeInformation.Name;
    }

    [CanBeNull]
    public static string GetTypeDisplayNameOrDefault (
        this IMemberInformationGlobalizationService memberInformationGlobalizationService,
        ITypeInformation typeInformation,
        ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("memberInformationGlobalizationService", memberInformationGlobalizationService);
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      string resourceValue;
      if (memberInformationGlobalizationService.TryGetTypeDisplayName (typeInformation, typeInformationForResourceResolution, out resourceValue))
        return resourceValue;

      return null;
    }

    [NotNull]
    public static string GetPropertyDisplayName (
        this IMemberInformationGlobalizationService memberInformationGlobalizationService,
        IPropertyInformation propertyInformation,
        ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("memberInformationGlobalizationService", memberInformationGlobalizationService);
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      return GetPropertyDisplayNameOrDefault (memberInformationGlobalizationService, propertyInformation, typeInformationForResourceResolution)
             ?? propertyInformation.Name;
    }

    [CanBeNull]
    public static string GetPropertyDisplayNameOrDefault (
        this IMemberInformationGlobalizationService memberInformationGlobalizationService,
        IPropertyInformation propertyInformation,
        ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("memberInformationGlobalizationService", memberInformationGlobalizationService);
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      string resourceValue;
      if (memberInformationGlobalizationService.TryGetPropertyDisplayName (
          propertyInformation,
          typeInformationForResourceResolution,
          out resourceValue))
        return resourceValue;

      return null;
    }

    public static bool ContainsTypeDisplayName (
        this IMemberInformationGlobalizationService memberInformationGlobalizationService,
        ITypeInformation typeInformation,
        ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("memberInformationGlobalizationService", memberInformationGlobalizationService);
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      return GetTypeDisplayNameOrDefault (memberInformationGlobalizationService, typeInformation, typeInformationForResourceResolution)
             != null;
    }

    public static bool ContainsPropertyDisplayName (
        this IMemberInformationGlobalizationService memberInformationGlobalizationService,
        IPropertyInformation propertyInformation,
        ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("memberInformationGlobalizationService", memberInformationGlobalizationService);
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      return GetPropertyDisplayNameOrDefault (memberInformationGlobalizationService, propertyInformation, typeInformationForResourceResolution)
             != null;
    }
  }
}