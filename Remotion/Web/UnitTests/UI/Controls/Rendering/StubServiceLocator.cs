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
using Remotion.Web.UI.Controls.Rendering.WebTabStrip;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.Rendering
{
  public class StubServiceLocator : ServiceLocatorImplBase
  {
    private readonly IDataStore<Type, object> _instances = new SimpleDataStore<Type, object>();

    public StubServiceLocator ()
    {
      var rendererStub = MockRepository.GenerateStub<IWebTabStripRenderer>();
      var tabRendererStub = MockRepository.GenerateStub<IWebTabRenderer>();

      var stubFactory = MockRepository.GenerateStub<IWebTabStripRendererFactory>();
      stubFactory.Stub (stub => stub.CreateRenderer (null, null, null)).IgnoreArguments().Return (rendererStub);
      stubFactory.Stub (stub => stub.CreateTabRenderer (null, null, null)).IgnoreArguments ().Return (tabRendererStub);
      _instances.Add (typeof (IWebTabStripRendererFactory), stubFactory);
    }

    public void SetFactory<T> (T factory)
    {
      _instances[typeof (T)] = factory;
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