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
using System.Runtime.Serialization;
using Remotion.Mixins;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  [Serializable]
  public class BindableObjectWithIdentityBaseImplementation : BindableObjectWithIdentityMixin, IDeserializationCallback
  {
    public static BindableObjectWithIdentityBaseImplementation Create (BindableObjectWithIdentityBase wrapper)
    {
      ArgumentUtility.CheckNotNull ("wrapper", wrapper);
      Assertion.DebugAssert (!ReflectionUtility.CanAscribe (typeof (BindableObjectWithIdentityBaseImplementation), typeof (Mixin<,>)),
          "we assume the mixin does not have a base object");
      return MixinTargetMockUtility.CreateMixinWithMockedTarget<BindableObjectWithIdentityBaseImplementation, object> (wrapper, wrapper);
    }

    private readonly BindableObjectWithIdentityBase _wrapper;

    public BindableObjectWithIdentityBaseImplementation (BindableObjectWithIdentityBase wrapper)
    {
      ArgumentUtility.CheckNotNull ("wrapper", wrapper);
      _wrapper = wrapper;
    }

    public override string UniqueIdentifier
    {
      get { return ((BindableObjectWithIdentityBase) This).UniqueIdentifier; }
    }

    void IDeserializationCallback.OnDeserialization (object sender)
    {
      Assertion.DebugAssert (!ReflectionUtility.CanAscribe (typeof (BindableObjectWithIdentityBaseImplementation), typeof (Mixin<,>)),
          "we assume the mixin does not have a base object");
      MixinTargetMockUtility.SignalOnDeserialization (this, _wrapper);
    }
  }
}