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
using Remotion.ObjectBinding.Web.UI.Controls;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation.Rendering
{
  public abstract class ColumnRendererTestBase<T> : BocListRendererTestBase
      where T: BocColumnDefinition
  {
    protected T Column { get; set; }

    public virtual void SetUp ()
    {
      Initialize();

      List.FixedColumns.Add (Column);
      List.Stub (mock => mock.GetColumns()).Return (List.FixedColumns.ToArray());
      List.Stub (mock => mock.GetColumnRenderers()).Return (
          List.FixedColumns.ToArray().Select ((cd, i) => cd.GetRenderer (new StubServiceLocator(), HttpContext, List, i)).ToArray());
      List.Stub (stub => stub.ResolveClientUrl (null)).IgnoreArguments ().Do ((Func<string, string>) (url => url.TrimStart ('~')));
    }
  }
}