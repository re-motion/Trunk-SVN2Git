using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.Domain;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.AclTools
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private DatabaseFixtures _dbFixtures;
    private static ObjectID s_orderClassID;

    public static ObjectID OrderClassID
    {
      get { return s_orderClassID; }
    }

    static public List<AccessControlList> aclList { get; private set; }


    [SetUp]
    public void SetUp ()
    {
      try
      {
        AccessControlTestHelper accessControlTestHelper = new AccessControlTestHelper();
        using (accessControlTestHelper.Transaction.EnterDiscardingScope())
        {
          _dbFixtures = new DatabaseFixtures();
          _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.Current);

          SecurableClassDefinition orderClass = accessControlTestHelper.CreateOrderClassDefinition();
          accessControlTestHelper.AttachAccessType (orderClass, Guid.NewGuid(), "FirstAccessType", 0);
          accessControlTestHelper.AttachAccessType (orderClass, Guid.NewGuid(), "FirstAccessType2", 2);
          accessControlTestHelper.AttachAccessType (orderClass, Guid.NewGuid(), "FirstAccessType3", 3);
          aclList = accessControlTestHelper.CreateAclsForOrderAndPaymentAndDeliveryStates (orderClass);
          var ace = aclList[0].CreateAccessControlEntry ();
          ace.Permissions[0].Allowed = true; // FirstAccessType
          s_orderClassID = orderClass.ID;

          ClientTransaction.Current.Commit();
        }
      }
      catch (Exception e)
      {
        Console.WriteLine (e);
        throw;
      }
    }

  }
}