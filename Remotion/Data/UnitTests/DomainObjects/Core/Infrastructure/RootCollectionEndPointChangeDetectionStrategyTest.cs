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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class RootCollectionEndPointChangeDetectionStrategyTest : ClientTransactionBaseTest
  {
    private Order _order1;
    private Order _order2;
    private Order _order3;

    private RootCollectionEndPointChangeDetectionStrategy _strategy;
    private IDomainObjectCollectionData _currentData;

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);
      _order3 = Order.GetObject (DomainObjectIDs.Order3);

      _currentData = new DomainObjectCollectionData (new[] { _order1, _order2 });

      _strategy = new RootCollectionEndPointChangeDetectionStrategy ();
    }

    [Test]
    public void HasDataChanged_False ()
    {
      Assert.That (_strategy.HasDataChanged (_currentData, new DomainObjectCollectionData (_currentData)), Is.False);
    }

    [Test]
    public void HasDataChanged_False_OrderOnly ()
    {
      var originalData = new DomainObjectCollectionData (_currentData.Reverse());
      Assert.That (_strategy.HasDataChanged (_currentData, originalData), Is.False);
    }

    [Test]
    public void HasDataChanged_True_Content ()
    {
      var originalData = new DomainObjectCollectionData (new[] { _order1, _order2, _order3 });
      Assert.That (_strategy.HasDataChanged (_currentData, originalData), Is.True);
    }
  }
}