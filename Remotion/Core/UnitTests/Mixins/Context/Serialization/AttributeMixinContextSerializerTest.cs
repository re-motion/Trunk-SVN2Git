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
using System.Runtime.Serialization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context.Serialization;

namespace Remotion.UnitTests.Mixins.Context.Serialization
{
  [TestFixture]
  public class AttributeMixinContextSerializationTest
  {
    private AttributeMixinContextSerializer _serializer;
    private AttributeMixinContextDeserializer _deserializer;
    private AttributeMixinContextDeserializer _invalidDeserializer;

    [SetUp]
    public void SetUp()
    {
      _serializer = new AttributeMixinContextSerializer ();
      _deserializer = new AttributeMixinContextDeserializer (_serializer.Values);
      _invalidDeserializer = new AttributeMixinContextDeserializer (new object[] {1, 2, 3, 4});
    }

    [Test]
    public void AddMixinType()
    {
      _serializer.AddMixinType (typeof (DateTime));
      Assert.That (_deserializer.GetMixinType (), Is.EqualTo (typeof (DateTime)));
    }

    [Test]
    public void AddMixinKind()
    {
      _serializer.AddMixinKind (MixinKind.Used);
      Assert.That (_deserializer.GetMixinKind (), Is.EqualTo (MixinKind.Used));
    }

    [Test]
    public void AddIntroducedMemberVisibility ()
    {
      _serializer.AddIntroducedMemberVisibility (MemberVisibility.Public);
      Assert.That (_deserializer.GetIntroducedMemberVisibility(), Is.EqualTo (MemberVisibility.Public));
    }

    [Test]
    public void AddExplicitDependencies ()
    {
      _serializer.AddExplicitDependencies (new[] { typeof (int), typeof (string) });
      Assert.That (_deserializer.GetExplicitDependencies (), Is.EqualTo (new[] { typeof (int), typeof (string) }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Expected an array with 4 elements.\r\nParameter name: values")]
    public void Deserializer_InvalidArray()
    {
      new AttributeMixinContextDeserializer (new[] { "x" });
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'System.Type' at index 0 in the values array, but found 'System.Int32'.")]
    public void GetMixinType_Invalid()
    {
      _invalidDeserializer.GetMixinType ();
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'Remotion.Mixins.MixinKind' at index 1 in the values array, but found 'System.Int32'.")]
    public void GetMixinKind_Invalid ()
    {
      _invalidDeserializer.GetMixinKind ();
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'Remotion.Mixins.MemberVisibility' at index 2 in the values array, but found 'System.Int32'.")]
    public void GetIntroducedMemberVisibility_Invalid ()
    {
      _invalidDeserializer.GetIntroducedMemberVisibility();
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'System.Type[]' at index 3 in the values array, but found 'System.Int32'.")]
    public void GetExplicitDependencies_Invalid ()
    {
      _invalidDeserializer.GetExplicitDependencies ();
    }
  }
}
