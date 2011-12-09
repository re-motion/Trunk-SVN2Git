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
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  public static class ConcreteTypeBuilderTestHelper
  {
    public static ModuleManager GetCurrentModuleManager ()
    {
      var concreteTypeBuilder = ConcreteTypeBuilder.Current;
      return GetModuleManager(concreteTypeBuilder);
    }

    public static ModuleManager GetModuleManager (IConcreteTypeBuilder concreteTypeBuilder)
    {
      return (ModuleManager) ((ConcreteTypeBuilder) concreteTypeBuilder).Scope;
    }

    public static IModuleManager GetIModuleManager (IConcreteTypeBuilder concreteTypeBuilder)
    {
      return ((ConcreteTypeBuilder) concreteTypeBuilder).Scope;
    }
  }
}