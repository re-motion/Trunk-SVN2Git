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
using System.Collections.Generic;
using NUnit.Framework;
using System.Reflection;
using Remotion.Collections;
using Remotion.Data.Linq.Backend.FieldResolving;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Data.UnitTests.Linq.Parsing.FieldResolving
{
  [TestFixture]
  public class SelectFieldAccessPolicyTest : FieldAccessPolicyTestBase
  {
    private SelectFieldAccessPolicy _policy;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();
      _policy = new SelectFieldAccessPolicy ();
    }

    [Test]
    public void AdjustMemberInfosForFromIdentifier ()
    {
      var result = _policy.AdjustMemberInfosForDirectAccessOfQuerySource (StudentReference);
      Assert.That (result.A, Is.Null);
      Assert.That (result.B, Is.Empty);
    }

    [Test]
    public void AdjustMemberInfosForRelation()
    {
      Tuple<MemberInfo, IEnumerable<MemberInfo>> result = _policy.AdjustMemberInfosForRelation (StudentDetail_Student_Member, new[] { StudentDetailDetail_StudentDetail_Member });
      var expected = new Tuple<MemberInfo, IEnumerable<MemberInfo>> (null, new[] {StudentDetailDetail_StudentDetail_Member, StudentDetail_Student_Member});

      Assert.AreEqual (expected.A, result.A);
      Assert.That (result.B, Is.EqualTo (expected.B));
    }

    [Test]
    public void OptimizeRelatedKeyAccess_False ()
    {
      Assert.That (_policy.OptimizeRelatedKeyAccess (), Is.False);
    }
  }
}
