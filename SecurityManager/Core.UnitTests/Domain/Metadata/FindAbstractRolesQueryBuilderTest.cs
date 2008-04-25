using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class FindAbstractRolesQueryBuilderTest : DomainTest
  {
    [Test]
    public void CreateQuery_OneRole ()
    {
      FindAbstractRolesQueryBuilder queryBuilder = new FindAbstractRolesQueryBuilder ();
      Query query = queryBuilder.CreateQuery (new EnumWrapper[] { new EnumWrapper (ProjectRoles.QualityManager) });

      Assert.AreEqual (1, query.Parameters.Count);
      Assert.IsTrue (query.Parameters.Contains ("@p0"));
      Assert.AreEqual ("QualityManager|Remotion.SecurityManager.UnitTests.TestDomain.ProjectRoles, Remotion.SecurityManager.UnitTests", query.Parameters["@p0"].Value);
      Assert.AreEqual ("SELECT * FROM [AbstractRoleDefinitionView] WHERE [Name] = @p0", query.Statement);
    }
  }
}
