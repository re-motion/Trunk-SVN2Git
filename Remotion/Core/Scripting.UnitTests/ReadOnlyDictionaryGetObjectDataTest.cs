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
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ReadOnlyDictionaryGetObjectDataTest
  {
    [Test]
    [Explicit]
    public void GetObjectData ()
    {
      var dictionaryMock = MockRepository.GenerateMock<Dictionary<string, string>> ();
      var readOnlyDictionary = new ReadOnlyDictionary<string, string> (dictionaryMock);

      var formatterConverterStub = MockRepository.GenerateStub <IFormatterConverter>();
      var serializationInfo = new SerializationInfo (typeof (ReadOnlyDictionary<string, string>), formatterConverterStub); // Cannot mock SerializationInfo (sealed)      
      var streamingContext = new StreamingContext (); // Cannot mock StreamingContext (struct)

      readOnlyDictionary.GetObjectData (serializationInfo, streamingContext);
      dictionaryMock.Replay ();

      dictionaryMock.AssertWasCalled (d => d.GetObjectData (serializationInfo, streamingContext));
    }



    [Test]
    [Explicit]
    public void GetObjectData2 ()
    {
      var mocks = new MockRepository ();

      var formatterConverterStub = mocks.Stub<IFormatterConverter> ();
      var serializationInfo = new SerializationInfo (typeof (Object), formatterConverterStub); // Cannot mock SerializationInfo (sealed)
      var streamingContext = new StreamingContext (); // Cannot mock StreamingContext (struct)

      var dictionaryMock = mocks.StrictMock<Dictionary<string, string>> ();
      dictionaryMock.Expect (d => d.GetObjectData (serializationInfo, streamingContext));

      mocks.ReplayAll ();

      var readOnlyDictionary = new ReadOnlyDictionary<string, string> (dictionaryMock);
      readOnlyDictionary.GetObjectData (serializationInfo, streamingContext);

      mocks.VerifyAll ();
    }

    [Test]
    [Explicit]
    public void GetObjectData_TestDictionaryVersion ()
    {
      var mocks = new MockRepository ();

      var formatterConverterStub = mocks.Stub<IFormatterConverter> ();
      var serializationInfo = new SerializationInfo (typeof (Object), formatterConverterStub); // Cannot mock SerializationInfo (sealed)
      var streamingContext = new StreamingContext (); // Cannot mock StreamingContext (struct)

      var dictionaryMock = mocks.StrictMock<TestDictionary<string, string>> ();
      dictionaryMock.Expect (d => d.GetObjectData (serializationInfo, streamingContext));

      mocks.ReplayAll ();

      var readOnlyDictionary = new ReadOnlyDictionary<string, string> (dictionaryMock);
      readOnlyDictionary.GetObjectData (serializationInfo, streamingContext);

      mocks.VerifyAll ();
    }


    [Test]
    [Explicit]
    public void ToStringTest ()
    {
      var dictionaryMock = MockRepository.GenerateMock<Dictionary<string, string>> ();
      var readOnlyDictionary = new ReadOnlyDictionary<string, string> (dictionaryMock);

      readOnlyDictionary.ToString ();

      dictionaryMock.AssertWasCalled (d => d.ToString ());
    }


    [Test]
    [Explicit]
    public void ToStringTest2 ()
    {
      var mocks = new MockRepository ();

      var dictionaryMock = mocks.StrictMock<Dictionary<string, string>> ();
      dictionaryMock.Expect (d => d.ToString ()).Return ("");

      mocks.ReplayAll ();

      var readOnlyDictionary = new ReadOnlyDictionary<string, string> (dictionaryMock);
      readOnlyDictionary.ToString ();

      dictionaryMock.VerifyAllExpectations ();
    }

    [Test]
    [Explicit]
    public void ToStringTest3 ()
    {
      MockRepository mocks = new MockRepository ();
      var dictionaryMock = mocks.DynamicMock<Dictionary<string, string>> ();
      Assert.That (dictionaryMock, Is.Not.Null);
      var readOnlyDictionary = new ReadOnlyDictionary<string, string> (dictionaryMock);
      Expect.Call (dictionaryMock.ToString ()).Return ("xyz");
      mocks.ReplayAll ();
      Assert.That (readOnlyDictionary.ToString (), Is.EqualTo ("xyz"));
      mocks.VerifyAll ();
    }

    [Test]
    [Explicit]
    public void FooTest ()
    {
      MockRepository mocks = new MockRepository ();
      var fooMock = mocks.StrictMock<Foo> ();
      Assert.That (fooMock, Is.Not.Null);
      Expect.Call (fooMock.Bar ()).Return ("xyz");
      mocks.ReplayAll ();
      Assert.That (fooMock.Bar (), Is.EqualTo ("xyz"));
      mocks.VerifyAll ();
    }


    [Test]
    [Explicit]
    public void FooSecurityAttributeTest ()
    {
      MockRepository mocks = new MockRepository ();
      var fooMock = mocks.StrictMock<Foo> ();
      Assert.That (fooMock, Is.Not.Null);
      Expect.Call (fooMock.BarSecurityPermission ()).Return ("xyz");
      mocks.ReplayAll ();
      Assert.That (fooMock.BarSecurityPermission (), Is.EqualTo ("xyz"));
      mocks.VerifyAll ();
    }



    [Test]
    [Explicit]
    public void FooGenericTest ()
    {
      MockRepository mocks = new MockRepository ();
      var fooMock = mocks.StrictMock<Foo<string, int>> ();
      Assert.That (fooMock, Is.Not.Null);
      Expect.Call (fooMock.Bar ("DEF", 1234)).Return ("xyz");
      mocks.ReplayAll ();
      Assert.That (fooMock.Bar ("DEF", 1234), Is.EqualTo ("xyz"));
      mocks.VerifyAll ();
    }


    [Test]
    [Explicit]
    public void FooGenericSecurityAttributeTest ()
    {
      MockRepository mocks = new MockRepository ();
      var fooMock = mocks.StrictMock<Foo<string, int>> ();
      Assert.That (fooMock, Is.Not.Null);
      Expect.Call (fooMock.BarSecurityPermission ("DEF", 1234)).Return ("xyz");
      mocks.ReplayAll ();
      Assert.That (fooMock.BarSecurityPermission ("DEF", 1234), Is.EqualTo ("xyz"));
      mocks.VerifyAll ();
    }


    [Test]
    [Explicit]
    public void ToStringFoo ()
    {
      MockRepository mocks = new MockRepository ();
      var fooMock = mocks.DynamicMock<Foo<string, int>> ();
      Assert.That (fooMock, Is.Not.Null);
      Expect.Call (fooMock.ToString ()).Return ("xyz");
      mocks.ReplayAll ();
      Assert.That (fooMock.ToString (), Is.EqualTo ("xyz"));
      mocks.VerifyAll ();
    }

    public class Foo
    {
      public virtual string Bar ()
      {
        return "abc";
      }

      [SecurityPermission (SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
      public virtual string BarSecurityPermission ()
      {
        return "abc";
      }
    }

    public class Foo<T0, T1>
    {
      public virtual string Bar (T0 t0, T1 t1)
      {
        return "abc" + t0 + t1;
      }

      [SecurityPermission (SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
      public virtual string BarSecurityPermission (T0 t0, T1 t1)
      {
        return "abc" + t0 + t1;
      }
    }
  }

  [Serializable]
  public class TestDictionary<TKey, TValue> : Dictionary<TKey, TValue>
  {
    public TestDictionary ()
    {

    }

    protected TestDictionary (SerializationInfo info, StreamingContext context)
    {
    }

    public override void GetObjectData (SerializationInfo info, StreamingContext context)
    {

    }
  }
}