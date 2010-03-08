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
using Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Factories;
using Remotion.Web;
using Remotion.Web.UI.Controls.Rendering.DatePickerButton;
using Remotion.Web.UI.Controls.Rendering.DatePickerButton.QuirksMode.Factories;
using Remotion.Web.UI.Controls.Rendering.DropDownMenu;
using Remotion.Web.UI.Controls.Rendering.DropDownMenu.QuirksMode.Factories;
using Remotion.Web.UI.Controls.Rendering.ListMenu;
using Remotion.Web.UI.Controls.Rendering.ListMenu.StandardMode.Factories;
using BocDateTimeValueRendererFactory=
    Remotion.ObjectBinding.Web.Legacy.UI.Controls.Factories.BocDateTimeValueRendererFactory;
using BocReferenceValueRendererFactory=
    Remotion.ObjectBinding.Web.Legacy.UI.Controls.Factories.BocReferenceValueRendererFactory;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering
{
  public class StubServiceLocator : ServiceLocatorImplBase
  {
    private readonly IDataStore<Type, object> _instances = new SimpleDataStore<Type, object>();

    public StubServiceLocator ()
    {
      _instances.Add (typeof (IBocListRendererFactory), new BocListRendererFactory());

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
      _instances.Add (typeof (IBocMultilineTextValueRendererFactory), new BocMultilineTextValueRendererFactory());
      _instances.Add (typeof (IBocTextValueRendererFactory), new BocTextValueRendererFactory ());
      _instances.Add (typeof (IBocBooleanValueRendererFactory), new BocBooleanValueRendererFactory ());
      _instances.Add (typeof (IBocCheckboxRendererFactory), new BocCheckboxRendererFactory ());
      _instances.Add (typeof (IBocEnumValueRendererFactory), new BocEnumValueRendererFactory ());

      _instances.Add (typeof (ResourceTheme), ResourceTheme.ClassicBlue);
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
