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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocDateTimeValue;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocDateTimeValue.QuirksMode.Factories;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode.Factories;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocReferenceValue;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocReferenceValue.QuirksMode.Factories;
using Remotion.Web.UI.Controls.Rendering.DatePickerButton;
using Remotion.Web.UI.Controls.Rendering.DatePickerButton.QuirksMode.Factories;
using Remotion.Web.UI.Controls.Rendering.DropDownMenu;
using Remotion.Web.UI.Controls.Rendering.DropDownMenu.QuirksMode.Factories;
using Remotion.Web.UI.Controls.Rendering.ListMenu;
using Remotion.Web.UI.Controls.Rendering.ListMenu.StandardMode.Factories;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList
{
  public class StubServiceLocator : ServiceLocatorImplBase
  {
    private readonly IDataStore<Type, object> _instances = new SimpleDataStore<Type, object>();

    public StubServiceLocator ()
    {
      _instances.Add (typeof (IBocListRendererFactory), new BocListRendererFactory());
      _instances.Add (typeof (IBocListMenuBlockRendererFactory), new BocListRendererFactory());
      _instances.Add (typeof (IBocListNavigationBlockRendererFactory), new BocListRendererFactory());
      _instances.Add (typeof (IBocListTableBlockRendererFactory), new BocListRendererFactory());
      _instances.Add (typeof (IBocRowRendererFactory), new BocListRendererFactory());

      _instances.Add (typeof (IBocColumnRendererFactory<BocSimpleColumnDefinition>), new BocColumnRendererFactory());
      _instances.Add (typeof (IBocColumnRendererFactory<BocCompoundColumnDefinition>), new BocColumnRendererFactory());
      _instances.Add (typeof (IBocColumnRendererFactory<BocCommandColumnDefinition>), new BocColumnRendererFactory());
      _instances.Add (typeof (IBocColumnRendererFactory<BocCustomColumnDefinition>), new BocColumnRendererFactory());
      _instances.Add (typeof (IBocColumnRendererFactory<BocRowEditModeColumnDefinition>), new BocColumnRendererFactory());
      _instances.Add (typeof (IBocColumnRendererFactory<BocDropDownMenuColumnDefinition>), new BocColumnRendererFactory());
      _instances.Add (typeof (IBocIndexColumnRendererFactory), new BocColumnRendererFactory());
      _instances.Add (typeof (IBocSelectorColumnRendererFactory), new BocColumnRendererFactory());

      _instances.Add (typeof (IBocColumnRendererFactory<StubColumnDefinition>), new StubColumnRendererFactory());

      _instances.Add (typeof (IDropDownMenuRendererFactory), new DropDownMenuRendererFactory());
      _instances.Add (typeof (IListMenuRendererFactory), new ListMenuRendererFactory ());
      _instances.Add (typeof (IDatePickerButtonRendererFactory), new DatePickerButtonRendererFactory ()); 
      _instances.Add (typeof (IBocReferenceValueRendererFactory), new BocReferenceValueRendererFactory ());
      _instances.Add (typeof (IBocDateTimeValueRendererFactory), new BocDateTimeValueRendererFactory());
      
    }

    public void SetRowRendererFactory (IBocRowRendererFactory factory)
    {
      SetFactory (factory);
    }

    public void SetTableBlockRendererFactory (IBocListTableBlockRendererFactory factory)
    {
      SetFactory (factory);
    }

    public void SetMenuBlockRendererFactory (IBocListMenuBlockRendererFactory factory)
    {
      SetFactory (factory);
    }

    public void SetNavigationBlockRendererFactory (IBocListTableBlockRendererFactory factory)
    {
      SetFactory (factory);
    }

    private void SetFactory<T> (T factory)
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