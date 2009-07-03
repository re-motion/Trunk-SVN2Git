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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.Web;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering.ListMenu;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList.QuirksMode
{
  public abstract class BocListRendererTestBase : RendererTestBase
  {
    protected IBocList List { get; set; }
    protected IBusinessObject BusinessObject { get; set; }
    protected BocListDataRowRenderEventArgs EventArgs { get; set; }

    protected override void Initialize ()
    {
      Initialize (true);
    }

    protected void Initialize (bool withRowObjects)
    {
      base.Initialize ();

      TypeWithReference businessObject;
      if (withRowObjects)
      {
        businessObject = TypeWithReference.Create (
            TypeWithReference.Create ("referencedObject1"),
            TypeWithReference.Create ("referencedObject2"));
        businessObject.ReferenceList = new[] { businessObject.FirstValue, businessObject.SecondValue };
        
      }
      else
      {
        businessObject = TypeWithReference.Create();
        businessObject.ReferenceList = new TypeWithReference[0];
      }
      BusinessObject = (IBusinessObject) businessObject;
      BusinessObject.BusinessObjectClass.BusinessObjectProvider.AddService<IBusinessObjectWebUIService>
        (new ReflectionBusinessObjectWebUIService ());

      EventArgs = new BocListDataRowRenderEventArgs (0, (IBusinessObject) businessObject.FirstValue);
      EventArgs.IsEditableRow = false;

      InitializeMockList();
    }

    private void InitializeMockList ()
    {
      List = MockRepository.GenerateMock<IBocList>();

      List.Stub (list => list.HasClientScript).Return (true);

      List.Stub (list => list.DataSource).Return (MockRepository.GenerateStub<IBusinessObjectDataSource>());
      List.DataSource.BusinessObject = BusinessObject;
      List.Stub (list => list.Property).Return (BusinessObject.BusinessObjectClass.GetPropertyDefinition ("ReferenceList"));

      List.Stub (list => list.Value).Return (((TypeWithReference) BusinessObject).ReferenceList);

      List.Stub (list => list.FixedColumns).Return (new BocColumnDefinitionCollection (List));
      List.Stub (list => list.IsColumnVisible (null)).IgnoreArguments().Return (true);

      List.Stub (list => list.SelectorControlCheckedState).Return (new List<int> ());

      var listMenuStub = MockRepository.GenerateStub<IListMenu>();
      List.Stub (list => list.ListMenu).Return (listMenuStub);

      StateBag stateBag = new StateBag ();
      List.Stub (mock => mock.Attributes).Return (new AttributeCollection (stateBag));
      List.Stub (mock => mock.Style).Return (List.Attributes.CssStyle);
      List.Stub (mock => mock.ControlStyle).Return (new Style (stateBag));

      var page = MockRepository.GenerateMock<IPage>();
      List.Stub (list => list.Page).Return (page);

      var clientScriptManager = MockRepository.GenerateMock<IClientScriptManager>();
      page.Stub (pageMock => pageMock.ClientScript).Return (clientScriptManager);

      clientScriptManager.Stub (scriptManagerMock => scriptManagerMock.GetPostBackEventReference (null, ""))
          .IgnoreArguments().Return ("postBackEventReference");

      var editModeController = MockRepository.GenerateMock<IEditModeController>();
      List.Stub (list => list.EditModeController).Return (editModeController);

      List.Stub (list => list.GetResourceManager()).Return (
          MultiLingualResources.GetResourceManager (typeof (ObjectBinding.Web.UI.Controls.BocList.ResourceIdentifier)));
    }
  }
}