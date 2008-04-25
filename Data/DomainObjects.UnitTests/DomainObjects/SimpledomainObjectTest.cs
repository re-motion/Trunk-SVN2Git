using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
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