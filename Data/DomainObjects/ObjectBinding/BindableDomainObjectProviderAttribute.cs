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
using Remotion.ObjectBinding;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// Apply this attribute to your BindableDomainObject-type to classify the this type as using the DomainObject-specific implementation of object binding.
  /// </summary>
  /// <remarks>
  /// The <see cref="BindableDomainObjectMixin"/> already applies this attribute.
  /// </remarks>
  public class BindableDomainObjectProviderAttribute : BusinessObjectProviderAttribute
  {
    public BindableDomainObjectProviderAttribute ()
        : base (typeof (BindableDomainObjectProvider))
    {
    }
  }
}
