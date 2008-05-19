using System;
using NUnit.Framework;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;

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