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
using Remotion.BridgeImplementations;
using Remotion.BridgeInterfaces;
using Remotion.Collections;
using Remotion.Context;
using Remotion.Logging.BridgeImplementations;
using Remotion.Logging.BridgeInterfaces;
using Remotion.Mixins.BridgeImplementations;
using Remotion.Mixins.BridgeInterfaces;
using Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Factories;
using Remotion.Reflection;
using Remotion.Web;
using Remotion.Web.Factories;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation.Rendering;
using Remotion.Web.UI.Controls.DropDownMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.Factories;
using Remotion.Web.UI.Controls.ListMenuImplementation.Rendering;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{
  public class StubServiceLocator : ServiceLocatorImplBase
  {
    private readonly IDataStore<Type, object> _instances = new SimpleDataStore<Type, object>();

    public StubServiceLocator ()
    {
      var bocListCssClassDefinition = new BocListCssClassDefinition();

      _instances.Add (typeof (IBocListRendererFactory), new BocListRendererFactory (bocListCssClassDefinition));

      _instances.Add (typeof (IBocSimpleColumnRendererFactory), new BocSimpleColumnRendererFactory (bocListCssClassDefinition));
      _instances.Add (typeof (IBocCompoundColumnRendererFactory), new BocCompoundColumnRendererFactory (bocListCssClassDefinition));
      _instances.Add (typeof (IBocCommandColumnRendererFactory), new BocCommandColumnRendererFactory (bocListCssClassDefinition));
      _instances.Add (typeof (IBocCustomColumnRendererFactory), new BocCustomColumnRendererFactory (bocListCssClassDefinition));
      _instances.Add (typeof (IBocRowEditModeColumnRendererFactory), new BocRowEditModeColumnRendererFactory (bocListCssClassDefinition));
      _instances.Add (typeof (IBocDropDownMenuColumnRendererFactory), new BocDropDownMenuColumnRendererFactory (bocListCssClassDefinition));
      _instances.Add (typeof (IBocIndexColumnRendererFactory), new BocIndexColumnRendererFactory (bocListCssClassDefinition));
      _instances.Add (typeof (IBocSelectorColumnRendererFactory), new BocSelectorColumnRendererFactory (bocListCssClassDefinition));

      _instances.Add (typeof (IBocStubColumnRendererFactory), new StubColumnRendererFactory());

      _instances.Add (typeof (IDropDownMenuRendererFactory), new DropDownMenuRendererFactory());
      _instances.Add (typeof (IListMenuRendererFactory), new ListMenuRendererFactory ());
      _instances.Add (typeof (IDatePickerButtonRendererFactory), new DatePickerButtonRendererFactory ());
      _instances.Add (typeof (IBocReferenceValueRenderer), new BocReferenceValueRenderer (new ResourceUrlFactory (new ResourceTheme.ClassicBlue ())));
      _instances.Add (typeof (IBocDateTimeValueRenderer), new BocDateTimeValueRenderer (new ResourceUrlFactory (new ResourceTheme.ClassicBlue ())));
      _instances.Add (typeof (IBocMultilineTextValueRenderer), new BocMultilineTextValueRenderer (new ResourceUrlFactory (new ResourceTheme.ClassicBlue())));
      _instances.Add (typeof (IBocTextValueRenderer), new BocTextValueRenderer (new ResourceUrlFactory(new ResourceTheme.ClassicBlue())));
      _instances.Add (typeof (IBocBooleanValueRendererFactory), new BocBooleanValueRendererFactory ());
      _instances.Add (typeof (IBocBooleanValueResourceSetFactory), new BocBooleanValueResourceSetFactory ());
      _instances.Add (typeof (IBocCheckboxRendererFactory), new BocCheckboxRendererFactory ());
      _instances.Add (typeof (IBocEnumValueRenderer), new BocEnumValueRenderer (new ResourceUrlFactory (new ResourceTheme.ClassicBlue ())));

      _instances.Add (typeof (IClientScriptBehaviorFactory), new ClientScriptBehaviorFactory());
      _instances.Add (typeof (IThemedResourceUrlResolverFactory), new StubResourceUrlResolverFactory());
      _instances.Add (typeof (IResourceUrlFactory), new ResourceUrlFactory (new ResourceTheme.ClassicBlue()));

      _instances.Add (typeof (IParamListCreateImplementation), new ParamListCreateImplementation());
      _instances.Add (typeof (IObjectFactoryImplementation), new ObjectFactoryImplementation());
      _instances.Add (typeof (ITypeFactoryImplementation), new TypeFactoryImplementation());
      _instances.Add (typeof (ILogManagerImplementation), new LogManagerImplementation());
      _instances.Add (typeof (IBootstrapStorageProvider), new BootstrapStorageProvider());
      _instances.Add (typeof (ITypeDiscoveryServiceFactoryImplementation), new TypeDiscoveryServiceFactoryImplementation());
      _instances.Add (typeof (IMixinTypeUtilityImplementation), new MixinTypeUtilityImplementation());
      _instances.Add (typeof (IAdapterRegistryImplementation), new AdapterRegistryImplementation());
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