// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
  public class BindableObjectBaseImplementation : BindableObjectMixin, IDeserializationCallback, IBindableObjectBaseImplementation
  {
    public static BindableObjectBaseImplementation Create (BindableObjectBase wrapper)
    {
      ArgumentUtility.CheckNotNull ("wrapper", wrapper);
      Assertion.DebugAssert (!ReflectionUtility.CanAscribe (typeof (BindableObjectBaseImplementation), typeof (Mixin<,>)),
          "we assume the mixin does not have a base object");
      var impl = new BindableObjectBaseImplementation (wrapper);
      ((IInitializableMixin) impl).Initialize (wrapper, null, false);
      return impl;
    }

    private readonly BindableObjectBase _wrapper;

    protected BindableObjectBaseImplementation (BindableObjectBase wrapper)
    {
      ArgumentUtility.CheckNotNull ("wrapper", wrapper);
      _wrapper = wrapper;
    }

    void IDeserializationCallback.OnDeserialization(object sender)
    {
      Assertion.DebugAssert (!ReflectionUtility.CanAscribe (typeof (BindableObjectMixin), typeof (Mixin<,>)),
          "we assume the mixin does not have a base object");
      ((IInitializableMixin) this).Initialize (_wrapper, null, true);
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
