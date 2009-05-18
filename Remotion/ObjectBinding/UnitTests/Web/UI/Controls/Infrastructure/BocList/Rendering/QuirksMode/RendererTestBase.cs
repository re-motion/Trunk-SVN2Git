// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System.Web.UI;
using NUnit.Framework;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.Web;
using Remotion.ObjectBinding.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  public abstract class RendererTestBase
  {
    protected HtmlHelper Html { get; set; }
    protected IBocList List { get; set; }
    protected IBusinessObject BusinessObject { get; set; }
    protected BocListDataRowRenderEventArgs EventArgs { get; set; }

    [SetUp]
    public virtual void SetUp ()
    {
      TypeWithReference businessObject = TypeWithReference.Create (
          TypeWithReference.Create ("referencedObject1"),
          TypeWithReference.Create ("referencedObject2"));
      businessObject.ReferenceList = new[] { businessObject.FirstValue, businessObject.SecondValue };
      BusinessObject = (IBusinessObject) businessObject;
      BusinessObject.BusinessObjectClass.BusinessObjectProvider.AddService<IBusinessObjectWebUIService> (new ReflectionBusinessObjectWebUIService());

      EventArgs = new BocListDataRowRenderEventArgs (0, (IBusinessObject) businessObject.FirstValue);
      EventArgs.IsEditableRow = false;

      Html = new HtmlHelper();
      Html.InitializeStream();
    }

    protected void InitializeMockList ()
    {
      List = MockRepository.GenerateMock<IBocList>();

      List.Stub (mock => mock.DataSource).Return (MockRepository.GenerateStub<IBusinessObjectDataSource>());
      List.DataSource.BusinessObject = BusinessObject;
      List.Property = BusinessObject.BusinessObjectClass.GetPropertyDefinition ("ReferenceList");

      List.Stub (mock => mock.Value).Return (((TypeWithReference) BusinessObject).ReferenceList);

      // List.Stub (mock => mock.FixedColumns).Return (new BocColumnDefinitionCollection(List));
      
    }

    protected void InitializeBocList ()
    {
      Page page = new Page();
      List = new BocListMock();
      page.Controls.Add (((BocListMock) List));

      List.DataSource = MockRepository.GenerateStub<IBusinessObjectDataSource> ();
      List.DataSource.BusinessObject = BusinessObject;
      List.Property = BusinessObject.BusinessObjectClass.GetPropertyDefinition ("ReferenceList");

      ((BocListMock) List).Value = ((TypeWithReference) BusinessObject).ReferenceList;
    }
  }
}