/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Queries
{
  [TestFixture]
  public class QueryParameterCollectionTest : StandardMappingTest
  {
    private QueryParameterCollection _collection;
    private QueryParameter _parameter;

    public override void SetUp ()
    {
      base.SetUp ();

      _parameter = new QueryParameter ("name", "value");
      _collection = new QueryParameterCollection ();
    }

    [Test]
    public void Add ()
    {
      _collection.Add (_parameter);
      Assert.AreEqual (1, _collection.Count);
    }

    [Test]
    public void QueryParameterIndexer ()
    {
      _collection.Add (_parameter);
      Assert.AreSame (_parameter, _collection[_parameter.Name]);
    }

    [Test]
    public void NumericIndexer ()
    {
      _collection.Add (_parameter);
      Assert.AreSame (_parameter, _collection[0]);
    }

    [Test]
    public void ContainsParameterNameTrue ()
    {
      _collection.Add (_parameter);
      Assert.IsTrue (_collection.Contains (_parameter.Name));
    }

    [Test]
    public void ContainsParameterNameFalse ()
    {
      Assert.IsFalse (_collection.Contains (_parameter.Name));
    }

    [Test]
    public void CopyConstructor ()
    {
      _collection.Add (_parameter);

      QueryParameterCollection copiedCollection = new QueryParameterCollection (_collection, false);

      Assert.AreEqual (1, copiedCollection.Count);
      Assert.AreSame (_parameter, copiedCollection[0]);
    }

    [Test]
    public void ContainsParameterTrue ()
    {
      _collection.Add (_parameter);
      Assert.IsTrue (_collection.Contains (_parameter));
    }

    [Test]
    public void ContainsParameterFalse ()
    {
      _collection.Add (_parameter);
      QueryParameter copy = new QueryParameter (_parameter.Name, _parameter.Value, _parameter.ParameterType);
      Assert.IsFalse (_collection.Contains (copy));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsNullQueryParameter ()
    {
      _collection.Contains ((QueryParameter) null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsNullQueryParameterName ()
    {
      _collection.Contains ((string) null);
    }

    [Test]
    public void AddShorthand1 ()
    {
      _collection.Add (_parameter.Name, _parameter.Value, _parameter.ParameterType);
      Assert.AreEqual (1, _collection.Count);
      Assert.AreEqual (_parameter.Name, _collection[0].Name);
      Assert.AreEqual (_parameter.Value, _collection[0].Value);
      Assert.AreEqual (_parameter.ParameterType, _collection[0].ParameterType);
    }

    [Test]
    public void AddShorthand2 ()
    {
      _collection.Add (_parameter.Name, _parameter.Value);
      Assert.AreEqual (1, _collection.Count);
      Assert.AreEqual (_parameter.Name, _collection[0].Name);
      Assert.AreEqual (_parameter.Value, _collection[0].Value);
      Assert.AreEqual (QueryParameterType.Value, _collection[0].ParameterType);
    }
  }
}
