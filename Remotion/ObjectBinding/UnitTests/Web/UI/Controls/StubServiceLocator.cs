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
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Globalization;
using Remotion.Globalization.Implementation;
using Remotion.Mixins.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Factories;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation.Rendering;
using Remotion.Web.UI.Controls.DropDownMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.ListMenuImplementation.Rendering;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{
  public class StubServiceLocator : ServiceLocatorImplBase
  {
    private readonly IDataStore<Type, object> _instances = new SimpleDataStore<Type, object>();
    private readonly IServiceLocator _defaultServiceLocator = new DefaultServiceLocator();

    public StubServiceLocator ()
    {
      var bocListCssClassDefinition = new BocListCssClassDefinition();
      var compoundGlobalizationService =
          new CompoundGlobalizationService (
              new IGlobalizationService[]
              {
                  new GlobalizationService (new ResourceManagerResolver<MultiLingualResourcesAttribute>())
              });

      _instances.Add (
          typeof (IBocListRenderer),
          new BocListRenderer (
              new FakeResourceUrlFactory(),
              compoundGlobalizationService,
              bocListCssClassDefinition,
              new BocListTableBlockRenderer (
                  bocListCssClassDefinition,
                  new BocRowRenderer (
                      bocListCssClassDefinition,
                      new BocIndexColumnRenderer (bocListCssClassDefinition),
                      new BocSelectorColumnRenderer (bocListCssClassDefinition))),
              new BocListNavigationBlockRenderer (
                  new FakeResourceUrlFactory(),
                  compoundGlobalizationService,
                  bocListCssClassDefinition),
              new BocListMenuBlockRenderer (bocListCssClassDefinition)));

      _instances.Add (
          typeof (IBocSimpleColumnRenderer),
          new BocSimpleColumnRenderer (new FakeResourceUrlFactory(), bocListCssClassDefinition));
      _instances.Add (
          typeof (IBocCompoundColumnRenderer),
          new BocCompoundColumnRenderer (new FakeResourceUrlFactory(), bocListCssClassDefinition));
      _instances.Add (
          typeof (IBocCommandColumnRenderer),
          new BocCommandColumnRenderer (new FakeResourceUrlFactory(), bocListCssClassDefinition));
      _instances.Add (
          typeof (IBocCustomColumnRenderer),
          new BocCustomColumnRenderer (new FakeResourceUrlFactory(), bocListCssClassDefinition));
      _instances.Add (
          typeof (IBocRowEditModeColumnRenderer),
          new BocRowEditModeColumnRenderer (new FakeResourceUrlFactory(), bocListCssClassDefinition));
      _instances.Add (
          typeof (IBocDropDownMenuColumnRenderer),
          new BocDropDownMenuColumnRenderer (new FakeResourceUrlFactory(), bocListCssClassDefinition));
      _instances.Add (typeof (IBocIndexColumnRenderer), new BocIndexColumnRenderer (bocListCssClassDefinition));
      _instances.Add (typeof (IBocSelectorColumnRenderer), new BocSelectorColumnRenderer (bocListCssClassDefinition));

      _instances.Add (typeof (IDropDownMenuRenderer), new DropDownMenuRenderer (new FakeResourceUrlFactory(), compoundGlobalizationService));
      _instances.Add (typeof (IListMenuRenderer), new ListMenuRenderer (new FakeResourceUrlFactory(), compoundGlobalizationService));
      _instances.Add (
          typeof (IDatePickerButtonRenderer),
          new DatePickerButtonRenderer (new FakeResourceUrlFactory(), compoundGlobalizationService));
      _instances.Add (
          typeof (IBocReferenceValueRenderer),
          new BocReferenceValueRenderer (new FakeResourceUrlFactory(), compoundGlobalizationService));
      _instances.Add (
          typeof (IBocDateTimeValueRenderer),
          new BocDateTimeValueRenderer (new FakeResourceUrlFactory(), compoundGlobalizationService));
      _instances.Add (
          typeof (IBocMultilineTextValueRenderer),
          new BocMultilineTextValueRenderer (new FakeResourceUrlFactory(), compoundGlobalizationService));
      _instances.Add (typeof (IBocTextValueRenderer), new BocTextValueRenderer (new FakeResourceUrlFactory(), compoundGlobalizationService));
      _instances.Add (
          typeof (IBocBooleanValueRenderer),
          new BocBooleanValueRenderer (
              new FakeResourceUrlFactory(),
              compoundGlobalizationService,
              new BocBooleanValueResourceSetFactory (new FakeResourceUrlFactory())));
      _instances.Add (
          typeof (IBocBooleanValueResourceSetFactory),
          new BocBooleanValueResourceSetFactory (new FakeResourceUrlFactory()));
      _instances.Add (typeof (IBocCheckBoxRenderer), new BocCheckBoxRenderer (new FakeResourceUrlFactory(), compoundGlobalizationService));
      _instances.Add (typeof (IBocEnumValueRenderer), new BocEnumValueRenderer (new FakeResourceUrlFactory(), compoundGlobalizationService));

      _instances.Add (typeof (IClientScriptBehavior), new ClientScriptBehavior());
      _instances.Add (typeof (IInfrastructureResourceUrlFactory), new StubInfrastructureResourceUrlFactory());
      _instances.Add (typeof (IResourceUrlFactory), new FakeResourceUrlFactory());
    }

    public void SetFactory<T> (T factory)
    {
      _instances[typeof (T)] = factory;
    }

    protected override object DoGetInstance (Type serviceType, string key)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);

      if (IsCoreType (serviceType))
        return _defaultServiceLocator.GetInstance (serviceType, key);

      return _instances.GetOrCreateValue (
          serviceType,
          delegate (Type type) { throw new ArgumentException (string.Format ("No service for type '{0}' registered.", type)); });
    }

    protected override IEnumerable<object> DoGetAllInstances (Type serviceType)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);

      if (IsCoreType (serviceType))
        return _defaultServiceLocator.GetAllInstances (serviceType);

      object serviceInstance;
      if (_instances.TryGetValue (serviceType, out serviceInstance))
        return new[] { serviceInstance };
      return new object[0];
    }

    private bool IsCoreType (Type serviceType)
    {
      return serviceType.Assembly != typeof (BocList).Assembly;
    }
  }
}