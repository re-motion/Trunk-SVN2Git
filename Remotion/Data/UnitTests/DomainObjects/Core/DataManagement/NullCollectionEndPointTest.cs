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
      base.SetUp();
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

    [Test]
    public void CreateSelfReplaceModification ()
    {
      Assert.That (_endPoint.CreateSelfReplaceModification (_relatedObject), Is.InstanceOfType (typeof (NullEndPointModification)));
    }
  }
}