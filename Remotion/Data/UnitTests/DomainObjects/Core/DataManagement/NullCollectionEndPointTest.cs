// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class NullCollectionEndPointTest : ClientTransactionBaseTest
  {
    private IRelationEndPointDefinition _definition;
    private NullCollectionEndPoint _endPoint;
    private OrderItem _relatedObject;

    public override void SetUp ()
    {
      base.SetUp ();
      _definition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order))
          .GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");
      _endPoint = new NullCollectionEndPoint (_definition);
      _relatedObject = OrderItem.NewObject();
    }

    [Test]
    public void CreateInsertModification ()
    {
      Assert.That (_endPoint.CreateInsertModification (_relatedObject, 12), Is.InstanceOfType (typeof (NullEndPointModification)));
    }

    [Test]
    public void CreateAddModification ()
    {
      Assert.That (_endPoint.CreateAddModification (_relatedObject), Is.InstanceOfType (typeof (NullEndPointModification)));
    }

    [Test]
    public void CreateRemoveModification ()
    {
      Assert.That (_endPoint.CreateRemoveModification (_relatedObject), Is.InstanceOfType (typeof (NullEndPointModification)));
    }

    [Test]
    public void CreateReplaceModification ()
    {
      Assert.That (_endPoint.CreateReplaceModification (12, _relatedObject), Is.InstanceOfType (typeof (NullEndPointModification)));
    }
  }
}