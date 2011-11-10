// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain
{
  [Instantiable]
  [DBTable]
  public abstract class SampleBindableDomainObject_AccessingImplementationFromCtor : BindableDomainObject
  {
    public static SampleBindableDomainObject_AccessingImplementationFromCtor NewObject ()
    {
      return NewObject<SampleBindableDomainObject_AccessingImplementationFromCtor> ();
    }

    public static SampleBindableDomainObject_AccessingImplementationFromCtor NewObject (IBindableDomainObjectImplementation implementation)
    {
      return NewObject<SampleBindableDomainObject_AccessingImplementationFromCtor> (ParamList.Create (implementation));
    }

    public static SampleBindableDomainObject_AccessingImplementationFromCtor GetObject (ObjectID id)
    {
      return GetObject<SampleBindableDomainObject_AccessingImplementationFromCtor> (id);
    }

    public readonly string DisplayNameFromCtor;

    protected SampleBindableDomainObject_AccessingImplementationFromCtor()
    {
      DisplayNameFromCtor = base.DisplayName;
    }

    public abstract string Name { get; set; }
    public abstract int Int32 { get; set; }
  }
}
