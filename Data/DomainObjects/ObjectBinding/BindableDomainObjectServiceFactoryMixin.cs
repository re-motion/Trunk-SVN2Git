/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// The <see cref="BindableDomainObjectServiceFactoryMixin"/> is an extension of the <see cref="BindableObjectServiceFactory"/> used by
  /// the <see cref="BindableObjectProvider"/> and provides default service instances for bindable domain object implementations.
  /// </summary>
  /// <remarks>
  /// The following <see cref="IBusinessObjectService"/> interfaces are supported.
  /// <list type="bullet">
  ///   <listheader>
  ///     <term>Service Interface</term>
  ///     <description>Service creates instance of type</description>
  ///   </listheader>
  ///   <item>
  ///     <term><see cref="IGetObjectService"/></term>
  ///     <description><see cref="BindableDomainObjectGetObjectService"/></description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="ISearchAvailableObjectsService"/></term>
  ///     <description><see cref="BindableDomainObjectSearchService"/></description>
  ///   </item>
  /// </list>
  /// </remarks>
  [Extends (typeof (BindableObjectServiceFactory))]
  public class BindableDomainObjectServiceFactoryMixin
      : Mixin<BindableObjectServiceFactory, IBusinessObjectServiceFactory>, IBusinessObjectServiceFactory
  {
    public BindableDomainObjectServiceFactoryMixin ()
    {
    }

    [OverrideTarget]
    public virtual IBusinessObjectService CreateService (IBusinessObjectProviderWithIdentity provider, Type serviceType)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("serviceType", serviceType, typeof (IBusinessObjectService));

      if (provider.ProviderAttribute is BindableDomainObjectProviderAttribute)
      {
        if (serviceType == typeof (IGetObjectService))
          return new BindableDomainObjectGetObjectService();

        if (serviceType == typeof (ISearchAvailableObjectsService))
          return new BindableDomainObjectSearchService();
      }

      return Base.CreateService (provider, serviceType);
    }
  }
}
