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
using System.Collections.Specialized;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{

[TestFixture]
public class NameValueCollectionUtilityTest
{
  // types

  // static members and constants

  // member fields

  private NameValueCollection _collection;
  private NameValueCollection _otherCollection;

  // construction and disposing

  public NameValueCollectionUtilityTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp()
  { 
    _collection = new NameValueCollection();
    _collection.Add ("FirstKey", "FirstValue");
    _collection.Add ("SecondKey", "SecondValue");
    _collection.Add ("ThirdKey", "ThirdValue");
    _collection.Add ("ThirdKey", "Other ThirdValue");

    _otherCollection = new NameValueCollection();
    _otherCollection.Add ("SecondKey", "Other SecondValue");
    _otherCollection.Add ("FourthKey", "FourthValue");
    _otherCollection.Add ("FifthKey", "FifthValue");
  }

  [Test]
  public void Clone()
  {
    NameValueCollection actual = NameValueCollectionUtility.Clone (_collection);
    
    Assert.IsNotNull (actual);
    Assert.IsFalse (ReferenceEquals (_collection, actual));
    Assert.AreEqual (3, actual.Count);

    Assert.AreEqual ("FirstKey", actual.GetKey (0));
    Assert.AreEqual ("SecondKey", actual.GetKey (1));
    Assert.AreEqual ("ThirdKey", actual.GetKey (2));

    Assert.AreEqual ("FirstValue", actual["FirstKey"]);
    Assert.AreEqual ("SecondValue", actual["SecondKey"]);
    Assert.AreEqual ("ThirdValue,Other ThirdValue", actual["ThirdKey"]);
  }

  [Test]
  public void AppendWithNull()
  {
    NameValueCollectionUtility.Append (_collection, null);

    Assert.AreEqual (3, _collection.Count);

    Assert.AreEqual ("FirstKey", _collection.GetKey (0));
    Assert.AreEqual ("SecondKey", _collection.GetKey (1));
    Assert.AreEqual ("ThirdKey", _collection.GetKey (2));

    Assert.AreEqual ("FirstValue", _collection["FirstKey"]);
    Assert.AreEqual ("SecondValue", _collection["SecondKey"]);
    Assert.AreEqual ("ThirdValue,Other ThirdValue", _collection["ThirdKey"]);
  } 

  [Test]
  public void AppendWithEmptyCollection()
  {
    NameValueCollectionUtility.Append (_collection, new NameValueCollection());

    Assert.AreEqual (3, _collection.Count);

    Assert.AreEqual ("FirstKey", _collection.GetKey (0));
    Assert.AreEqual ("SecondKey", _collection.GetKey (1));
    Assert.AreEqual ("ThirdKey", _collection.GetKey (2));

    Assert.AreEqual ("FirstValue", _collection["FirstKey"]);
    Assert.AreEqual ("SecondValue", _collection["SecondKey"]);
    Assert.AreEqual ("ThirdValue,Other ThirdValue", _collection["ThirdKey"]);
  } 

  [Test]
  public void AppendWithOtherCollection()
  {
    NameValueCollectionUtility.Append (_collection, _otherCollection);

    Assert.AreEqual (5, _collection.Count);

    Assert.AreEqual ("FirstKey", _collection.GetKey (0));
    Assert.AreEqual ("SecondKey", _collection.GetKey (1));
    Assert.AreEqual ("ThirdKey", _collection.GetKey (2));
    Assert.AreEqual ("FourthKey", _collection.GetKey (3));
    Assert.AreEqual ("FifthKey", _collection.GetKey (4));

    Assert.AreEqual ("FirstValue", _collection["FirstKey"]);
    Assert.AreEqual ("Other SecondValue", _collection["SecondKey"]);
    Assert.AreEqual ("ThirdValue,Other ThirdValue", _collection["ThirdKey"]);
    Assert.AreEqual ("FourthValue", _collection["FourthKey"]);
    Assert.AreEqual ("FifthValue", _collection["FifthKey"]);
  } 

  [Test]
  public void MergeWithFirstNullAndSecondNull()
  {
    Assert.IsNull (NameValueCollectionUtility.Merge (null, null));
  }

  [Test]
  public void MergeWithFirstValueAndSecondNull()
  {
    NameValueCollection actual = NameValueCollectionUtility.Merge (_collection, null);
    
    Assert.IsNotNull (actual);
    Assert.IsFalse (ReferenceEquals (_collection, actual));
    Assert.AreEqual (3, actual.Count);

    Assert.AreEqual ("FirstKey", actual.GetKey (0));
    Assert.AreEqual ("SecondKey", actual.GetKey (1));
    Assert.AreEqual ("ThirdKey", actual.GetKey (2));

    Assert.AreEqual ("FirstValue", actual["FirstKey"]);
    Assert.AreEqual ("SecondValue", actual["SecondKey"]);
    Assert.AreEqual ("ThirdValue,Other ThirdValue", actual["ThirdKey"]);
  }

  [Test]
  public void MergeWithFirstNullAndSecondValue()
  {
    NameValueCollection actual = NameValueCollectionUtility.Merge (null, _collection);
    
    Assert.IsNotNull (actual);
    Assert.IsFalse (ReferenceEquals (_collection, actual));
    Assert.AreEqual (3, actual.Count);

    Assert.AreEqual ("FirstKey", actual.GetKey (0));
    Assert.AreEqual ("SecondKey", actual.GetKey (1));
    Assert.AreEqual ("ThirdKey", actual.GetKey (2));

    Assert.AreEqual ("FirstValue", actual["FirstKey"]);
    Assert.AreEqual ("SecondValue", actual["SecondKey"]);
    Assert.AreEqual ("ThirdValue,Other ThirdValue", actual["ThirdKey"]);
  }

  [Test]
  public void MergeWithFirstValueAndSecondValue()
  {
    NameValueCollection actual = NameValueCollectionUtility.Merge (_collection, _otherCollection);

    Assert.IsNotNull (actual);
    Assert.IsFalse (ReferenceEquals (_collection, actual));
    Assert.AreEqual (5, actual.Count);

    Assert.AreEqual ("FirstKey", actual.GetKey (0));
    Assert.AreEqual ("SecondKey", actual.GetKey (1));
    Assert.AreEqual ("ThirdKey", actual.GetKey (2));
    Assert.AreEqual ("FourthKey", actual.GetKey (3));
    Assert.AreEqual ("FifthKey", actual.GetKey (4));

    Assert.AreEqual ("FirstValue", actual["FirstKey"]);
    Assert.AreEqual ("Other SecondValue", actual["SecondKey"]);
    Assert.AreEqual ("ThirdValue,Other ThirdValue", actual["ThirdKey"]);
    Assert.AreEqual ("FourthValue", actual["FourthKey"]);
    Assert.AreEqual ("FifthValue", actual["FifthKey"]);
  } 
}

}
