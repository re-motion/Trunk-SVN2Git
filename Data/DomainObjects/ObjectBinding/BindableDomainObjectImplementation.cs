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

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  [Serializable]
  public class BindableDomainObjectImplementation: BindableDomainObjectMixin, IDeserializationCallback
  {
    public static BindableDomainObjectImplementation Create (BindableDomainObject wrapper)
    {
      ArgumentUtility.CheckNotNull ("wrapper", wrapper);
      Assertion.DebugAssert (!Utilities.ReflectionUtility.CanAscribe (typeof (BindableDomainObjectImplementation), typeof (Mixin<,>)),
          "we assume the mixin does not have a base object");
      return MixinTargetMockUtility.CreateMixinWithMockedTarget<BindableDomainObjectImplementation, IDomainObject> (wrapper, wrapper);
    }

    private readonly BindableDomainObject _wrapper;

    protected BindableDomainObjectImplementation (BindableDomainObject wrapper)
    {
      ArgumentUtility.CheckNotNull ("wrapper", wrapper);
      _wrapper = wrapper;
    }

    void IDeserializationCallback.OnDeserialization (object sender)
    {
      Assertion.DebugAssert (!Utilities.ReflectionUtility.CanAscribe (typeof (BindableDomainObjectImplementation), typeof (Mixin<,>)),
          "we assume the mixin does not have a base object");
      MixinTargetMockUtility.SignalOnDeserialization (this, _wrapper);
    }
  }
}