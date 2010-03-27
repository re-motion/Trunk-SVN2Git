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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class DataContainerLoaderHelperTest : SqlProviderBaseTest
  {
    private DataContainerLoaderHelper _loaderHelper;

    public override void SetUp ()
    {
      base.SetUp ();
      _loaderHelper = new DataContainerLoaderHelper ();
    }

    [Test]
    public void GetCommandBuilderForRelatedIDLookup ()
    {
      Provider.Connect ();
      
      var orderDefinition = DomainObjectIDs.Order1.ClassDefinition;
      var relationProperty = orderDefinition.GetMandatoryPropertyDefinition (typeof (Order).FullName + ".Customer");
      
      var builder = (SingleIDLookupCommandBuilder) _loaderHelper.GetCommandBuilderForRelatedIDLookup (Provider, "Order", relationProperty, DomainObjectIDs.Customer1);
      Assert.That (builder.EntityName, Is.EqualTo ("Order"));
      Assert.That (builder.OrderExpression, Is.EqualTo ("OrderNo asc"));
      Assert.That (builder.SelectColumns, Is.EqualTo ("*"));
      Assert.That (builder.CheckedColumnName, Is.EqualTo ("CustomerID"));
      Assert.That (builder.ExpectedValue, Is.EqualTo (DomainObjectIDs.Customer1));
    }

    [Test]
    public void GetCommandBuilderForIDLookup_SingleID ()
    {
      Provider.Connect ();

      var builder = (SingleIDLookupCommandBuilder) _loaderHelper.GetCommandBuilderForIDLookup (Provider, "Order", new[] { DomainObjectIDs.Customer1 });

      Assert.That (builder.EntityName, Is.EqualTo ("Order"));
      Assert.That (builder.OrderExpression, Is.Null);
      Assert.That (builder.SelectColumns, Is.EqualTo ("*"));
      Assert.That (builder.CheckedColumnName, Is.EqualTo ("ID"));
      Assert.That (builder.ExpectedValue, Is.EqualTo (DomainObjectIDs.Customer1));
    }

    [Test]
    public void GetCommandBuilderForIDLookup_MultipleIDs ()
    {
      Provider.Connect ();

      var builder = (MultiIDLookupCommandBuilder) _loaderHelper.GetCommandBuilderForIDLookup (
          Provider, "Order", new[] { DomainObjectIDs.Customer1, DomainObjectIDs.Customer2 });

      Assert.That (builder.EntityName, Is.EqualTo ("Order"));
      Assert.That (builder.SelectColumns, Is.EqualTo ("*"));
      Assert.That (builder.CheckedColumnName, Is.EqualTo ("ID"));
      Assert.That (builder.CheckedColumnTypeName, Is.EqualTo ("uniqueidentifier"));
      Assert.That (builder.ExpectedValues, Is.EqualTo (new[] { DomainObjectIDs.Customer1, DomainObjectIDs.Customer2 }));
    }
  }
}