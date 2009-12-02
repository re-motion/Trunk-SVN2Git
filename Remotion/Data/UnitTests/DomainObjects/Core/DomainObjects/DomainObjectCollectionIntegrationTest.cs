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
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class DomainObjectCollectionIntegrationTest : ClientTransactionBaseTest
  {
    [Test]
    public void Remove_WithDiscardedObject ()
    {
      var collection = new DomainObjectCollection ();
      Customer customer = Customer.NewObject ();
      collection.Add (customer);
      customer.Delete ();
      Assert.That (customer.IsDiscarded, Is.True);

      //The next line does not throw an ObjectDiscardedException:
      collection.Remove (customer);

      Assert.That (collection, Is.Empty);
    }

    [Test]
    public void Clear_WithDiscardedObject ()
    {
      var collection = new DomainObjectCollection ();
      Customer customer = Customer.NewObject ();
      collection.Add (customer);

      customer.Delete ();
      Assert.That (customer.IsDiscarded, Is.True);

      //The next line does not throw an exception:
      collection.Clear ();

      Assert.That (collection, Is.Empty);
    }


  }
}