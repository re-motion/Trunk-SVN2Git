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
using System.Diagnostics;
using Remotion.Mixins;
using Remotion.ObjectBinding;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Apply this mixin to a type in order to add an <see cref="IBusinessObjectWithIdentity"/> implementation.
  /// </summary>
  [Serializable]
  [BindableObjectWithIdentityProvider]
  [CopyCustomAttributes (typeof (BindableObjectWithIdentityMixin.DebuggerDisplay))]
  public abstract class BindableObjectWithIdentityMixin : BindableObjectMixin, IBusinessObjectWithIdentity
  {
    [DebuggerDisplay ("{UniqueIdentifier} ({((Remotion.Mixins.IMixinTarget)this).Configuration.Type.FullName})")]
    internal class DebuggerDisplay // the attributes of this class are copied to the target class
    {
    }

    public BindableObjectWithIdentityMixin ()
    {
    }

    public abstract string UniqueIdentifier { get; }
  }
}
