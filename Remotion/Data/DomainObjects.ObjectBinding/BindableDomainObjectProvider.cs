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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// The implementation of  <see cref="IBusinessObjectProvider"/> to be used with the <see cref="BindableDomainObjectMixin"/>.
  /// </summary>
  /// <remarks>
  /// This provider is associated with the <see cref="BindableDomainObjectMixin"/> via the <see cref="BindableDomainObjectProviderAttribute"/>.
  /// </remarks>
  public class BindableDomainObjectProvider : BindableObjectProvider
  {
    public BindableDomainObjectProvider (IMetadataFactory metadataFactory, IBusinessObjectServiceFactory serviceFactory)
        : base (metadataFactory, serviceFactory)
    {
    }

    public BindableDomainObjectProvider ()
        : base (BindableDomainObjectMetadataFactory.Create())
    {
    }
  }
}