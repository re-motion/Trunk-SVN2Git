// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Web;
using Remotion.Web.Factories;

namespace OBWTest
{
  public class StubServiceLocator : ServiceLocatorImplBase
  {
    private readonly IDataStore<Type, object> _instances = new SimpleDataStore<Type, object>();

    public StubServiceLocator ()
    {
      var bocListCssClassDefinition = new BocListCssClassDefinition();

      _instances.Add (
          typeof (IBocListRenderer),
          new BocListRenderer (
              new ResourceUrlFactory (new ResourceTheme.ClassicBlue()),
              bocListCssClassDefinition,
              new BocListTableBlockRenderer (
                  new BocListCssClassDefinition(),
                  new BocRowRenderer (
                      new BocListCssClassDefinition(),
                      new BocIndexColumnRenderer (new BocListCssClassDefinition()),
                      new BocSelectorColumnRenderer (new BocListCssClassDefinition()))),
              new BocListNavigationBlockRenderer (new ResourceUrlFactory (new ResourceTheme.ClassicBlue()), new BocListCssClassDefinition()),
              new BocListMenuBlockRenderer (new BocListCssClassDefinition())));

      _instances.Add (
          typeof (IBocSimpleColumnRenderer),
          new BocSimpleColumnRenderer (new ResourceUrlFactory (new ResourceTheme.ClassicBlue()), bocListCssClassDefinition));
      _instances.Add (
          typeof (IBocCompoundColumnRenderer),
          new BocCompoundColumnRenderer (new ResourceUrlFactory (new ResourceTheme.ClassicBlue()), bocListCssClassDefinition));
      _instances.Add (
          typeof (IBocCommandColumnRenderer),
          new BocCommandColumnRenderer (new ResourceUrlFactory (new ResourceTheme.ClassicBlue()), bocListCssClassDefinition));
      _instances.Add (
          typeof (IBocCustomColumnRenderer),
          new BocCustomColumnRenderer (new ResourceUrlFactory (new ResourceTheme.ClassicBlue()), bocListCssClassDefinition));
      _instances.Add (
          typeof (IBocRowEditModeColumnRenderer),
          new BocRowEditModeColumnRenderer (new ResourceUrlFactory (new ResourceTheme.ClassicBlue()), bocListCssClassDefinition));
      _instances.Add (
          typeof (IBocDropDownMenuColumnRenderer),
          new BocDropDownMenuColumnRenderer (new ResourceUrlFactory (new ResourceTheme.ClassicBlue()), bocListCssClassDefinition));
      _instances.Add (typeof (IBocIndexColumnRenderer), new BocIndexColumnRenderer (bocListCssClassDefinition));
      _instances.Add (typeof (IBocSelectorColumnRenderer), new BocSelectorColumnRenderer (bocListCssClassDefinition));
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