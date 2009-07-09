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
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Remotion.Collections;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocReferenceValue;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocReferenceValue.QuirksMode.Factories;
using Remotion.Web.UI.Controls.Rendering.DropDownMenu;
using Remotion.Web.UI.Controls.Rendering.DropDownMenu.QuirksMode.Factories;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocReferenceValue
{
  public class StubServiceLocator : ServiceLocatorImplBase
  {
    private readonly IDataStore<Type, object> _instances = new SimpleDataStore<Type, object>();

    public StubServiceLocator ()
    {
      _instances.Add (typeof (IBocReferenceValueRendererFactory), new BocReferenceValueRendererFactory());
      _instances.Add (typeof (IDropDownMenuRendererFactory), new DropDownMenuRendererFactory());
    }

    protected override object DoGetInstance (Type serviceType, string key)
    {
      return _instances[serviceType];
    }

    protected override IEnumerable<object> DoGetAllInstances (Type serviceType)
    {
      return new[]{_instances[serviceType]};
    }
  }
}