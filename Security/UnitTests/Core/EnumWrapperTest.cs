using System;
using NUnit.Framework;
using Remotion.Security.UnitTests.Core.SampleDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class EnumWrapperTest
  {
    [Test]
    [Obsolete ("Remove typename check, repalce with tostring.", true)]
    public void InitializeFromEnum ()
    {
      EnumWrapper wrapper = new EnumWrapper (TestAccessTypes.First);

      Assert.AreEqual ("First", wrapper.Name);
      Assert.AreEqual ("Remotion.Security.UnitTests.Core.SampleDomain.TestAccessTypes, Remotion.Security.UnitTests", wrapper.TypeName);
    }

    [Test]
    [Obsolete ("Remove typename check, repalce with tostring.", true)]
    public void InitializeFromString ()
    {
      EnumWrapper wrapper = new EnumWrapper ("First", "Remotion.Security.UnitTests::Core.SampleDomain.TestAccessTypes");

      Assert.AreEqual ("First", wrapper.Name);
      Assert.AreEqual ("Remotion.Security.UnitTests.Core.SampleDomain.TestAccessTypes, Remotion.Security.UnitTests", wrapper.TypeName);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Enumerated type 'Remotion.Security.UnitTests.Core.SampleDomain.TestFlags' cannot be wrapped. "
        + "Only enumerated types without the System.FlagsAttribute can be wrapped.\r\nParameter name: enumValue")]
    public void InitializeWithEnumHavingFlagsAttribute ()
    {
      new EnumWrapper (TestFlags.First);
    }

    [Test]
    public void Equals ()
    {
      EnumWrapper expected = new EnumWrapper (TestAccessTypes.First);

      Assert.IsTrue (expected.Equals (expected));
      
      Assert.IsTrue (expected.Equals (new EnumWrapper (TestAccessTypes.First)));
      Assert.IsTrue (new EnumWrapper (TestAccessTypes.First).Equals (expected));

      Assert.IsTrue (expected.Equals (new EnumWrapper ("First", "Remotion.Security.UnitTests.Core.SampleDomain.TestAccessTypes, Remotion.Security.UnitTests")));
      Assert.IsTrue (new EnumWrapper ("First", "Remotion.Security.UnitTests.Core.SampleDomain.TestAccessTypes, Remotion.Security.UnitTests").Equals (expected));
      
      Assert.IsFalse (expected.Equals (new EnumWrapper (TestAccessTypes.Second)));
      Assert.IsFalse (new EnumWrapper (TestAccessTypes.Second).Equals (expected));
      
      Assert.IsFalse (expected.Equals (null));

      Assert.AreEqual (expected, new EnumWrapper (TestAccessTypes.First));
      Assert.AreNotEqual (expected, new EnumWrapper (TestAccessTypes.Second));
    }

    [Test]
    public void TestGetHashCode ()
    {
      EnumWrapper expected = new EnumWrapper (TestAccessTypes.First);

      Assert.AreEqual (expected.GetHashCode (), new EnumWrapper (TestAccessTypes.First).GetHashCode ());
      Assert.AreEqual (expected.GetHashCode (), new EnumWrapper ("First", "Remotion.Security.UnitTests.Core.SampleDomain.TestAccessTypes, Remotion.Security.UnitTests").GetHashCode ());
    }

    [Test]
    [ExpectedException (typeof (TypeLoadException))]
    [Obsolete ("Remove.", true)]
    public void GetEnum_FromInvalidTypeName ()
    {
      EnumWrapper wrapper = new EnumWrapper ("First", "Remotion.Security.UnitTests::Core.SampleDomain.Invalid");

      wrapper.GetEnum ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The type 'Remotion.Security.UnitTests.Core.SampleDomain.SimpleType, Remotion.Security.UnitTests' is not an enumerated type.")]
    [Obsolete ("Remove.", true)]
    public void GetEnum_FromTypeNotEnum ()
    {
      EnumWrapper wrapper = new EnumWrapper ("First", "Remotion.Security.UnitTests::Core.SampleDomain.SimpleType");

      wrapper.GetEnum ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The enumerated type 'Remotion.Security.UnitTests.Core.SampleDomain.TestAccessTypes, Remotion.Security.UnitTests' does not define the value 'Invalid'.")]
    [Obsolete ("Remove.", true)]
    public void GetEnum_FromInvalidName ()
    {
      EnumWrapper wrapper = new EnumWrapper ("Invalid", "Remotion.Security.UnitTests::Core.SampleDomain.TestAccessTypes");

      wrapper.GetEnum ();
    }

    [Test]
    public void ConvertToString ()
    {
      EnumWrapper wrapper = new EnumWrapper ("Name", "Namespace.TypeName, Assembly");

      Assert.AreEqual ("Name|Namespace.TypeName, Assembly", wrapper.ToString ());
    }

    [Test]
    [Obsolete ("Remove typename check, repalce with tostring.", true)]
    public void Parse ()
    {
      EnumWrapper wrapper = EnumWrapper.Parse ("Name|Namespace.TypeName, Assembly");

      Assert.AreEqual ("Namespace.TypeName, Assembly", wrapper.TypeName);
      Assert.AreEqual ("Name", wrapper.Name);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The value 'Name' did not contain the type name of the enumerated value. Expected format: 'Name|TypeName'\r\nParameter name: value")]
    public void Parse_WithMissingPipe ()
    {
      EnumWrapper.Parse ("Name");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The value '|Namespace.TypeName, Assembly' did not contain the name of the enumerated value. Expected format: 'Name|TypeName'\r\nParameter name: value")]
    public void Parse_WithMissingName ()
    {
      EnumWrapper.Parse ("|Namespace.TypeName, Assembly");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The value 'Name|' did not contain the type name of the enumerated value. Expected format: 'Name|TypeName'\r\nParameter name: value")]
    public void Parse_WithMissingTypeName ()
    {
      EnumWrapper.Parse ("Name|");
    }

    [Test]
    public void Serialization ()
    {
      EnumWrapper wrapper = new EnumWrapper ("bla", "ble");
      EnumWrapper deserializedWrapper = Serializer.SerializeAndDeserialize (wrapper);
      Assert.AreEqual (wrapper, deserializedWrapper);
    }
  }
}