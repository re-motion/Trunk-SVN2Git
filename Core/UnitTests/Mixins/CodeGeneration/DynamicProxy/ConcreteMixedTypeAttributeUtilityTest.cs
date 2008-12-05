// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.CodeGeneration.DynamicProxy
{
  [TestFixture]
  public class ConcreteMixedTypeAttributeUtilityTest
  {
    [Test]
    public void CreateAttributeBuilder ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddCompleteInterface (typeof (uint))
          .AddMixin (typeof (double)).OfKind (MixinKind.Used)
          .AddMixin (typeof (string)).WithDependency (typeof (bool)).OfKind (MixinKind.Extending)
          .BuildClassContext ();

      CustomAttributeBuilder builder = ConcreteMixedTypeAttributeUtility.CreateAttributeBuilder (context);
      TypeBuilder typeBuilder = ((ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope.ObtainDynamicModuleWithWeakName ().DefineType ("Test_ConcreteMixedTypeAttribute");
      typeBuilder.SetCustomAttribute (builder);
      Type type = typeBuilder.CreateType ();
      ConcreteMixedTypeAttribute attribute = AttributeUtility.GetCustomAttribute<ConcreteMixedTypeAttribute> (type, false);
      Assert.That (attribute.GetClassContext (), Is.EqualTo (context));
    }
  }
}
