// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  /// <summary>
  /// The <see cref="SecurityManagerObjectServiceFactoryMixin"/> is an extension of the <see cref="BindableObjectServiceFactory"/> used by
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
  ///     <term><see cref="UserPropertiesSearchService"/></term>
  ///     <description><see cref="UserPropertiesSearchService"/></description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="GroupPropertiesSearchService"/></term>
  ///     <description><see cref="GroupPropertiesSearchService"/></description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="RolePropertiesSearchService"/></term>
  ///     <description><see cref="RolePropertiesSearchService"/></description>
  ///   </item>
  /// </list>
  /// </remarks>
  [CLSCompliant (false)]
  [Extends (typeof (BindableObjectServiceFactory), AdditionalDependencies = new Type[] {typeof (BindableDomainObjectServiceFactoryMixin)})]
  public class SecurityManagerObjectServiceFactoryMixin
      : Mixin<BindableObjectServiceFactory, IBusinessObjectServiceFactory>, IBusinessObjectServiceFactory
  {
    public SecurityManagerObjectServiceFactoryMixin ()
    {
    }

    [OverrideTarget]
    public virtual IBusinessObjectService CreateService (IBusinessObjectProviderWithIdentity provider, Type serviceType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("serviceType", serviceType, typeof (IBusinessObjectService));

      if (serviceType == typeof (UserPropertiesSearchService))
        return new UserPropertiesSearchService();

      if (serviceType == typeof (GroupPropertiesSearchService))
        return new GroupPropertiesSearchService();

      if (serviceType == typeof (RolePropertiesSearchService))
        return new RolePropertiesSearchService();

      if (serviceType == typeof (SubstitutionPropertiesSearchService))
        return new SubstitutionPropertiesSearchService ();

      if (serviceType == typeof (AccessControlEntryPropertiesSearchService))
        return new AccessControlEntryPropertiesSearchService ();

      return Base.CreateService (provider, serviceType);
    }
  }
}
