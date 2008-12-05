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
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Development.UnitTests.Core.UnitTesting
{
  [TestFixture]
  public class SerializerTest
  {
    [Test]
    public void SerializeAndDeserialize()
    {
      int[] array = new int[] {1, 2, 3};
      int[] array2 = Serializer.SerializeAndDeserialize (array);
      Assert.AreNotSame (array, array2);

      Assert.AreEqual (array.Length, array2.Length);
      Assert.AreEqual (array[0], array2[0]);
      Assert.AreEqual (array[1], array2[1]);
      Assert.AreEqual (array[2], array2[2]);
    }

    [Test]
    public void XmlSerialize ()
    {
      int[] array = new int[] {1, 2, 3};
      byte[] serializedArray = Serializer.XmlSerialize (array);
      string serializedArrayString = Encoding.UTF8.GetString (serializedArray);

      Assert.AreEqual (GetExpectedXmlString(), serializedArrayString);
    }

    private string GetExpectedXmlString ()
    {
      return @"<?xml version=""1.0""?>
<ArrayOfInt xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <int>1</int>
  <int>2</int>
  <int>3</int>
</ArrayOfInt>";
    }

    [Test]
    public void XmlDeserialize ()
    {
      byte[] serializedArray = Encoding.UTF8.GetBytes (GetExpectedXmlString());
      int[] array = Serializer.XmlDeserialize<int[]> (serializedArray);
      Assert.That (array, Is.EqualTo (new int[] { 1, 2, 3 }));
    }

    [Test]
    public void XmlSerializeAndDeserialize ()
    {
      string[] array = Serializer.XmlSerializeAndDeserialize (new string[] { "1", "2", "3" });
      Assert.That (array, Is.EqualTo (new string[] { "1", "2", "3" }));
    }
  }
}
