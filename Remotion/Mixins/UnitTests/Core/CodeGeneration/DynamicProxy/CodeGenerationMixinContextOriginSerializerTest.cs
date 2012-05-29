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
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Context;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.DynamicProxy
{
  [TestFixture]
  public class CodeGenerationMixinContextOriginSerializerTest : CodeGenerationBaseTest
  {
    [Test]
    public void IntegrationTest()
    {
      var someAssembly = GetType().Assembly;
      var referenceContextOrigin = new MixinContextOrigin ("some kind", someAssembly, "some location");

      var module = ((ModuleManager) (SavedTypeBuilder.Scope)).Scope.ObtainDynamicModuleWithWeakName ();
      var type = module.DefineType ("CodeGenerationMixinContextOriginSerializerTest.IntegrationTest");
      var method =
          type.DefineMethod ("Test", MethodAttributes.Public | MethodAttributes.Static, typeof (MixinContextOrigin), Type.EmptyTypes);
      var emitter = new MethodBuilderEmitter (method);

      var serializer = new CodeGenerationMixinContextOriginSerializer ();
      referenceContextOrigin.Serialize (serializer);

      emitter.CodeBuilder.AddStatement (new ReturnStatement (serializer.GetConstructorInvocationExpression ()));
      emitter.Generate ();

      Type compiledType = type.CreateType ();
      var result = (MixinContextOrigin) compiledType.GetMethod ("Test").Invoke (null, null);

      Assert.That (result.Kind, Is.EqualTo ("some kind"));
      Assert.That (result.Assembly, Is.SameAs (someAssembly));
      Assert.That (result.Location, Is.EqualTo ("some location"));
    }
  }
}
