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
using Remotion.ExtensibleEnums;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.Globalization.ExtensibleEnums;
using Remotion.Globalization.Implementation;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Defines a facade for localizing the information exposed by the <see cref="IBusinessObject"/> interfaces via various globalization services.
  /// </summary>
  /// <remarks>
  /// Implementations of the <see cref="BindableObjectGlobalizationService"/> can be directly registered with <see cref="BusinessObjectProvider"/> 
  /// using the <see cref="BusinessObjectProvider.AddService"/> method or indirectly by providing a custom implementation of the 
  /// <see cref="IBusinessObjectServiceFactory"/>.
  /// </remarks>
  [ConcreteImplementation(typeof(BindableObjectGlobalizationService), Lifetime = LifetimeKind.Singleton)]
  public sealed class BindableObjectGlobalizationService : IBusinessObjectService
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
        ICompoundGlobalizationService globalizationServices,
        IMemberInformationGlobalizationService memberInformationGlobalizationService,
        IEnumerationGlobalizationService enumerationGlobalizationService,
        IExtensibleEnumerationGlobalizationService extensibleEnumerationGlobalizationService)
    {
      ArgumentUtility.CheckNotNull ("globalizationServices", globalizationServices);
      ArgumentUtility.CheckNotNull ("memberInformationGlobalizationService", memberInformationGlobalizationService);
      ArgumentUtility.CheckNotNull ("enumerationGlobalizationService", enumerationGlobalizationService);
      ArgumentUtility.CheckNotNull ("extensibleEnumerationGlobalizationService", extensibleEnumerationGlobalizationService);
      
      _resourceManager =
          new DoubleCheckedLockingContainer<IResourceManager> (() => globalizationServices.GetResourceManager (typeof (ResourceIdentifier)));
      _memberInformationGlobalizationService = memberInformationGlobalizationService;
      _enumerationGlobalizationService = enumerationGlobalizationService;
      _extensibleEnumerationGlobalizationService = extensibleEnumerationGlobalizationService;
    }

    /// <summary>
    /// Gets the localized display name of an enumeration value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The localized display name.</returns>
    public string GetEnumerationValueDisplayName (Enum value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      return _enumerationGlobalizationService.GetEnumerationValueDisplayName (value);
    }

    /// <summary>
    /// Gets the localized display name of the extensible enumeration value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The localized display name.</returns>
    public string GetExtensibleEnumerationValueDisplayName (IExtensibleEnum value) //move to member info globalization service
    {
      ArgumentUtility.CheckNotNull ("value", value);
      return _extensibleEnumerationGlobalizationService.GetExtensibleEnumerationValueDisplayName (value);
    }

    /// <summary>
    /// Gets the localized display name of a boolean value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The localized display name.</returns>
    public string GetBooleanValueDisplayName (bool value)
    {
      return _resourceManager.Value.GetString (value ? ResourceIdentifier.True : ResourceIdentifier.False);
    }

    /// <summary>
    /// Gets the localized display name of a type.
    /// </summary>
    /// <param name="typeInformation">The <see cref="ITypeInformation"/> for which to lookup the resource.</param>
    /// <param name="typeInformationForResourceResolution">The <see cref="ITypeInformation"/> providing the resources.</param>
    /// <returns>The localized display name.</returns>
    public string GetTypeDisplayName (ITypeInformation typeInformation, ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      return _memberInformationGlobalizationService.GetTypeDisplayName (typeInformation, typeInformationForResourceResolution);
    }

    /// <summary>
    /// Gets the localized display name of a property.
    /// </summary>
    /// <param name="propertyInformation">The <see cref="IPropertyInformation"/> for which to lookup the resource.</param>
    /// <param name="typeInformationForResourceResolution">The <see cref="ITypeInformation"/> providing the resources.</param>
    /// <returns>The localized display name.</returns>
    public string GetPropertyDisplayName (IPropertyInformation propertyInformation, ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      // Note: Currently, MixedMultilingualResources requires the concrete mixed type and the concrete implemented property for globalization 
      // attribute analysis. We need to extract that information from BindableObjectMixinIntroducedPropertyInformation. The goal is to redesign mixin-
      // based globalization some time, so that we can work with ordinary IPropertyInformation objects

      var mixinIntroducedPropertyInformation = propertyInformation as BindableObjectMixinIntroducedPropertyInformation;
      var property = mixinIntroducedPropertyInformation == null
          ? propertyInformation
          : mixinIntroducedPropertyInformation.FindInterfaceDeclarations()  //Is already evaluated within implementation -> not performance relevant
              .Single (
                  () =>
                      new InvalidOperationException (
                          string.Format (
                              "BindableObjectGlobalizationService only supports unique interface declarations but proerty '{0}' is declared on multiply interfaces",
                              propertyInformation.Name)));

      return _memberInformationGlobalizationService.GetPropertyDisplayName(property, typeInformationForResourceResolution);
    }
  }
}