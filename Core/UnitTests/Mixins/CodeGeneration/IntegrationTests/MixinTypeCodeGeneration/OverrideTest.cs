/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration
{
  [TestFixture]
  public class OverrideTest : CodeGenerationBaseTest
  {
    [Test]
    public void OverrideMixinMethod ()
    {
      ClassOverridingMixinMembers com = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers)).With ();
      IMixinWithAbstractMembers comAsIAbstractMixin = com as IMixinWithAbstractMembers;
      Assert.IsNotNull (comAsIAbstractMixin);
      Assert.AreEqual ("MixinWithAbstractMembers.ImplementedMethod-ClassOverridingMixinMembers.AbstractMethod-25",
          comAsIAbstractMixin.ImplementedMethod ());
    }

    [Test]
    public void OverrideMixinProperty ()
    {
      ClassOverridingMixinMembers com = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers)).With ();
      IMixinWithAbstractMembers comAsIAbstractMixin = com as IMixinWithAbstractMembers;
      Assert.IsNotNull (comAsIAbstractMixin);
      Assert.AreEqual ("MixinWithAbstractMembers.ImplementedProperty-ClassOverridingMixinMembers.AbstractProperty",
          comAsIAbstractMixin.ImplementedProperty ());
    }

    [Test]
    public void OverrideMixinEvent ()
    {
      ClassOverridingMixinMembers com = CreateMixedObject<ClassOverridingMixinMembers> (typeof (MixinWithAbstractMembers)).With ();
      IMixinWithAbstractMembers comAsIAbstractMixin = com as IMixinWithAbstractMembers;
      Assert.IsNotNull (comAsIAbstractMixin);
      Assert.AreEqual ("MixinWithAbstractMembers.ImplementedEvent", comAsIAbstractMixin.ImplementedEvent ());
    }

    [Test]
    public void DoubleOverride ()
    {
      ClassOverridingSingleMixinMethod com = CreateMixedObject<ClassOverridingSingleMixinMethod> (typeof (MixinOverridingClassMethod)).With ();
      IMixinOverridingClassMethod comAsIAbstractMixin = com as IMixinOverridingClassMethod;
      Assert.IsNotNull (comAsIAbstractMixin);
      Assert.AreEqual ("ClassOverridingSingleMixinMethod.AbstractMethod-25", comAsIAbstractMixin.AbstractMethod (25));
      Assert.AreEqual ("MixinOverridingClassMethod.OverridableMethod-13", com.OverridableMethod (13));
    }

    [Test]
    public void ClassOverridingInheritedMixinMethod ()
    {
      ClassOverridingInheritedMixinMethod coimm = ObjectFactory.Create<ClassOverridingInheritedMixinMethod> ().With ();
      MixinWithInheritedMethod mixin = Mixin.Get<MixinWithInheritedMethod> (coimm);
      Assert.AreEqual ("ClassOverridingInheritedMixinMethod.ProtectedInheritedMethod-"
          + "ClassOverridingInheritedMixinMethod.ProtectedInternalInheritedMethod-"
          + "ClassOverridingInheritedMixinMethod.PublicInheritedMethod",
          mixin.InvokeInheritedMethods ());
    }

    [Test]
    public void ClassWithProtectedOverrider ()
    {
      ClassOverridingMixinMembersProtected com = CreateMixedObject<ClassOverridingMixinMembersProtected> (typeof (MixinWithAbstractMembers)).With ();
      IMixinWithAbstractMembers comAsIAbstractMixin = com as IMixinWithAbstractMembers;

      Assert.IsNotNull (comAsIAbstractMixin);
      Assert.AreEqual ("MixinWithAbstractMembers.ImplementedMethod-ClassOverridingMixinMembersProtected.AbstractMethod-25",
          comAsIAbstractMixin.ImplementedMethod ());
      Assert.AreEqual ("MixinWithAbstractMembers.ImplementedProperty-ClassOverridingMixinMembersProtected.AbstractProperty",
          comAsIAbstractMixin.ImplementedProperty ());
      Assert.AreEqual ("MixinWithAbstractMembers.ImplementedEvent", comAsIAbstractMixin.ImplementedEvent ());
    }
  }
}
