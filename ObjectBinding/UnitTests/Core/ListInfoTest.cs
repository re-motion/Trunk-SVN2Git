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
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core
{
  [TestFixture]
  public class ListInfoTest
  {
    [Test]
    public void GetPropertyType ()
    {
      ListInfo listInfo = new ListInfo (typeof (string[]), typeof (string));
      Assert.That (listInfo.PropertyType, Is.SameAs (typeof (string[])));
    }

    [Test]
    public void GetItemType ()
    {
      IListInfo listInfo = new ListInfo (typeof (string[]), typeof (string));
      Assert.That (listInfo.ItemType, Is.SameAs (typeof (string)));
    }

    [Test]
    public void GetRequiresWriteBack_WithArray ()
    {
      IListInfo listInfo = new ListInfo (typeof (string[]), typeof (string));
      Assert.That (listInfo.RequiresWriteBack, Is.True);
    }

    [Test]
    public void GetRequiresWriteBack_WithIList ()
    {
      IListInfo listInfo = new ListInfo (typeof (IList), typeof (string));
      Assert.That (listInfo.RequiresWriteBack, Is.False);
    }

    [Test]
    public void CreateList_ReferenceType ()
    {
      IListInfo listInfo = new ListInfo (typeof (SimpleReferenceType[]), typeof (SimpleReferenceType));
      Assert.That (listInfo.CreateList (1), Is.EquivalentTo (new SimpleReferenceType[1]));
    }

    [Test]
    public void CreateList_ValueType ()
    {
      IListInfo listInfo = new ListInfo (typeof (SimpleValueType[]), typeof (SimpleValueType));
      Assert.That (listInfo.CreateList (1), Is.EquivalentTo (new SimpleValueType[1]));
    }

    [Test]
    public void CreateList_NullableValueType ()
    {
      IListInfo listInfo = new ListInfo (typeof (SimpleValueType?[]), typeof (SimpleValueType?));
      Assert.That (listInfo.CreateList (1), Is.EquivalentTo (new SimpleValueType?[1]));
    }

    [Test]
    [Ignore ("TODO: implement and integrate with BocList")]
    public void InsertItem ()
    {      
    }

    [Test]
    [Ignore ("TODO: implement and integrate with BocList")]
    public void RemoveItem ()
    {
    }

    [Test]
    [Ignore ("TODO: test")]
    public void Initialize_WithMismatchedItemType ()
    {
    }
  }
}
