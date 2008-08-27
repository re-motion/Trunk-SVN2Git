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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting.ObjectMother;
using List = Remotion.Development.UnitTesting.ObjectMother.List;

namespace Remotion.Development.UnitTests
{
  [NUnit.Framework.TestFixture]
  public class NewTest
  {
    [Test]
    public void NewListTest ()
    {
      var collection = List.New (7, 11, 13, 17);
      var collectionExpected = new List<int> { 7, 11, 13, 17 };
      Assert.That (collection, Is.EqualTo (collectionExpected));
    }

    [Test]
    public void NewQueueTest ()
    {
      var collection = Queue.New (7, 11, 13, 17);
      var collectionExpected = new Queue<int>();
      collectionExpected.Enqueue (7);
      collectionExpected.Enqueue (11);
      collectionExpected.Enqueue (13);
      collectionExpected.Enqueue (17);
      Assert.That (collection, Is.EqualTo (collectionExpected));
    }


    [Test]
    public void NewDictionary1Test ()
    {
      var collection = Dictionary.New ("B", 2);
      var collectionExpected = new Dictionary<string, int>();
      collectionExpected["B"] = 2;
      Assert.That (collection, Is.EquivalentTo (collectionExpected));
    }

    [Test]
    public void NewDictionary2Test ()
    {
      var collection = Dictionary.New ("B", 2, "D", 4);
      var collectionExpected = new Dictionary<string, int>();
      collectionExpected["B"] = 2;
      collectionExpected["D"] = 4;
      Assert.That (collection, Is.EquivalentTo (collectionExpected));
    }

    [Test]
    public void NewDictionary3Test ()
    {
      var collection = Dictionary.New ("B", 2, "D", 4, "C", 3);
      var collectionExpected = new Dictionary<string, int>();
      collectionExpected["B"] = 2;
      collectionExpected["C"] = 3;
      collectionExpected["D"] = 4;
      Assert.That (collection, Is.EquivalentTo (collectionExpected));
    }

    [Test]
    public void NewDictionaryTest4 ()
    {
      var collection = Dictionary.New ("B", 2, "D", 4, "C", 3, "A", 1);
      //var collectionExpected = new Queue<int> { 7, 11, 13, 17 };
      var collectionExpected = new Dictionary<string, int>();
      collectionExpected["A"] = 1;
      collectionExpected["B"] = 2;
      collectionExpected["C"] = 3;
      collectionExpected["D"] = 4;
      Assert.That (collection, Is.EquivalentTo (collectionExpected));
    }


  }
}