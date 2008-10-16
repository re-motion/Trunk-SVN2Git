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
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Apply to a class to add a reflection-based implementation of <see cref="IBusinessObjectWithIdentity"/> to the class via 
  /// <see cref="BindableObjectWithIdentityMixin"/>. Use this attribute if deriving from <see cref="BindableObjectWithIdentityBase"/> is not possible.
  /// </summary>
  /// <remarks>This attribute adds the <see cref="BindableObjectWithIdentityMixin"/> to its target class. 
  /// Use <see cref="ObjectFactory"/> to instantiate the target class.</remarks>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class BindableObjectWithIdentityAttribute : UsesAttribute
  {
    public BindableObjectWithIdentityAttribute ()
        : base (typeof (BindableObjectWithIdentityMixin))
    {
    }
  }
}
