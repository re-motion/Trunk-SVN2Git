/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.DataManagement
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

      _clientToLocationDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Location)].GetRelationDefinition ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client");
      _clientEndPointDefinition = _clientToLocationDefinition.GetEndPointDefinition ("Client", null);
      _locationEndPointDefinition = _clientToLocationDefinition.GetEndPointDefinition ("Location", "Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client");
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
