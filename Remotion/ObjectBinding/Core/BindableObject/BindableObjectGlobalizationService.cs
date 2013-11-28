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
using Remotion.ExtensibleEnums;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.Globalization.Implementation;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  //TODO: doc
  public class BindableObjectGlobalizationService : IBindableObjectGlobalizationService
  {
    [ResourceIdentifiers]
    [MultiLingualResources ("Remotion.ObjectBinding.Globalization.BindableObjectGlobalizationService")]
    private enum ResourceIdentifier
    {
      True,
      False
    }

    private readonly IMemberInformationGlobalizationService _memberInformationGlobalizationService;
    private readonly DoubleCheckedLockingContainer<IResourceManager> _resourceManager;
    private readonly IEnumerationGlobalizationService _enumerationGlobalizationService;
    private readonly IExtensibleEnumerationGlobalizationService _extensibleEnumerationGlobalizationService;

    public BindableObjectGlobalizationService (
      //TOOD AO: replace with ICompoundGloblaizationService
        IEnumerable<IGlobalizationService> globalizationServices,
        IMemberInformationGlobalizationService memberInformationGlobalizationService,
        IEnumerationGlobalizationService enumerationGlobalizationService,
        IExtensibleEnumerationGlobalizationService extensibleEnumerationGlobalizationService)
    {
      ArgumentUtility.CheckNotNull ("globalizationServices", globalizationServices);
      ArgumentUtility.CheckNotNull ("memberInformationGlobalizationService", memberInformationGlobalizationService);
      ArgumentUtility.CheckNotNull ("enumerationGlobalizationService", enumerationGlobalizationService);
      ArgumentUtility.CheckNotNull ("extensibleEnumerationGlobalizationService", extensibleEnumerationGlobalizationService);
      
      _resourceManager =
          new DoubleCheckedLockingContainer<IResourceManager> (
              () => new CompoundGlobalizationService (globalizationServices).GetResourceManager (typeof (ResourceIdentifier)));
      _memberInformationGlobalizationService = memberInformationGlobalizationService;
      _enumerationGlobalizationService = enumerationGlobalizationService;
      _extensibleEnumerationGlobalizationService = extensibleEnumerationGlobalizationService;
    }

    public string GetEnumerationValueDisplayName (Enum value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      return _enumerationGlobalizationService.GetEnumerationValueDisplayName (value);
    }

    public string GetExtensibleEnumerationValueDisplayName (IExtensibleEnum value) //move to member info globalization service
    {
      ArgumentUtility.CheckNotNull ("value", value);
      return _extensibleEnumerationGlobalizationService.GetExtensibleEnumerationValueDisplayName (value);
    }

    public string GetBooleanValueDisplayName (bool value)
    {
      return _resourceManager.Value.GetString (value ? ResourceIdentifier.True : ResourceIdentifier.False);
    }

    public string GetPropertyDisplayName (ITypeInformation typeInfo, IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("typeInfo", typeInfo);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      // Note: Currently, MixedMultilingualResources requires the concrete mixed type and the concrete implemented property for globalization 
      // attribute analysis. We need to extract that information from BindableObjectMixinIntroducedPropertyInformation. The goal is to redesign mixin-
      // based globalization some time, so that we can work with ordinary IPropertyInformation objects

      var mixinIntroducedPropertyInformation = propertyInfo as BindableObjectMixinIntroducedPropertyInformation;
      var property = mixinIntroducedPropertyInformation == null
          ? propertyInfo
          : mixinIntroducedPropertyInformation.FindInterfaceDeclarations()  //TODO AO: should be cached!?
              .Single (
                  () =>
                      new InvalidOperationException (
                          string.Format (
                              "BindableObjectGlobalizationService only supports unique interface declarations but proerty '{0}' is declared on multiply interfaces",
                              propertyInfo.Name)));

      return _memberInformationGlobalizationService.GetPropertyDisplayName (property, typeInfo);
    }
  }
}