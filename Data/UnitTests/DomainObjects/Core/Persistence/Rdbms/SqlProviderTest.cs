// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Data.SqlClient;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderTest : SqlProviderBaseTest
  {
    [Test]
    public void IsConnectedFalse ()
    {
      RdbmsProvider rdbmsProvider = Provider;

      Assert.IsFalse (rdbmsProvider.IsConnected);
    }

    [Test]
    public void ConnectionHandling ()
    {
      RdbmsProvider rdbmsProvider = Provider;

      rdbmsProvider.Connect ();
      Assert.IsTrue (rdbmsProvider.IsConnected);
      rdbmsProvider.Disconnect ();
      Assert.IsFalse (rdbmsProvider.IsConnected);
    }

    [Test]
    public void Disposing ()
    {
      using (StorageProvider provider = Provider)
      {
        provider.LoadDataContainer (DomainObjectIDs.Order1);
      }

      RdbmsProvider rdbmsProvider = Provider;
      Assert.IsFalse (rdbmsProvider.IsConnected);
    }

    [Test]
    public void GetParameterName ()
    {
      Assert.AreEqual ("@parameter", Provider.GetParameterName ("parameter"));
      Assert.AreEqual ("@parameter", Provider.GetParameterName ("@parameter"));
    }

    [Test]
    public void ConnectionReturnsSqlConnection ()
    {
      // Note: If Provider.Connection returns a SqlConnection instead of IDbConnection, the line below does not create a compiler error.
      SqlConnection sqlConnection = Provider.Connection;
    }

    [Test]
    public void TransactionReturnsSqlTransaction ()
    {
      // Note: If Provider.Transaction returns a SqlTransaction instead of IDbTransaction, the line below does not create a compiler error.
      SqlTransaction sqlTransaction = Provider.Transaction;
    }

    [Test]
    [ExpectedException (typeof (ObjectDisposedException))]
    public void GetColumnsFromSortExpressionChecksForDisposal ()
    {
      Provider.Dispose ();
      Provider.GetColumnsFromSortExpression ("StorageSpecificName asc");
    }

    [Test]
    public void GetTypeConversionServices()
    {
      Assert.AreSame (ProviderDefinition.TypeConversionProvider, ((StorageProvider) Provider).TypeConversionProvider);
    }

    [Test]
    public void CreateValueConverter ()
    {
      ValueConverter valueConverter = Provider.CreateValueConverter();
      Assert.IsNotNull (valueConverter);
      Assert.AreSame (ProviderDefinition.TypeConversionProvider, valueConverter.TypeConversionProvider);
    }
  }
}
