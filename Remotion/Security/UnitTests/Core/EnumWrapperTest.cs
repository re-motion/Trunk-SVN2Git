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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Security;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class EnumWrapperTest
  {
    public enum TestEnum { One, Two, Three }
    [Flags]
    public enum TestFlags { One, Two, Three }

    [Test]
    public void Initialization_FromEnumValue ()
    {
      EnumWrapper wrapper = new EnumWrapper(TestEnum.One);
      Assert.That (wrapper.Name, Is.EqualTo ("One|Remotion.Security.UnitTests.Core.EnumWrapperTest+TestEnum, Remotion.Security.UnitTests"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Enumerated type 'Remotion.Security.UnitTests.Core.EnumWrapperTest+TestFlags' "
        + "cannot be wrapped. Only enumerated types without the System.FlagsAttribute can be wrapped.\r\nParameter name: enumValue")]
    public void Initialization_FromFlags_Fails ()
    {
      new EnumWrapper (TestFlags.One);
    }

    [Test]
    public void Initialization_FromString ()
    {
      EnumWrapper wrapper = new EnumWrapper("123");
      Assert.That (wrapper.Name, Is.EqualTo ("123"));
    }

    [Test]
    public void Initialization_FromStrings ()
    {
      EnumWrapper wrapper = new EnumWrapper ("123", "456");
      Assert.That (wrapper.Name, Is.EqualTo ("123|456"));
    }

    [Test]
    public void Equals_True ()
    {
      EnumWrapper one = new EnumWrapper ("123");
      EnumWrapper two = new EnumWrapper ("123");
      Assert.That (one, Is.EqualTo (two));
    }

    [Test]
    public void Equals_False ()
    {
      EnumWrapper one = new EnumWrapper ("123");
      EnumWrapper two = new EnumWrapper ("321");
      Assert.That (one, Is.Not.EqualTo (two));
    }

    [Test]
    public void Equatable_Equals_True ()
    {
      EnumWrapper one = new EnumWrapper ("123");
      EnumWrapper two = new EnumWrapper ("123");
      Assert.That (one.Equals (two));
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      EnumWrapper one = new EnumWrapper ("123");
      EnumWrapper two = new EnumWrapper ("123");
      Assert.That (one.GetHashCode(), Is.EqualTo (two.GetHashCode()));
    }

    [Test]
    public void ConvertToString_SimpleName ()
    {
      EnumWrapper one = new EnumWrapper ("123");
      Assert.That (one.ToString (), Is.EqualTo (one.Name));
    }

    [Test]
    public void ConvertToString_WithTypeName ()
    {
      EnumWrapper wrapper = new EnumWrapper ("Name", "Namespace.TypeName, Assembly");

      Assert.AreEqual ("Name|Namespace.TypeName, Assembly", wrapper.ToString ());
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
