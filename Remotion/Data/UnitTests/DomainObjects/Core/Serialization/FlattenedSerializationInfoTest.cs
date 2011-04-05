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
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class FlattenedSerializationInfoTest
  {
    [Test]
    public void Values ()
    {
      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo();
      serializationInfo.AddIntValue (1);
      serializationInfo.AddBoolValue (true);
      serializationInfo.AddIntValue (2);
      serializationInfo.AddBoolValue (false);
      serializationInfo.AddValue (new DateTime (2007, 1, 2));
      serializationInfo.AddValue ("Foo");
      serializationInfo.AddValue<object> (null);
      serializationInfo.AddValue<int?> (null);
      object[] data = serializationInfo.GetData();

      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);
      Assert.AreEqual (1, deserializationInfo.GetIntValue ());
      Assert.AreEqual (true, deserializationInfo.GetBoolValue ());
      Assert.AreEqual (2, deserializationInfo.GetIntValue ());
      Assert.AreEqual (false, deserializationInfo.GetBoolValue ());
      Assert.AreEqual (new DateTime (2007, 1, 2), deserializationInfo.GetValue<DateTime> ());
      Assert.AreEqual ("Foo", deserializationInfo.GetValue<string> ());
      Assert.AreEqual (null, deserializationInfo.GetValue<int?> ());
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Object stream: The serialization stream contains an object of type "
        + "System.DateTime at position 0, but an object of type System.String was expected.")]
    public void InvalidDeserializedType ()
    {
      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddValue (DateTime.Now);
      object[] data = serializationInfo.GetData ();

      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);
      deserializationInfo.GetValue<string> ();
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Object stream: There is no more data in the serialization stream at "
        + "position 0.")]
    public void InvalidDeserializedType_DifferentStream ()
    {
      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddIntValue (1);
      object[] data = serializationInfo.GetData ();

      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);
      deserializationInfo.GetValue<string> ();
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Object stream: The serialization stream contains a null value at "
       + "position 0, but an object of type System.DateTime was expected.")]
    public void InvalidDeserializedType_WithNullAndValueType ()
    {
      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddValue<object> (null);
      object[] data = serializationInfo.GetData ();

      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);
      deserializationInfo.GetValue<DateTime> ();
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Object stream: There is no more data in the serialization stream at "
        + "position 0.")]
    public void InvalidNumberOfDeserializedItems ()
    {
      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo ();
      object[] data = serializationInfo.GetData ();

      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);
      deserializationInfo.GetValue<string> ();
    }

    [Test]
    public void Arrays ()
    {
      object[] array1 = new object[] { "Foo", 1, 3.0 };
      DateTime[] array2 = new DateTime[] { DateTime.MinValue, DateTime.MaxValue };

      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddArray (array1);
      serializationInfo.AddArray (array2);
      object[] data = serializationInfo.GetData ();

      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);
      Assert.That (deserializationInfo.GetArray<object> (), Is.EqualTo (array1));
      Assert.That (deserializationInfo.GetArray<DateTime> (), Is.EqualTo (array2));
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Object stream: The serialization stream contains an object of type "
        + "System.String at position 0, but an object of type System.Int32 was expected.")]
    public void InvalidArrayType ()
    {
      object[] array1 = new object[] { "Foo", 1, 3.0 };

      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddArray (array1);
      object[] data = serializationInfo.GetData ();

      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);
      deserializationInfo.GetArray<int> ();
    }

    [Test]
    public void Collections ()
    {
      List<object> list1 = new List<object> (new object[] { "Foo", 1, 3.0 });
      List<DateTime> list2 = new List<DateTime> (new DateTime[] { DateTime.MinValue, DateTime.MaxValue });

      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddCollection (list1);
      serializationInfo.AddCollection (list2);
      object[] data = serializationInfo.GetData ();

      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);
      List<object> deserializedList1 = new List<object> ();
      List<DateTime> deserializedList2 = new List<DateTime> ();

      deserializationInfo.FillCollection (deserializedList1);
      deserializationInfo.FillCollection (deserializedList2);

      Assert.That (deserializedList1, Is.EqualTo (list1));
      Assert.That (deserializedList2, Is.EqualTo (list2));
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Object stream: The serialization stream contains an object of type "
        + "System.String at position 0, but an object of type System.Int32 was expected.")]
    public void InvalidCollectionType ()
    {
      List<object> list = new List<object> (new object[] { "Foo", 1, 3.0 });

      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddCollection (list);
      object[] data = serializationInfo.GetData ();

      List<int> deserializedList1 = new List<int> ();
      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);
      deserializationInfo.FillCollection (deserializedList1);
    }

    [Test]
    public void Handles ()
    {
      DateTime dt1 = DateTime.MinValue;
      DateTime dt2 = DateTime.MaxValue;

      string s1 = "Foo";
      string s2 = "Fox";

      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo ();

      serializationInfo.AddHandle (dt1);
      serializationInfo.AddHandle (dt2);
      serializationInfo.AddHandle (dt1);
      serializationInfo.AddHandle (dt1);
      serializationInfo.AddHandle (s1);
      serializationInfo.AddHandle (s2);
      serializationInfo.AddHandle (s1);
      serializationInfo.AddHandle (s1);
      serializationInfo.AddHandle (s2);

      object[] data = serializationInfo.GetData();

      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);

      Assert.AreEqual (dt1, deserializationInfo.GetValueForHandle<DateTime> ());
      Assert.AreEqual (dt2, deserializationInfo.GetValueForHandle<DateTime> ());
      Assert.AreEqual (dt1, deserializationInfo.GetValueForHandle<DateTime> ());
      Assert.AreEqual (dt1, deserializationInfo.GetValueForHandle<DateTime> ());
      Assert.AreEqual (s1, deserializationInfo.GetValueForHandle<string> ());
      Assert.AreEqual (s2, deserializationInfo.GetValueForHandle<string> ());
      Assert.AreEqual (s1, deserializationInfo.GetValueForHandle<string> ());
      Assert.AreEqual (s1, deserializationInfo.GetValueForHandle<string> ());
      Assert.AreEqual (s2, deserializationInfo.GetValueForHandle<string> ());
    }

    [Test]
    public void NullHandles ()
    {
      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo ();

      serializationInfo.AddHandle<string> (null);
      serializationInfo.AddHandle<int?> (null);

      object[] data = serializationInfo.GetData ();

      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);

      Assert.AreEqual (null, deserializationInfo.GetValueForHandle<string> ());
      Assert.AreEqual (null, deserializationInfo.GetValueForHandle<int?> ());
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Object stream: The serialization stream contains an object of type "
        + "System.DateTime at position 1, but an object of type System.String was expected.")]
    public void HandlesWithInvalidType ()
    {
      DateTime dt1 = DateTime.MinValue;

      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddHandle (dt1);
      serializationInfo.AddHandle (dt1);

      object[] data = serializationInfo.GetData ();

      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);
      deserializationInfo.GetValueForHandle<DateTime> ();
      deserializationInfo.GetValueForHandle<string> ();
    }

    [Test]
    public void FlattenedSerializables ()
    {
      FlattenedSerializableStub stub = new FlattenedSerializableStub ("begone, foul fiend", 123);
      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddValue (stub);
      object[] data = serializationInfo.GetData();

      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);
      FlattenedSerializableStub deserializedStub = deserializationInfo.GetValue<FlattenedSerializableStub> ();

      Assert.AreEqual ("begone, foul fiend", deserializedStub.Data1);
      Assert.AreEqual (123, deserializedStub.Data2);
    }

    [Test]
    public void FlattenedSerializables_Null ()
    {
      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddValue<FlattenedSerializableStub> (null);
      object[] data = serializationInfo.GetData ();

      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);
      FlattenedSerializableStub deserializedStub = deserializationInfo.GetValue<FlattenedSerializableStub> ();

      Assert.That (deserializedStub, Is.Null);
    }

    [Test]
    public void FlattenedSerializableHandles ()
    {
      FlattenedSerializableStub stub = new FlattenedSerializableStub ("begone, foul fiend", 123);
      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddHandle (stub);
      serializationInfo.AddHandle (stub);
      serializationInfo.AddHandle (stub);
      object[] data = serializationInfo.GetData ();

      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);
      FlattenedSerializableStub deserializedStub1 = deserializationInfo.GetValueForHandle<FlattenedSerializableStub> ();
      FlattenedSerializableStub deserializedStub2 = deserializationInfo.GetValueForHandle<FlattenedSerializableStub> ();
      FlattenedSerializableStub deserializedStub3 = deserializationInfo.GetValueForHandle<FlattenedSerializableStub> ();

      Assert.AreSame (deserializedStub1, deserializedStub2);
      Assert.AreSame (deserializedStub2, deserializedStub3);
      Assert.AreEqual ("begone, foul fiend", deserializedStub1.Data1);
      Assert.AreEqual (123, deserializedStub1.Data2);
    }

    [Test]
    public void FlattenedSerializableHandles_WithOtherHandles ()
    {
      FlattenedSerializableStub stub1 = new FlattenedSerializableStub ("begone, foul fiend", 123);
      FlattenedSerializableStub stub2 = new FlattenedSerializableStub ("befoul, gone fiend", 125);
      stub1.Data3 = stub2;
      
      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddHandle (stub1);
      object[] data = serializationInfo.GetData ();

      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);
      FlattenedSerializableStub deserializedStub1 = deserializationInfo.GetValueForHandle<FlattenedSerializableStub> ();
      FlattenedSerializableStub deserializedStub2 = deserializedStub1.Data3;

      Assert.AreNotSame (deserializedStub1, deserializedStub2);
      Assert.AreEqual ("begone, foul fiend", deserializedStub1.Data1);
      Assert.AreEqual (123, deserializedStub1.Data2);
      Assert.AreSame (deserializedStub2, deserializedStub1.Data3);

      Assert.AreEqual ("befoul, gone fiend", deserializedStub2.Data1);
      Assert.AreEqual (125, deserializedStub2.Data2);
      Assert.IsNull (deserializedStub2.Data3);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The serialized data contains a cycle, this is not supported.")]
    public void FlattenedSerializableHandles_RecursiveHandles ()
    {
      FlattenedSerializableStub stub1 = new FlattenedSerializableStub ("begone, foul fiend", 123);
      FlattenedSerializableStub stub2 = new FlattenedSerializableStub ("befoul, gone fiend", 125);
      stub1.Data3 = stub2;
      stub2.Data3 = stub1;

      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddHandle (stub1);
      object[] data = serializationInfo.GetData ();

      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);
      deserializationInfo.GetValueForHandle<FlattenedSerializableStub> ();
      // The following would be expected if this worked
      //FlattenedSerializableStub deserializedStub1 = deserializationInfo.GetValueForHandle<FlattenedSerializableStub> ();
      //FlattenedSerializableStub deserializedStub2 = deserializedStub1.Data3;

      //Assert.AreNotSame (deserializedStub1, deserializedStub2);
      //Assert.AreEqual ("begone, foul fiend", deserializedStub1.Data1);
      //Assert.AreEqual (123, deserializedStub1.Data2);
      //Assert.AreSame (deserializedStub2, deserializedStub1.Data3);

      //Assert.AreEqual ("befoul, gone fiend", deserializedStub2.Data1);
      //Assert.AreEqual (125, deserializedStub2.Data2);
      //Assert.AreSame (deserializedStub1, deserializedStub2.Data3);
    }

    [Test]
    public void FlattenedSerializableArray ()
    {
      FlattenedSerializableStub stub1 = new FlattenedSerializableStub ("begone, foul fiend", 123);
      FlattenedSerializableStub stub2 = new FlattenedSerializableStub ("'twas brillig, and the slithy toves", 124);
      FlattenedSerializableStub[] stubs = new FlattenedSerializableStub[] {stub1, stub2};
      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddArray (stubs);
      object[] data = serializationInfo.GetData ();

      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);
      FlattenedSerializableStub[] deserializedStubs = deserializationInfo.GetArray<FlattenedSerializableStub> ();

      Assert.AreEqual (2, deserializedStubs.Length);
      Assert.AreEqual ("begone, foul fiend", deserializedStubs[0].Data1);
      Assert.AreEqual (123, deserializedStubs[0].Data2);
      Assert.AreEqual ("'twas brillig, and the slithy toves", deserializedStubs[1].Data1);
      Assert.AreEqual (124, deserializedStubs[1].Data2);
    }

    [Test]
    public void ArrayWithFlattenedSerializables ()
    {
      FlattenedSerializableStub stub1 = new FlattenedSerializableStub ("begone, foul fiend", 123);
      FlattenedSerializableStub stub2 = new FlattenedSerializableStub ("'twas brillig, and the slithy toves", 124);
      object[] stubs = new object[] { stub1, stub2 };
      FlattenedSerializationInfo serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddArray (stubs);
      object[] data = serializationInfo.GetData ();

      FlattenedDeserializationInfo deserializationInfo = new FlattenedDeserializationInfo (data);
      object[] deserializedStubs = deserializationInfo.GetArray<object> ();

      Assert.AreEqual (2, deserializedStubs.Length);
      Assert.AreEqual ("begone, foul fiend", ((FlattenedSerializableStub) deserializedStubs[0]).Data1);
      Assert.AreEqual (123, ((FlattenedSerializableStub) deserializedStubs[0]).Data2);
      Assert.AreEqual ("'twas brillig, and the slithy toves", ((FlattenedSerializableStub) deserializedStubs[1]).Data1);
      Assert.AreEqual (124, ((FlattenedSerializableStub) deserializedStubs[1]).Data2);
    }

    [Test]
    public void SignalDeserializationFinished ()
    {
      var serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddValue (1);
      serializationInfo.AddValue (2);
      serializationInfo.AddValue ("three");

      bool deserializationFinishedCalled = false;
      object deserializationFinishedSender = null;

      var deserializationInfo = new FlattenedDeserializationInfo (serializationInfo.GetData ());
      deserializationInfo.DeserializationFinished += (sender, args) => { deserializationFinishedCalled = true; deserializationFinishedSender = sender; };

      var o = deserializationInfo.GetValue<object> ();
      Assert.That (o, Is.EqualTo (1));

      o = deserializationInfo.GetValue<object> ();
      Assert.That (o, Is.EqualTo (2));

      o = deserializationInfo.GetValue<object> ();
      Assert.That (o, Is.EqualTo ("three"));
      Assert.That (deserializationFinishedCalled, Is.False);

      deserializationInfo.SignalDeserializationFinished ();

      Assert.That (deserializationFinishedCalled, Is.True);
      Assert.That (deserializationFinishedSender, Is.SameAs (deserializationInfo));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot signal DeserializationFinished when there is still integer data"
        + " left to deserialize.")]
    public void SignalDeserializationFinished_BeforeIntStreamFinished ()
    {
      var serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddIntValue (1);

      var deserializationInfo = new FlattenedDeserializationInfo (serializationInfo.GetData ());
      deserializationInfo.SignalDeserializationFinished ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot signal DeserializationFinished when there is still boolean data"
        + " left to deserialize.")]
    public void SignalDeserializationFinished_BeforeBoolStreamFinished ()
    {
      var serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddBoolValue (true);

      var deserializationInfo = new FlattenedDeserializationInfo (serializationInfo.GetData ());
      deserializationInfo.SignalDeserializationFinished ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot signal DeserializationFinished when there is still object data"
        + " left to deserialize.")]
    public void SignalDeserializationFinished_BeforeObjectStreamFinished ()
    {
      var serializationInfo = new FlattenedSerializationInfo ();
      serializationInfo.AddValue (DateTime.Now);

      var deserializationInfo = new FlattenedDeserializationInfo (serializationInfo.GetData ());
      deserializationInfo.SignalDeserializationFinished ();
    }
  }
}
