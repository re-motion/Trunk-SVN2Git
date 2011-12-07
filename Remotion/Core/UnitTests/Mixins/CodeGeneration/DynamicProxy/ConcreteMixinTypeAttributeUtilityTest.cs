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
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.CodeGeneration.DynamicProxy
{
  [TestFixture]
  public class ConcreteMixinTypeAttributeUtilityTest
  {
    [Test]
    public void CreateAttributeBuilder ()
    {
      var identifier = new ConcreteMixinTypeIdentifier (typeof (double), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());
      CustomAttributeBuilder builder = ConcreteMixinTypeAttributeUtility.CreateAttributeBuilder (identifier);

      var moduleManager = ConcreteTypeBuilderTestHelper.GetCurrentModuleManager();
      TypeBuilder typeBuilder = moduleManager
          .Scope
          .ObtainDynamicModuleWithWeakName ()
          .DefineType ("Test_ConcreteMixinTypeAttribute");
      typeBuilder.SetCustomAttribute (builder);
      Type type = typeBuilder.CreateType ();

      var attribute = AttributeUtility.GetCustomAttribute<ConcreteMixinTypeAttribute> (type, false);
      
      var regeneratedIdentifier = attribute.GetIdentifier ();

      Assert.That (regeneratedIdentifier, Is.EqualTo (identifier));
    }
  }
}
