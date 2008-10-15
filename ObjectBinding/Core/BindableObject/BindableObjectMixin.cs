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
using Remotion.Mixins.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Apply this mixin to a type in order to add an <see cref="IBusinessObject"/> implementation if you cannot derive from 
  /// <see cref="BindableObjectBase"/>.
  /// </summary>
  [Serializable]
  [BindableObjectProvider]
  public class BindableObjectMixin : BindableObjectMixinBase<object>
  {
    public BindableObjectMixin ()
    {
    }

    protected override BindableObjectClass InitializeBindableObjectClass()
    {
      Type targetType = MixinTypeUtility.GetUnderlyingTargetType (This.GetType());
      return BindableObjectProvider.GetBindableObjectClass (targetType);
    }
  }
}
