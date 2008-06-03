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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// The <see cref="BindableObjectServiceFactory"/> is the default implementation of the <see cref="IBusinessObjectServiceFactory"/>
  /// and provides service instances common for all bindable object implementations.
  /// </summary>
  /// <remarks>
  /// The following <see cref="IBusinessObjectService"/> interfaces are supported.
  /// <list type="bullet">
  ///   <listheader>
  ///     <term>Service Interface</term>
  ///     <description>Service creates instance of type</description>
  ///   </listheader>
  ///   <item>
  ///     <term><see cref="IBindableObjectGlobalizationService"/></term>
  ///     <description><see cref="BindableObjectGlobalizationService"/></description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="IBusinessObjectStringFormatterService"/></term>
  ///     <description><see cref="BusinessObjectStringFormatterService"/></description>
  ///   </item>
  /// </list>
  /// </remarks>
  public class BindableObjectServiceFactory : IBusinessObjectServiceFactory
  {
    public static BindableObjectServiceFactory Create ()
    {
      return ObjectFactory.Create<BindableObjectServiceFactory> (true).With();
    }

    protected BindableObjectServiceFactory ()
    {
    }

    public virtual IBusinessObjectService CreateService (IBusinessObjectProviderWithIdentity provider, Type serviceType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("serviceType", serviceType, typeof (IBusinessObjectService));

      if (serviceType == typeof (IBindableObjectGlobalizationService))
        return new BindableObjectGlobalizationService();

      if (serviceType == typeof (IBusinessObjectStringFormatterService))
        return new BusinessObjectStringFormatterService();

      return null;
    }
  }
}
