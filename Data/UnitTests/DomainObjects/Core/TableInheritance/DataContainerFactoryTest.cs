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
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  [TestFixture]
  public class DataContainerFactoryTest : SqlProviderBaseTest
  {
    [Test]
    public void RelationClassIDColumnRefersToAbstractClass ()
    {
      ObjectID id = new ObjectID (typeof (Order), new Guid ("{F404FD2C-B92F-46d8-BEAC-F92C0599BFD3}"));
      SelectCommandBuilder builder = SelectCommandBuilder.CreateForIDLookup (Provider, "*", "TableInheritance_Order", id);

      using (IDbCommand command = builder.Create ())
      {
        using (IDataReader reader = command.ExecuteReader ())
        {
          DataContainerFactory factory = new DataContainerFactory (Provider, reader);

          try
          {
            factory.CreateDataContainer ();
            Assert.Fail ("RdbmsProviderException was expected.");
          }
          catch (RdbmsProviderException ex)
          {
            string expectedBeginOfMessage = string.Format ("Error while reading property 'Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain.Order.Customer' of object '{0}':", id);
            Assert.IsTrue (ex.Message.StartsWith (expectedBeginOfMessage));
          }
        }
      }
    }
  }
}
