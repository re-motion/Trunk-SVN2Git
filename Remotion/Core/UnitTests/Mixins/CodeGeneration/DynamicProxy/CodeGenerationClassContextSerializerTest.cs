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
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Context;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.DynamicProxy
{
  [TestFixture]
  public class CodeGenerationClassContextSerializerTest : CodeGenerationBaseTest
  {
    [Test]
    public void IntegrationTest()
    {
      var referenceMixinContext1 = new MixinContext (MixinKind.Extending, typeof (BT1Mixin1), MemberVisibility.Private, new[] { typeof (string), typeof (double) });
      var referenceMixinContext2 = new MixinContext (MixinKind.Used, typeof (BT1Mixin2), MemberVisibility.Public);
      var referenceClassContext = new ClassContext (typeof (BaseType1), new[] { referenceMixinContext1, referenceMixinContext2 }, new[] { typeof (DateTime) });

      var module = ((ModuleManager) (SavedTypeBuilder.Scope)).Scope.ObtainDynamicModuleWithWeakName ();
      var type = module.DefineType ("CodeGenerationClassContextSerializerTest.IntegrationTest");
      var method =
          type.DefineMethod ("Test", MethodAttributes.Public | MethodAttributes.Static, typeof (ClassContext), Type.EmptyTypes);
      var emitter = new MethodBuilderEmitter (method);

      var serializer = new CodeGenerationClassContextSerializer (emitter.CodeBuilder);
      referenceClassContext.Serialize (serializer);

      emitter.CodeBuilder.AddStatement (new ReturnStatement (serializer.GetConstructorInvocationExpression ()));
      emitter.Generate ();

      Type compiledType = type.CreateType ();
      object result = compiledType.GetMethod ("Test").Invoke (null, null);

      Assert.That (result, Is.EqualTo (referenceClassContext));
    }
  }
}
