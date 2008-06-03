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

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Apply this attribute to your BindableObjectWithIdentity-type to classify the this type as using the default reflection based object binding implementation.
  /// </summary>
  /// <remarks>
  /// The <see cref="BindableObjectMixin"/> already applies this attribute.
  /// </remarks>
  public class BindableObjectWithIdentityProviderAttribute : BusinessObjectProviderAttribute
  {
    public BindableObjectWithIdentityProviderAttribute ()
        : base (typeof (BindableObjectProvider))
    {
    }
  }
}
