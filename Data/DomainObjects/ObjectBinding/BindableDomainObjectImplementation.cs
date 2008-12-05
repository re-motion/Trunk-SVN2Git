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
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  [Serializable]
  public class BindableDomainObjectImplementation : BindableDomainObjectMixin, IDeserializationCallback, IBindableDomainObjectImplementation
  {
    public static BindableDomainObjectImplementation Create (BindableDomainObject wrapper)
    {
      ArgumentUtility.CheckNotNull ("wrapper", wrapper);
      Assertion.DebugAssert (!Utilities.ReflectionUtility.CanAscribe (typeof (BindableDomainObjectImplementation), typeof (Mixin<,>)),
                             "we assume the mixin does not have a base object");
      var impl = new BindableDomainObjectImplementation (wrapper);
      ((IInitializableMixin) impl).Initialize (wrapper, null, false);
      return impl;
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

    public string BaseDisplayName
    {
      get { return base.DisplayName; }
    }

    public string BaseUniqueIdentifier
    {
      get { return base.UniqueIdentifier; }
    }

    public override string DisplayName
    {
      get { return ((IBusinessObject) This).DisplayName; }
    }
  }
}
