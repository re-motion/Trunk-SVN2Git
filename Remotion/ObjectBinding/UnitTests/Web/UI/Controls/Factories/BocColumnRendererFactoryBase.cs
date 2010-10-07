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
using System.Web.UI;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using System.Web;
using Remotion.Web;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Factories
{
  [TestFixture]
  public class BocColumnRendererFactoryBase
  {
    protected HttpContextBase HttpContext { get; set; }
    protected HtmlHelper Html { get; set; }
    protected ObjectBinding.Web.UI.Controls.BocList List { get; set; }
    protected Page Page { get; set; }

    [SetUp]
    public void SetUp ()
    {
      HttpContext = MockRepository.GenerateMock<HttpContextBase>();

      Html = new HtmlHelper();

      List = new ObjectBinding.Web.UI.Controls.BocList();
      Page = new Page();
      Page.Controls.Add (List);
    }

    protected void CreateRenderer<T> (T column, Func<HttpContextBase, BocList, IServiceLocator, IBocColumnRenderer> bocColumnRenderer)
        where T : BocColumnDefinition
    {
      column.ColumnTitle = "TestColumn1";
      List.FixedColumns.Add (column);

      var serviceLocatorStub = MockRepository.GenerateStub<IServiceLocator>();
      var resourceUrlFactory = MockRepository.GenerateStub<IResourceUrlFactory>();
      serviceLocatorStub.Stub (stub => stub.GetInstance<IResourceUrlFactory> ()).Return (resourceUrlFactory);

      IBocColumnRenderer renderer = bocColumnRenderer(HttpContext, List, serviceLocatorStub);

      Assert.IsInstanceOfType (typeof (BocColumnRendererBase<T>), renderer);
      //Assert.AreSame (List, ((BocColumnRendererBase<T>) renderer).List);
      //Assert.AreSame (column, ((BocColumnRendererBase<T>) renderer).Column);
      Assert.AreSame (resourceUrlFactory, ((BocColumnRendererBase<T>) renderer).ResourceUrlFactory);
    }
  }
}