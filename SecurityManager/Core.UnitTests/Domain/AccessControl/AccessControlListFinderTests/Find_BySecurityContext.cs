using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlListFinderTests
{
  [TestFixture]
  public class Find_BySecurityContext : DomainTest
  {
    private SecurableClassDefinition _currentClassDefinition;
    private ClientTransaction _currentClassDefinitionTransaction;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
 
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      _currentClassDefinitionTransaction = ClientTransaction.NewRootTransaction ();
      _currentClassDefinition = dbFixtures.CreateAndCommitSecurableClassDefinitionWithAccessControlLists (1, _currentClassDefinitionTransaction);
    }

    public override void SetUp ()
    {
      base.SetUp ();
      _currentClassDefinitionTransaction.EnterNonDiscardingScope();
    }

    [Test]
    public void Succeed_WithValidSecurityContext ()
    {
      AccessControlList expectedAccessControlList;
      using (_currentClassDefinitionTransaction.EnterNonDiscardingScope ())
      {
        expectedAccessControlList = _currentClassDefinition.AccessControlLists[0];
      }
      SecurityContext context = new SecurityContext (typeof (Order));
     
      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      AccessControlList foundAcl = aclFinder.Find (ClientTransaction.NewRootTransaction (), context);

      Assert.AreEqual (expectedAccessControlList.ID, foundAcl.ID);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException),
        ExpectedMessage = "The securable class 'Remotion.SecurityManager.UnitTests.TestDomain.PremiumOrder, Remotion.SecurityManager.UnitTests' cannot be found.")]
    public void Fail_WithUnkownSecurableClassDefinition ()
    {
      SecurityContext context = new SecurityContext (typeof (PremiumOrder));

      AccessControlListFinder aclFinder = new AccessControlListFinder ();
      aclFinder.Find (ClientTransaction.NewRootTransaction (), context);
    }
  }
}
