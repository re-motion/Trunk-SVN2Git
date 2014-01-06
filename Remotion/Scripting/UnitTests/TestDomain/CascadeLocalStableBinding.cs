﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Runtime.CompilerServices;
using Castle.DynamicProxy;
using Remotion.Scripting.StableBindingImplementation;

namespace Remotion.Scripting.UnitTests.TestDomain
{
  public class CascadeLocalStableBinding : Cascade
  {
    private static ModuleScope CreateModuleScope ()
    {
      string name = "Remotion.Scripting.CodeGeneration.Generated.Test.CascadeLocalStableBinding";
      string nameSigned = name + ".Signed";
      string nameUnsigned = name + ".Unsigned";
      const string ext = ".dll";
      return new ModuleScope (true, false, nameSigned, nameSigned + ext, nameUnsigned, nameUnsigned + ext);
    }

    private readonly StableBindingProxyProvider _proxyProvider = new StableBindingProxyProvider (
        new TypeLevelTypeFilter (new[] { typeof (CascadeLocalStableBinding) }),
        CreateModuleScope());

    public CascadeLocalStableBinding (int nrChildren)
    {
      --nrChildren;
      Name = "C" + nrChildren;
      if (nrChildren > 0)
        Child = new CascadeLocalStableBinding (nrChildren);
    }

    [SpecialName]
    public object GetCustomMember (string name)
    {
      return _proxyProvider.GetAttributeProxy (this, name);
    }
  }
}