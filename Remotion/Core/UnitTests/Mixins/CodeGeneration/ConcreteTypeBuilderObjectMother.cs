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
using System;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  public static class ConcreteTypeBuilderObjectMother
  {
    public static ConcreteTypeBuilder CreateConcreteTypeBuilder ()
    {
      return new ConcreteTypeBuilder (new ModuleManagerFactory (), new GuidNameProvider (), new GuidNameProvider ());
    }

    public static ConcreteTypeBuilder CreateConcreteTypeBuilderWithFixedModuleManager (IModuleManager moduleManager)
    {
      var factory = ModuleManagerFactoryObjectMother.CreateFixedModuleManagerFactory (moduleManager);
      return new ConcreteTypeBuilder (factory, new GuidNameProvider (), new GuidNameProvider ());
    }

    public static ConcreteTypeBuilder CreateConcreteTypeBuilder (IModuleManager moduleManager, IConcreteMixedTypeNameProvider nameProviderMock)
    {
      return new ConcreteTypeBuilder (
          ModuleManagerFactoryObjectMother.CreateFixedModuleManagerFactory (moduleManager),
          nameProviderMock,
          new GuidNameProvider ());
    }

    public static IConcreteTypeBuilder CreateConcreteTypeBuilder (IModuleManager moduleManager, IConcreteMixinTypeNameProvider nameProviderMock)
    {
      return new ConcreteTypeBuilder (
          ModuleManagerFactoryObjectMother.CreateFixedModuleManagerFactory (moduleManager),
          new GuidNameProvider(),
          nameProviderMock);
    }
  }
}