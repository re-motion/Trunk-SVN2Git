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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class NullAnonymousEndPointTest : ClientTransactionBaseTest
  {
    private RelationDefinition _clientToLocationDefinition;
    private IRelationEndPointDefinition _clientEndPointDefinition;
    private IRelationEndPointDefinition _locationEndPointDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _clientToLocationDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Location)].GetRelationDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
      _clientEndPointDefinition = _clientToLocationDefinition.GetEndPointDefinition ("Client", null);
      _locationEndPointDefinition = _clientToLocationDefinition.GetEndPointDefinition ("Location", "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client");
    }

    [Test]
    public void Initialize ()
    {
      NullAnonymousEndPoint endPoint = new NullAnonymousEndPoint (_clientToLocationDefinition);

      Assert.IsNotNull (endPoint as INullObject);
      Assert.IsNotNull (endPoint as IEndPoint);
      Assert.IsNotNull (endPoint as AnonymousEndPoint);
      Assert.IsTrue (endPoint.IsNull);
      Assert.IsNull (endPoint.ClientTransaction);
      Assert.IsNull (endPoint.GetDomainObject ());
      Assert.IsNull (endPoint.GetDataContainer ());
      Assert.IsNull (endPoint.ObjectID);

      Assert.AreSame (_clientToLocationDefinition, endPoint.RelationDefinition);
      Assert.AreSame (_clientEndPointDefinition, endPoint.Definition);
      Assert.IsNotNull (endPoint.Definition as AnonymousRelationEndPointDefinition);
    }
  }
}
