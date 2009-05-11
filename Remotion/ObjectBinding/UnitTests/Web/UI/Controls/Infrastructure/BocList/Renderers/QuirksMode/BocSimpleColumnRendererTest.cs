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
using System.Diagnostics;
using System.Web.UI;
using HtmlAgilityPack;
using NUnit.Framework;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Renderers;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Renderers.QuirksMode
{
  [TestFixture]
  public class BocSimpleColumnRendererTest
  {
    private HtmlHelper Html { get; set; }
    private Page Page { get; set; }
    private ObjectBinding.Web.UI.Controls.BocList List { get; set; }
    private BocSimpleColumnDefinition Column { get; set; }
    private IBusinessObject BusinessObject { get; set; }
    private BocListDataRowRenderEventArgs EventArgs { get; set; }

    [SetUp]
    public void SetUp ()
    {
      BusinessObject = (IBusinessObject) TypeWithReference.Create ("TestObject");

      Page = new Page();
      List = new ObjectBinding.Web.UI.Controls.BocList();
      Page.Controls.Add (List);

      List.DataSource = new BusinessObjectReferenceDataSource();
      List.DataSource.BusinessObject = BusinessObject;
      List.Property =
          (IBusinessObjectReferenceProperty) BusinessObject.BusinessObjectClass.GetPropertyDefinition ("ReferenceList");
      //List.DataSource.BusinessObjectClass = new BindableObjectClass (BusinessObject.GetType(), new BindableObjectProvider());

      EventArgs = new BocListDataRowRenderEventArgs (0, BusinessObject);
      EventArgs.IsEditableRow = false;

      Column = new BocSimpleColumnDefinition();
      Column.IsDynamic = false;
      Column.IsReadOnly = false;
      Column.ColumnTitle = "FirstColumn";
      Column.PropertyPathIdentifier = "FirstValue";

      List.FixedColumns.Add (Column);

      Html = new HtmlHelper();
      Html.InitializeStream();
    }

    [Test]
    public void TestPerformanceGetType ()
    {
      Stopwatch stopwatch = new Stopwatch();

      stopwatch.Start();
      for (int i = 0; i < 10000; i++)
        Column.GetType();
      stopwatch.Stop();
      Assert.Fail ("Time elapsed: " + stopwatch.ElapsedMilliseconds + " ms");
    }

    [Test]
    public void TestPerformanceTypeOf ()
    {
      Stopwatch stopwatch = new Stopwatch();

      stopwatch.Start();
      for (int i = 0; i < 10000; i++)
      {
        Type type = typeof (IBocColumnRenderer<>);
      }
      stopwatch.Stop();
      Assert.Fail ("Time elapsed: " + stopwatch.ElapsedMilliseconds + " ms");
    }

    [Test]
    public void TestPerformanceMakeGenericType ()
    {
      Stopwatch stopwatch = new Stopwatch();
      Type typeParameter = Column.GetType();
      Type genericType = typeof (IBocColumnRenderer<>);
      stopwatch.Start();
      for (int i = 0; i < 10000; i++)
        genericType.MakeGenericType (typeParameter);
      stopwatch.Stop();
      Assert.Fail ("Time elapsed: " + stopwatch.ElapsedMilliseconds + " ms");
    }

    [Test]
    public void RenderEmptyCell ()
    {
      IBocColumnRenderer renderer = new BocSimpleColumnRenderer (List, Html.Writer, Column);

      renderer.RenderDataCell (0, false, "bocListTableCell", EventArgs);
      HtmlDocument document = Html.GetResultDocument();
    }

    [Test]
    public void RenderCell ()
    {
      BusinessObject.SetProperty ("FirstValue", TypeWithReference.Create ("referencedObject1"));

      IBocColumnRenderer renderer = new BocSimpleColumnRenderer (List, Html.Writer, Column);

      renderer.RenderDataCell (0, false, "bocListTableCell", EventArgs);
      HtmlDocument document = Html.GetResultDocument();
    }
  }
}