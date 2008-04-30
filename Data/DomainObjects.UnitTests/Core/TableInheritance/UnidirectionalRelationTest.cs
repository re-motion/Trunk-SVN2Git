using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance
{
  [TestFixture]
  public class UnidirectionalRelationTest : TableInheritanceMappingTest
  {
    [Test]
    [Ignore ("TODO: Implement referential integrity for unidirectional relationships.")]
    public void DeleteAndCommitWithConcreteTableInheritance()
    {
      SetDatabaseModifyable ();

      ClassWithUnidirectionalRelation classWithUnidirectionalRelation =
          ClassWithUnidirectionalRelation.GetObject (DomainObjectIDs.ClassWithUnidirectionalRelation);
      classWithUnidirectionalRelation.DomainBase.Delete ();
      ClientTransactionScope.CurrentTransaction.Commit ();

      try
      {
        Dev.Null = classWithUnidirectionalRelation.DomainBase;
        Assert.Fail ("Expected ObjectDiscardedException");
      }
      catch (ObjectDiscardedException)
      {
        // succeed
      }

      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
      {
        ClassWithUnidirectionalRelation reloadedObject =
            ClassWithUnidirectionalRelation.GetObject (DomainObjectIDs.ClassWithUnidirectionalRelation);

        Assert.IsNull (reloadedObject.DomainBase);
      }
    }
  }
}