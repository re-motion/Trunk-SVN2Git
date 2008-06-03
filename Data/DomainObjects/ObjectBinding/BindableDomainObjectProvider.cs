/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
