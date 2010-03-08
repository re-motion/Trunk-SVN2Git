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
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Remotion.Collections;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Factories;

namespace OBWTest
{
  public class StubServiceLocator : ServiceLocatorImplBase
  {
    private readonly IDataStore<Type, object> _instances = new SimpleDataStore<Type, object>();

    public StubServiceLocator ()
    {
      _instances.Add (typeof (IBocListRendererFactory), new BocListRendererFactory ());

      _instances.Add (typeof (IBocColumnRendererFactory<BocSimpleColumnDefinition>), new BocColumnRendererFactory());
      _instances.Add (typeof (IBocColumnRendererFactory<BocCompoundColumnDefinition>), new BocColumnRendererFactory());
      _instances.Add (typeof (IBocColumnRendererFactory<BocCommandColumnDefinition>), new BocColumnRendererFactory());
      _instances.Add (typeof (IBocColumnRendererFactory<BocCustomColumnDefinition>), new BocColumnRendererFactory());
      _instances.Add (typeof (IBocColumnRendererFactory<BocRowEditModeColumnDefinition>), new BocColumnRendererFactory());
      _instances.Add (typeof (IBocColumnRendererFactory<BocDropDownMenuColumnDefinition>), new BocColumnRendererFactory());
      _instances.Add (typeof (IBocIndexColumnRendererFactory), new BocColumnRendererFactory ());
      _instances.Add (typeof (IBocSelectorColumnRendererFactory), new BocColumnRendererFactory ());
    }

    protected override object DoGetInstance (Type serviceType, string key)
    {
      return _instances.GetOrCreateValue (
          serviceType, delegate (Type type) { throw new ArgumentException (string.Format ("No service for type '{0}' registered.", type)); });
    }

    protected override IEnumerable<object> DoGetAllInstances (Type serviceType)
    {
      throw new NotSupportedException();
    }
  }
}
