// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  [TestFixture]
  public class UnionSelectDbCommandBuilderTest : SqlProviderBaseTest
  {
    private UnionSelectDbCommandBuilder _builder;
    
    public override void SetUp ()
    {
      base.SetUp();

      var domainBaseClass = MappingConfiguration.Current.GetTypeDefinition (typeof (DomainBase));
      _builder = UnionSelectDbCommandBuilder.CreateForRelatedIDLookup (
          Provider,
          StorageNameProvider,
          domainBaseClass,
          domainBaseClass.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.DomainBase.Client"),
          DomainObjectIDs.Client,
          Provider.SqlDialect,
          Provider,
          Provider.StorageProviderDefinition);
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
      var domainBaseClass = ClassDefinitionFactory.CreateClassDefinition ("DomainBase", null, StorageProviderDefinition, typeof (DomainBase), false);
      var personClass = ClassDefinitionFactory.CreateClassDefinition (
          "Person", "TableInheritance_Person", StorageProviderDefinition, typeof (Person), false, domainBaseClass);
      var organizationalUnitClass = ClassDefinitionFactory.CreateClassDefinition (
          "OrganizationalUnit", "TableInheritance_OrganizationalUnit", StorageProviderDefinition, typeof (OrganizationalUnit), false, domainBaseClass);
      domainBaseClass.SetDerivedClasses (new[] { personClass, organizationalUnitClass });

      var clientClass = ClassDefinitionFactory.CreateClassDefinition (
          "Client", "TableInheritance_Client", StorageProviderDefinition, typeof (Client), false);
      var clientClassPropertyDefinition = PropertyDefinitionFactory.Create (
          domainBaseClass, typeof (DomainBase), "Client", "ClientID");
      domainBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { clientClassPropertyDefinition }, true));
      var propertyDefinition = domainBaseClass["Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.DomainBase.Client"];

      var domainBaseEndPointDefinition = new RelationEndPointDefinition (propertyDefinition, false);

      var clientEndPointDefinition = VirtualRelationEndPointDefinitionFactory.Create (
              clientClass, "AssignedObjects", false, CardinalityType.Many, typeof (DomainObjectCollection));

      var clientToDomainBaseRelationDefinition = new RelationDefinition ("ClientToDomainBase", clientEndPointDefinition, domainBaseEndPointDefinition);
      clientEndPointDefinition.SetRelationDefinition (clientToDomainBaseRelationDefinition);
      domainBaseEndPointDefinition.SetRelationDefinition (clientToDomainBaseRelationDefinition);

      domainBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { domainBaseEndPointDefinition }, true));
      clientClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { clientEndPointDefinition }, true));

      domainBaseClass.SetReadOnly();
      personClass.SetReadOnly();
      organizationalUnitClass.SetReadOnly();
      clientClass.SetReadOnly();

      UnionSelectDbCommandBuilder builder = UnionSelectDbCommandBuilder.CreateForRelatedIDLookup (
          Provider,
          StorageNameProvider,
          domainBaseClass,
          domainBaseClass.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.DomainBase.Client"),
          DomainObjectIDs.Client,
          Provider.SqlDialect,
          Provider,
          Provider.StorageProviderDefinition);

      using (IDbCommand command = builder.Create())
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
      using (IDbCommand command = _builder.Create())
      {
        string expectedCommandText =
            "SELECT [ID], [ClassID], [CreatedAt] FROM [TableInheritance_Person] WHERE [ClientID] = @ClientID\n"
            +
            "UNION ALL SELECT [ID], [ClassID], [CreatedAt] FROM [TableInheritance_OrganizationalUnit] WHERE [ClientID] = @ClientID ORDER BY [CreatedAt] ASC;";

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
      using (
          MixinConfiguration.BuildFromActive().ForClass (typeof (WhereClauseBuilder)).Clear().AddMixins (typeof (WhereClauseBuilderMixin)).EnterScope(
              ))
      {
        using (IDbCommand command = _builder.Create())
        {
          Assert.IsTrue (command.CommandText.Contains ("Mixed!"));
        }
      }
    }
  }
}