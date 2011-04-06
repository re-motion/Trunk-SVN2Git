using System;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Synchronization
{
  public class RelationInconsistenciesTestBase : ClientTransactionBaseTest
  {
    protected ObjectID CreateCompanyAndSetIndustrialSectorInOtherTransaction (ObjectID industrialSectorID)
    {
      return DomainObjectMother.CreateObjectAndSetRelationInOtherTransaction<Company, IndustrialSector> (industrialSectorID, (c, s) =>
      {
        c.IndustrialSector = s;
        c.Ceo = Ceo.NewObject();
      });
    }

    protected void SetIndustrialSectorInOtherTransaction (ObjectID companyID, ObjectID industrialSectorID)
    {
      DomainObjectMother.SetRelationInOtherTransaction<Company, IndustrialSector> (companyID, industrialSectorID, (c, s) => c.IndustrialSector = s);
    }

    protected ObjectID CreateComputerAndSetEmployeeInOtherTransaction (ObjectID employeeID)
    {
      return DomainObjectMother.CreateObjectAndSetRelationInOtherTransaction<Computer, Employee> (employeeID, (c, e) => c.Employee = e);
    }

    protected void SetEmployeeInOtherTransaction (ObjectID computerID, ObjectID employeeID)
    {
      DomainObjectMother.SetRelationInOtherTransaction<Computer, Employee> (computerID, employeeID, (c, e) => c.Employee = e);
    }

    protected void CheckSyncState<TOriginating, TRelated> (
        TOriginating originating,
        Expression<Func<TOriginating, TRelated>> propertyAccessExpression,
        bool expectedState)
        where TOriginating: DomainObject
    {
      Assert.That (
          BidirectionalRelationSyncService.IsSynchronized (ClientTransaction.Current, RelationEndPointID.Create (originating, propertyAccessExpression)),
          Is.EqualTo (expectedState));
    }

    protected void CheckActionWorks (Action action)
    {
      action ();
    }

    protected void CheckActionThrows<TException> (Action action, string expectedMessage) where TException : Exception
    {
      var hadException = false;
      try
      {
        action ();
      }
      catch (Exception ex)
      {
        hadException = true;
        Assert.That (ex, Is.TypeOf (typeof (TException)));
        Assert.That (
            ex.Message, 
            Is.StringContaining(expectedMessage), 
            "Expected: " + expectedMessage + Environment.NewLine + "Was: " + ex.Message);
      }

      if (!hadException)
        Assert.Fail ("Expected " + typeof (TException).Name);
    }

    protected Company CreateCompanyInDatabaseAndLoad ()
    {
      ObjectID objectID;
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var company = Company.NewObject ();
        company.Ceo = Ceo.NewObject ();
        ClientTransaction.Current.Commit ();
        objectID = company.ID;
      }
      return Company.GetObject (objectID);
    }

    protected IndustrialSector CreateIndustrialSectorInDatabaseAndLoad ()
    {
      ObjectID objectID;
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        IndustrialSector industrialSector = IndustrialSector.NewObject ();
        Company oldCompany = Company.NewObject ();
        oldCompany.Ceo = Ceo.NewObject ();
        industrialSector.Companies.Add (oldCompany);
        objectID = industrialSector.ID;

        ClientTransaction.Current.Commit ();
      }
      return IndustrialSector.GetObject (objectID);
    }

    protected T CreateObjectInDatabaseAndLoad<T> () where T : DomainObject
    {
      ObjectID objectID;
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var domainObject = LifetimeService.NewObject (ClientTransaction.Current, typeof (T), ParamList.Empty);
        ClientTransaction.Current.Commit ();
        objectID = domainObject.ID;
      }
      return (T) LifetimeService.GetObject (ClientTransaction.Current, objectID, false);
    }

    protected void PrepareInconsistentState_OneMany_ObjectIncluded (out Company company, out IndustrialSector industrialSector)
    {
      SetDatabaseModifyable ();

      company = CreateCompanyInDatabaseAndLoad ();
      Assert.That (company.IndustrialSector, Is.Null);

      industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);

      SetIndustrialSectorInOtherTransaction (company.ID, industrialSector.ID);

      // Resolve virtual end point - the database says that company points to industrialSector, but the transaction says it points to null!
      industrialSector.Companies.EnsureDataComplete ();

      Assert.That (company.IndustrialSector, Is.Null);
      Assert.That (industrialSector.Companies, Has.Member(company));

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);
    }

    protected void PrepareInconsistentState_OneMany_ObjectNotIncluded (out Company company, out IndustrialSector industrialSector)
    {
      SetDatabaseModifyable ();

      var companyID = CreateCompanyAndSetIndustrialSectorInOtherTransaction (DomainObjectIDs.IndustrialSector1);
      company = Company.GetObject (companyID);

      Assert.That (company.Properties[typeof (Company), "IndustrialSector"].GetRelatedObjectID (), Is.EqualTo (DomainObjectIDs.IndustrialSector1));

      SetIndustrialSectorInOtherTransaction (company.ID, DomainObjectIDs.IndustrialSector2);

      industrialSector = IndustrialSector.GetObject (DomainObjectIDs.IndustrialSector1);

      // Resolve virtual end point - the database says that company does not point to IndustrialSector1, but the transaction says it does!
      industrialSector.Companies.EnsureDataComplete ();

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies, Has.No.Member (company));
      CheckSyncState (company, c => c.IndustrialSector, false);
      CheckSyncState (industrialSector, s => s.Companies, true);
    }
  }
}