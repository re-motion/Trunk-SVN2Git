// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Runtime.Serialization;
using Remotion.Mixins;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  [Serializable]
  public class BindableObjectWithIdentityBaseImplementation : BindableObjectWithIdentityMixin, IDeserializationCallback, IBindableObjectBaseImplementation
  {
    public static BindableObjectWithIdentityBaseImplementation Create (BindableObjectWithIdentityBase wrapper)
    {
      ArgumentUtility.CheckNotNull ("wrapper", wrapper);
      Assertion.DebugAssert (!ReflectionUtility.CanAscribe (typeof (BindableObjectWithIdentityBaseImplementation), typeof (Mixin<,>)),
          "we assume the mixin does not have a base object");
      var impl = new BindableObjectWithIdentityBaseImplementation (wrapper);
      ((IInitializableMixin) impl).Initialize (wrapper, null, false);
      return impl;
    }

    private readonly BindableObjectWithIdentityBase _wrapper;

    protected BindableObjectWithIdentityBaseImplementation (BindableObjectWithIdentityBase wrapper)
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

    public string BaseDisplayName
    {
      get { return base.DisplayName; }
    }

    public override string DisplayName
    {
      get { return ((IBusinessObject) This).DisplayName; }
    }
  }
}
