// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Mixins;
using Remotion.Utilities;
using Remotion.Reflection;

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
      return ObjectFactory.Create<BindableObjectServiceFactory> (true, ParamList.Empty);
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
