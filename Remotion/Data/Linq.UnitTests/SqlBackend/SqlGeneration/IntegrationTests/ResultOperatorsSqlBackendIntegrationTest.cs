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

namespace Remotion.Data.Linq.UnitTests.SqlBackend.SqlGeneration.IntegrationTests
{
  [TestFixture]
  public class ResultOperatorsSqlBackendIntegrationTest : SqlBackendIntegrationTestBase
  {
    [Test]
    [Ignore ("TODO 2399")]
    public void TODO ()
    {
      
    }

    //result operators
    //(from c in _cooks select c).Count()
    //(from c in _cooks select c).Distinct()
    //(from c in _cooks select c).Take(5)
    //(from c in _cooks select c).Take(c.ID)
    //from k in _kitchen from c in k.Restaurant.Cooks.Take(k.RoomNumber) select c
    //(from c in _cooks select c).Single()
    //(from c in _cooks select c).First()
  }
}