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
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration
{
  [TestFixture]
  public class OverrideTest : CodeGenerationBaseTest
  {
    [Test]
    public void OverrideMixinMethod ()
    {
      ClassOverridingMixinMembers com = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers));
      var comAsIAbstractMixin = com as IMixinWithAbstractMembers;
      Assert.IsNotNull (comAsIAbstractMixin);
      Assert.AreEqual ("MixinWithAbstractMembers.ImplementedMethod-ClassOverridingMixinMembers.AbstractMethod-25",
          comAsIAbstractMixin.ImplementedMethod ());
    }

    [Test]
    public void OverrideMixinProperty ()
    {
      ClassOverridingMixinMembers com = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers));
      var comAsIAbstractMixin = com as IMixinWithAbstractMembers;
      Assert.IsNotNull (comAsIAbstractMixin);
      Assert.AreEqual ("MixinWithAbstractMembers.ImplementedProperty-ClassOverridingMixinMembers.AbstractProperty",
          comAsIAbstractMixin.ImplementedProperty ());
    }

    [Test]
    public void OverrideMixinEvent ()
    {
      ClassOverridingMixinMembers com = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers));
      var comAsIAbstractMixin = com as IMixinWithAbstractMembers;
      Assert.IsNotNull (comAsIAbstractMixin);
      Assert.AreEqual ("MixinWithAbstractMembers.ImplementedEvent", comAsIAbstractMixin.ImplementedEvent ());
    }

    [Test]
    public void DoubleOverride ()
    {
      ClassOverridingSingleMixinMethod com = CreateMixedObject<ClassOverridingSingleMixinMethod> (typeof (MixinOverridingClassMethod));
      var comAsIAbstractMixin = com as IMixinOverridingClassMethod;
      Assert.IsNotNull (comAsIAbstractMixin);
      Assert.AreEqual ("ClassOverridingSingleMixinMethod.AbstractMethod-25", comAsIAbstractMixin.AbstractMethod (25));
      Assert.AreEqual ("MixinOverridingClassMethod.OverridableMethod-13", com.OverridableMethod (13));
    }

    [Test]
    public void ClassOverridingInheritedMixinMethod ()
    {
      ClassOverridingInheritedMixinMethod coimm = ObjectFactory.Create<ClassOverridingInheritedMixinMethod> (ParamList.Empty);
      var mixin = Mixin.Get<MixinWithInheritedMethod> (coimm);
      Assert.AreEqual ("ClassOverridingInheritedMixinMethod.ProtectedInheritedMethod-"
          + "ClassOverridingInheritedMixinMethod.ProtectedInternalInheritedMethod-"
          + "ClassOverridingInheritedMixinMethod.PublicInheritedMethod",
          mixin.InvokeInheritedMethods ());
    }

    [Test]
    public void ClassWithProtectedOverrider ()
    {
      ClassOverridingMixinMembersProtected com = CreateMixedObject<ClassOverridingMixinMembersProtected> (typeof (MixinWithAbstractMembers));
      var comAsIAbstractMixin = com as IMixinWithAbstractMembers;

      Assert.IsNotNull (comAsIAbstractMixin);
      Assert.AreEqual ("MixinWithAbstractMembers.ImplementedMethod-ClassOverridingMixinMembersProtected.AbstractMethod-25",
          comAsIAbstractMixin.ImplementedMethod ());
      Assert.AreEqual ("MixinWithAbstractMembers.ImplementedProperty-ClassOverridingMixinMembersProtected.AbstractProperty",
          comAsIAbstractMixin.ImplementedProperty ());
      Assert.AreEqual ("MixinWithAbstractMembers.ImplementedEvent", comAsIAbstractMixin.ImplementedEvent ());
    }
  }
}
