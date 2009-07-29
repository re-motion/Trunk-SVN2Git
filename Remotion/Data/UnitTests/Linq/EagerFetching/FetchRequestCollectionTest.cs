// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.EagerFetching;
using Remotion.Data.UnitTests.Linq.TestDomain;

namespace Remotion.Data.UnitTests.Linq.EagerFetching
{
  [TestFixture]
  public class FetchRequestCollectionTest
  {
    private FetchRequestCollection _collection;
    private MemberInfo _scoresMember;

    [SetUp]
    public void SetUp ()
    {
      _collection = new FetchRequestCollection ();
      _scoresMember = typeof (Student).GetProperty ("Scores");
    }

    [Test]
    public void AddFetchRequest ()
    {
      Assert.That (_collection.FetchRequests, Is.Empty);

      var result = _collection.GetOrAddFetchRequest (new FetchManyRequest (_scoresMember));

      Assert.That (result.RelationMember, Is.SameAs (_scoresMember));
      Assert.That (_collection.FetchRequests, Is.EqualTo (new[] { result }));
    }

    [Test]
    public void AddFetchRequest_Twice ()
    {
      Assert.That (_collection.FetchRequests, Is.Empty);
      var result1 = _collection.GetOrAddFetchRequest (new FetchManyRequest (_scoresMember));
      var result2 = _collection.GetOrAddFetchRequest (new FetchManyRequest (_scoresMember));

      Assert.That (result1, Is.SameAs (result2));
      Assert.That (_collection.FetchRequests, Is.EqualTo (new[] { result1 }));
    }
  }
}