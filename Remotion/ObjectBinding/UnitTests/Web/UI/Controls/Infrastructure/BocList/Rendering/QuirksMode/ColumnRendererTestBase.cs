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
using System.Web.UI;
using NUnit.Framework;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.Web;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  public abstract class ColumnRendererTestBase<T>
      where T: BocColumnDefinition
  {
    protected HtmlHelper Html { get; set; }
    private Page Page { get; set; }
    protected BocListMock List { get; set; }
    protected T Column { get; set; }
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

      Page = new Page();
      List = new BocListMock();
      Page.Controls.Add (List);

      List.DataSource = new BusinessObjectReferenceDataSource();
      List.DataSource.BusinessObject = BusinessObject;
      List.Property =
          (IBusinessObjectReferenceProperty) BusinessObject.BusinessObjectClass.GetPropertyDefinition ("ReferenceList");
      List.Value = ((TypeWithReference) BusinessObject).ReferenceList;

      EventArgs = new BocListDataRowRenderEventArgs (0, (IBusinessObject) businessObject.FirstValue);
      EventArgs.IsEditableRow = false;

      List.FixedColumns.Add (Column);

      Html = new HtmlHelper();
      Html.InitializeStream();
    }
  }
}