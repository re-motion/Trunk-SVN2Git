using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Core.Metadata
{
  [TestFixture]
  public class EnumValueInfoListContentsAsserterTest
  {
    // types

    // static members

    // member fields

    private List<EnumValueInfo> _list;

    // construction and disposing

    public EnumValueInfoListContentsAsserterTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _list = new List<EnumValueInfo> ();
      _list.Add (new EnumValueInfo ("TypeName", "First", 1));
      _list.Add (new EnumValueInfo ("TypeName", "Second", 2));
    }

    [Test]
    public void AssertWithValidValue ()
    {
      EnumValueInfoListContentsAsserter asserter = new EnumValueInfoListContentsAsserter ("First", _list, string.Empty, null);
      Assert.IsTrue (asserter.Test ());
    }

    [Test]
    public void AssertWithListNull ()
    {
      EnumValueInfoListContentsAsserter asserter = new EnumValueInfoListContentsAsserter ("First", null, string.Empty, null);
      Assert.IsFalse (asserter.Test ());
    }

    [Test]
    public void AssertWithInvalidValue ()
    {
      EnumValueInfoListContentsAsserter asserter = new EnumValueInfoListContentsAsserter ("Other", _list, string.Empty, null);
      Assert.IsFalse (asserter.Test ());
    }

    [Test]
    public void GetMessage ()
    {
      EnumValueInfoListContentsAsserter asserter = new EnumValueInfoListContentsAsserter ("Expected", _list, string.Empty, null);
      Assert.AreEqual ("\texpected: <\"Expected\">\r\n\t but was: <<\"First\">,<\"Second\">>\r\n", asserter.Message);
    }

    [Test]
    public void GetMessageWithUserMessage ()
    {
      EnumValueInfoListContentsAsserter asserter = new EnumValueInfoListContentsAsserter ("Expected", _list, "Custom: {0}", "value");
      Assert.AreEqual ("Custom: value\r\n\texpected: <\"Expected\">\r\n\t but was: <<\"First\">,<\"Second\">>\r\n", asserter.Message);
    }
  }
}