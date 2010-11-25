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
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.TestDomain;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class DuckRequirementTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeImplementsRequiredDuckInterfaces ()
    {
      ClassFulfillingAllMemberRequirementsDuck cfrd = ObjectFactory.Create<ClassFulfillingAllMemberRequirementsDuck> (ParamList.Empty);
      Assert.IsTrue (cfrd is IMixinRequiringAllMembersRequirements);
      var mixin = Mixin.Get<MixinRequiringAllMembersFace> (cfrd);
      Assert.IsNotNull (mixin);
      Assert.AreEqual (42, mixin.PropertyViaThis);
    }

    [Test]
    public void RequiredFaceInterfaceViaDuck ()
    {
      ClassFulfillingAllMemberRequirementsExplicitly cfamre = ObjectFactory.Create<ClassFulfillingAllMemberRequirementsExplicitly> (ParamList.Empty);
      var mixin = Mixin.Get<MixinRequiringAllMembersFace> (cfamre);
      Assert.IsNotNull (mixin);
      Assert.AreEqual (37, mixin.PropertyViaThis);
    }

    [Test]
    public void RequiredBaseInterfaceViaDuck ()
    {
      ClassFulfillingAllMemberRequirements cfamr = ObjectFactory.Create<ClassFulfillingAllMemberRequirements> (ParamList.Empty);
      var mixin = Mixin.Get<MixinRequiringAllMembersBase> (cfamr);
      Assert.IsNotNull (mixin);
      Assert.AreEqual (11, mixin.PropertyViaBase);
    }

    [Test]
    public void ThisCallToDuckInterface ()
    {
      BaseTypeWithDuckFaceMixin duckFace = ObjectFactory.Create<BaseTypeWithDuckFaceMixin> (ParamList.Empty);
      Assert.AreEqual ("DuckFaceMixin.CallMethodsOnThis-DuckFaceMixin.MethodImplementedOnBase-BaseTypeWithDuckFaceMixin.ProtectedMethodImplementedOnBase",
          Mixin.Get<DuckFaceMixin> (duckFace).CallMethodsOnThis ());
    }

  }
}
