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
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode;

namespace OBWTest
{
  public class MockServiceLocator : ServiceLocatorImplBase
  {
    protected override object DoGetInstance (Type serviceType, string key)
    {
      switch (key)
      {
        case "IBocColumnRenderer<BocSimpleColumnDefinition>":
          return typeof (BocSimpleColumnRenderer);
        case "IBocColumnRenderer<BocCompoundColumnDefinition>":
          return typeof (BocCompoundColumnRenderer);
        case "IBocColumnRenderer<BocCommandColumnDefinition>":
          return typeof (BocCommandColumnRenderer);
        case "IBocColumnRenderer<BocCustomColumnDefinition>":
          return typeof (BocCustomColumnRenderer);
        case "IBocColumnRenderer<BocDropDownMenuColumnDefinition>":
          return typeof (BocDropDownMenuColumnRenderer);
        case "IBocColumnRenderer<BocRowEditModeColumnDefinition>":
          return typeof (BocRowEditModeColumnRenderer);
      }
      throw new InvalidOperationException();
    }

    protected override IEnumerable<object> DoGetAllInstances (Type serviceType)
    {
      List<object> columnRendererTypes = new List<object>();
      columnRendererTypes.Add (typeof (BocSimpleColumnRenderer));
      columnRendererTypes.Add (typeof (BocCompoundColumnRenderer));
      columnRendererTypes.Add (typeof (BocCommandColumnRenderer));
      columnRendererTypes.Add (typeof (BocCustomColumnRenderer));
      columnRendererTypes.Add (typeof (BocDropDownMenuColumnRenderer));
      columnRendererTypes.Add (typeof (BocRowEditModeColumnRenderer));

      return columnRendererTypes;
    }
  }
}