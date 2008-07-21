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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.DomainObjects
{
  [TestFixture]
  public class SimpleDomainObjectTest : ClientTransactionBaseTest
  {
    [Test]
    public void NewObject ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject().With();
      Assert.IsNotNull (instance);
      Assert.AreEqual (0, instance.IntProperty);
      instance.IntProperty = 5;
      Assert.AreEqual (5, instance.IntProperty);
    }

    [Test]
    public void GetObject_WithoutDeleted ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject ().With ();
      instance.IntProperty = 7;
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClassDerivedFromSimpleDomainObject gottenInstance = ClassDerivedFromSimpleDomainObject.GetObject (instance.ID);
        Assert.AreSame (instance, gottenInstance);
        Assert.AreEqual (7, gottenInstance.IntProperty);
      }
    }

    [Test]
    public void GetObject_IncludeDeletedFalse_NonDeleted ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject ().With ();
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClassDerivedFromSimpleDomainObject gottenInstance = ClassDerivedFromSimpleDomainObject.GetObject (instance.ID, false);
        Assert.AreSame (instance, gottenInstance);
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException), ExpectedMessage = "Object '.*' is already deleted.", MatchType = MessageMatch.Regex)]
    public void GetObject_IncludeDeletedFalse_Deleted ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject ().With ();
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        instance.Delete();
        ClassDerivedFromSimpleDomainObject.GetObject (instance.ID, false);
      }
    }

    [Test]
    public void GetObject_IncludeDeletedTrue_NonDeleted ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject ().With ();
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClassDerivedFromSimpleDomainObject gottenInstance = ClassDerivedFromSimpleDomainObject.GetObject (instance.ID, true);
        Assert.AreSame (instance, gottenInstance);
      }
    }

    [Test]
    public void GetObject_IncludeDeletedTrue_Deleted ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject ().With ();
      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        instance.Delete ();
        ClassDerivedFromSimpleDomainObject gottenInstance = ClassDerivedFromSimpleDomainObject.GetObject (instance.ID, true);
        Assert.AreSame (instance, gottenInstance);
        Assert.AreEqual (StateType.Deleted, gottenInstance.State);
      }
    }
  }
}
