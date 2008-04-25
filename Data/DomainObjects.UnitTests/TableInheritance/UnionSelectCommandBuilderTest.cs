using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;
using Remotion.Mixins;
using Remotion.Mixins.Context;

namespace Remotion.Data.DomainObjects.UnitTests.TableInheritance
{
  [TestFixture]
  public class UnionSelectCommandBuilderTest : SqlProviderBaseTest
  {
    private UnionSelectCommandBuilder _builder;

    public override void SetUp ()
    {
      base.SetUp ();

      ClassDefinition domainBaseClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (DomainBase));

      _builder = UnionSelectCommandBuilder.CreateForRelatedIDLookup (
          Provider, domainBaseClass, domainBaseClass.GetMandatoryPropertyDefinition ("Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain.DomainBase.Client"), DomainObjectIDs.Client);
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_builder.Provider, Is.SameAs (Provider));
    }

    [Test]
    public void Create ()
    {
      // Note: This test builds its own relations without a sort expression.
      ReflectionBasedClassDefinition domainBaseClass = new ReflectionBasedClassDefinition ("DomainBase", null, TableInheritanceTestDomainProviderID, typeof (DomainBase), false, new List<Type> ());

      ReflectionBasedClassDefinition personClass = new ReflectionBasedClassDefinition (
          "Person", "TableInheritance_Person", TableInheritanceTestDomainProviderID, typeof (Person), false, domainBaseClass, new List<Type> ());

      ReflectionBasedClassDefinition organizationalUnitClass = new ReflectionBasedClassDefinition (
          "OrganizationalUnit", "TableInheritance_OrganizationalUnit", TableInheritanceTestDomainProviderID, typeof (OrganizationalUnit), false, domainBaseClass, new List<Type> ());

      ReflectionBasedClassDefinition clientClass = new ReflectionBasedClassDefinition ("Client", "TableInheritance_Client", TableInheritanceTestDomainProviderID, typeof (Client), false, new List<Type> ());

      domainBaseClass.MyPropertyDefinitions.Add (ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(domainBaseClass, "Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain.DomainBase.Client", "ClientID", typeof (ObjectID)));

      RelationEndPointDefinition domainBaseEndPointDefinition = new RelationEndPointDefinition (domainBaseClass, "Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain.DomainBase.Client", false);

      VirtualRelationEndPointDefinition clientEndPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(clientClass, "AssignedObjects", false, CardinalityType.Many, typeof (DomainObjectCollection));

      RelationDefinition clientToDomainBaseDefinition = new RelationDefinition (
          "ClientToDomainBase", clientEndPointDefinition, domainBaseEndPointDefinition);

      domainBaseClass.MyRelationDefinitions.Add (clientToDomainBaseDefinition);
      clientClass.MyRelationDefinitions.Add (clientToDomainBaseDefinition);

      UnionSelectCommandBuilder builder = UnionSelectCommandBuilder.CreateForRelatedIDLookup (
          Provider, domainBaseClass, domainBaseClass.GetMandatoryPropertyDefinition ("Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain.DomainBase.Client"), DomainObjectIDs.Client);

      using (IDbCommand command = builder.Create ())
      {
        string expectedCommandText = 
            "SELECT [ID], [ClassID] FROM [TableInheritance_Person] WHERE [ClientID] = @ClientID\n"
            + "UNION ALL SELECT [ID], [ClassID] FROM [TableInheritance_OrganizationalUnit] WHERE [ClientID] = @ClientID;";

        Assert.IsNotNull (command);
        Assert.AreEqual (expectedCommandText, command.CommandText);
        Assert.AreEqual (1, command.Parameters.Count);
        Assert.AreEqual ("@ClientID", ((SqlParameter) command.Parameters[0]).ParameterName);
        Assert.AreEqual (DomainObjectIDs.Client.Value, ((SqlParameter) command.Parameters[0]).Value);
      }
    }

    [Test]
    public void CreateWithSortExpression ()
    {
      using (IDbCommand command = _builder.Create ())
      {
        string expectedCommandText =
            "SELECT [ID], [ClassID], CreatedAt FROM [TableInheritance_Person] WHERE [ClientID] = @ClientID\n"
            + "UNION ALL SELECT [ID], [ClassID], CreatedAt FROM [TableInheritance_OrganizationalUnit] WHERE [ClientID] = @ClientID ORDER BY CreatedAt asc;";

        Assert.IsNotNull (command);
        Assert.AreEqual (expectedCommandText, command.CommandText);
        Assert.AreEqual (1, command.Parameters.Count);
        Assert.AreEqual ("@ClientID", ((SqlParameter) command.Parameters[0]).ParameterName);
        Assert.AreEqual (DomainObjectIDs.Client.Value, ((SqlParameter) command.Parameters[0]).Value);
      }
    }

    [Test]
    public void WhereClauseBuilder_CanBeMixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (WhereClauseBuilder)).Clear().AddMixins (typeof (WhereClauseBuilderMixin)).EnterScope())
      {
        using (IDbCommand command = _builder.Create())
        {
          Assert.IsTrue (command.CommandText.Contains ("Mixed!"));
        }
      }
    }
  }
}
