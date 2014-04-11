// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using Remotion.Linq.Utilities;

namespace Remotion.Linq.UnitTests.Linq.Core.Utilities
{
  public class TestRegistry : RegistryBase<TestRegistry, Type, ITestRegistry>
  {
    public override ITestRegistry GetItem (Type key)
    {
      return GetItemExact (key);
    }

    protected override void RegisterForTypes (IEnumerable<Type> itemTypes)
    {
      foreach (var itemType in itemTypes)
      {
        var handler = (ITestRegistry) Activator.CreateInstance (itemType);
        Register (itemType, handler);
      }
    }

    public new ITestRegistry GetItemExact (Type key)
    {
      return base.GetItemExact (key);
    }
  }
}