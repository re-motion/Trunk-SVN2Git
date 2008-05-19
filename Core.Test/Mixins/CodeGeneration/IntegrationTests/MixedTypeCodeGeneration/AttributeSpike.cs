using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class AttributeSpike
  {
    [AttributeUsage (AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class MultiInheritedAttribute : Attribute { }

    [AttributeUsage (AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    public class MultiNonInheritedAttribute : Attribute { }

    [AttributeUsage (AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class NonMultiInheritedAttribute : Attribute { }

    [AttributeUsage (AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class NonMultiNonInheritedAttribute : Attribute { }

    [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
    public class Base
    {
      [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
      public virtual void Method ()
      {
      }

      [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
      public virtual int Property
      {
        get { return 0; }
      }

      [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
      public virtual event EventHandler Event;
    }

    [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
    public class DerivedWithAttributes : Base
    {
      [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
      public override void Method ()
      {
        base.Method ();
      }

      [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
      public override int Property
      {
        get { return base.Property; }
      }

      [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
      public override event EventHandler Event;
    }

    public class DerivedWithoutAttributes : Base
    {
      public override void Method ()
      {
        base.Method ();
      }

      public override int Property
      {
        get { return base.Property; }
      }

      public override event EventHandler Event;
    }

    [Test]
    public void AttributesOnDerivedTypes ()
    {
      object[] attributes = typeof (DerivedWithoutAttributes).GetCustomAttributes (true);
      Assert.AreEqual (2, attributes.Length);
      Assert.That (
          attributes, Is.EquivalentTo (new object[] {new MultiInheritedAttribute(), new NonMultiInheritedAttribute()}));

      attributes = typeof (DerivedWithAttributes).GetCustomAttributes (true);
      Assert.AreEqual (5, attributes.Length);

      Assert.That (attributes, Is.EquivalentTo (new object[] { new MultiInheritedAttribute (), new MultiInheritedAttribute(),
            new NonMultiNonInheritedAttribute (), new MultiNonInheritedAttribute(), new NonMultiInheritedAttribute() }));
    }

    [Test]
    public void AttributesOnDerivedMethods ()
    {
      object[] attributes = typeof (DerivedWithoutAttributes).GetMethod("Method").GetCustomAttributes (true);
      Assert.AreEqual (2, attributes.Length);
      Assert.That (
          attributes, Is.EquivalentTo (new object[] { new MultiInheritedAttribute (), new NonMultiInheritedAttribute () }));

      attributes = typeof (DerivedWithAttributes).GetMethod ("Method").GetCustomAttributes (true);
      Assert.AreEqual (5, attributes.Length);

      Assert.That (attributes, Is.EquivalentTo (new object[] { new MultiInheritedAttribute (), new MultiInheritedAttribute(),
            new NonMultiNonInheritedAttribute (), new MultiNonInheritedAttribute(), new NonMultiInheritedAttribute() }));
    }

    [Test]
    public void AttributesOnDerivedProperties ()
    {
      object[] attributes = typeof (DerivedWithoutAttributes).GetProperty ("Property").GetCustomAttributes (true);
      Assert.AreEqual (0, attributes.Length);

      attributes = typeof (DerivedWithAttributes).GetProperty ("Property").GetCustomAttributes (true);
      Assert.AreEqual (4, attributes.Length);

      Assert.That (attributes, Is.EquivalentTo (new object[] { new MultiInheritedAttribute (), 
            new NonMultiNonInheritedAttribute (), new MultiNonInheritedAttribute(), new NonMultiInheritedAttribute() }));
    }

    [Test]
    public void AttributesOnDerivedEvents ()
    {
      object[] attributes = typeof (DerivedWithoutAttributes).GetEvent ("Event").GetCustomAttributes (true);
      Assert.AreEqual (0, attributes.Length);

      attributes = typeof (DerivedWithAttributes).GetEvent ("Event").GetCustomAttributes (true);
      Assert.AreEqual (4, attributes.Length);

      Assert.That (attributes, Is.EquivalentTo (new object[] { new MultiInheritedAttribute (), 
            new NonMultiNonInheritedAttribute (), new MultiNonInheritedAttribute(), new NonMultiInheritedAttribute() }));
    }
  }
}
