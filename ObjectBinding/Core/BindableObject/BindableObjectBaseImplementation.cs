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
  public class BindableObjectBaseImplementation : BindableObjectMixin, IDeserializationCallback
  {
    public static BindableObjectBaseImplementation Create (BindableObjectBase wrapper)
    {
      ArgumentUtility.CheckNotNull ("wrapper", wrapper);
      Assertion.DebugAssert (!ReflectionUtility.CanAscribe (typeof (BindableObjectBaseImplementation), typeof (Mixin<,>)),
          "we assume the mixin does not have a base object");
      return MixinTargetMockUtility.CreateMixinWithMockedTarget<BindableObjectBaseImplementation, object> (wrapper, wrapper);
    }

    private readonly BindableObjectBase _wrapper;

    public BindableObjectBaseImplementation (BindableObjectBase wrapper)
    {
      ArgumentUtility.CheckNotNull ("wrapper", wrapper);
      _wrapper = wrapper;
    }

    void IDeserializationCallback.OnDeserialization(object sender)
    {
      Assertion.DebugAssert (!ReflectionUtility.CanAscribe (typeof (BindableObjectMixin), typeof (Mixin<,>)),
          "we assume the mixin does not have a base object");
      MixinTargetMockUtility.SignalOnDeserialization (this, _wrapper);
    }
  }
}