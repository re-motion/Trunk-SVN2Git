// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.ObjectBinding;
using Remotion.Security;

namespace Remotion.Data.UnitTests.DomainObjects.Security.TestDomain
{
  [BindableObject]
  public class BindableSecurableObjectMixin : DomainObjectMixin<DomainObject>, IBindableSecurableObjectMixin
  {
    public string MixedPropertyWithDefaultPermission
    {
      get { return Properties[typeof (BindableSecurableObjectMixin), "MixedPropertyWithDefaultPermission"].GetValue<string> (); }
      set { Properties[typeof (BindableSecurableObjectMixin), "MixedPropertyWithDefaultPermission"].SetValue (value); }
    }

    public string MixedPropertyWithReadPermission
    {
      [DemandPermission(TestAccessTypes.First)]
      get { return Properties[typeof (BindableSecurableObjectMixin), "MixedPropertyWithReadPermission"].GetValue<string> (); }
      set { Properties[typeof (BindableSecurableObjectMixin), "MixedPropertyWithReadPermission"].SetValue (value); }
    }

    public string MixedPropertyWithWritePermission
    {
      get { return Properties[typeof (BindableSecurableObjectMixin), "MixedPropertyWithWritePermission"].GetValue<string> (); }
      [DemandPermission (TestAccessTypes.First)]
      set { Properties[typeof (BindableSecurableObjectMixin), "MixedPropertyWithWritePermission"].SetValue (value); }
    }

    public string DefaultPermissionMixedProperty { get; set; }

    public string CustomPermissionMixedProperty
    {
      [DemandPermission (TestAccessTypes.First)]
      get;
      [DemandPermission (TestAccessTypes.Second)]
      set;
    }
  }
}