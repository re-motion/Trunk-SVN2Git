/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Parsing.FieldResolving;
using System.Linq;
using System.Linq.Expressions;

namespace Remotion.Data.UnitTests.Linq.ParsingTest.FieldResolvingTest
{
  [TestFixture]
  public class WhereFieldAccessPolicyTest
  {
    [Test]
    public void AdjustMemberInfosForFromIdentifier ()
    {
      MainFromClause fromClause = 
          ExpressionHelper.CreateMainFromClause (Expression.Parameter (typeof (Student), "s"), ExpressionHelper.CreateQuerySource());
      WhereFieldAccessPolicy policy = new WhereFieldAccessPolicy (StubDatabaseInfo.Instance);

      var result = policy.AdjustMemberInfosForAccessedIdentifier (fromClause.Identifier);
      Assert.That (result.A, Is.EqualTo (typeof (Student).GetProperty ("ID")));
      Assert.That (result.B, Is.Empty);
    }

    [Test]
    public void AdjustMemberInfosForRelation ()
    {
      MemberInfo joinMember = typeof (Student_Detail_Detail).GetProperty ("Student_Detail");
      MemberInfo relationMember = typeof (Student_Detail).GetProperty ("IndustrialSector");

      WhereFieldAccessPolicy policy = new WhereFieldAccessPolicy (StubDatabaseInfo.Instance);
      Tuple<MemberInfo, IEnumerable<MemberInfo>> result = policy.AdjustMemberInfosForRelation (relationMember, new[] { joinMember });
      var expected = new Tuple<MemberInfo, IEnumerable<MemberInfo>> (relationMember, new[] { joinMember });

      Assert.AreEqual (expected.A, result.A);
      Assert.That (result.B, Is.EqualTo (expected.B));
    }

    [Test]
    public void AdjustMemberInfosForRelation_VirtualSide ()
    {
      MemberInfo joinMember = typeof (Student_Detail_Detail).GetProperty ("IndustrialSector");
      MemberInfo relationMember = typeof (IndustrialSector).GetProperty ("Student_Detail");
      MemberInfo primaryKeyMember = typeof (Student_Detail).GetProperty ("ID");

      WhereFieldAccessPolicy policy = new WhereFieldAccessPolicy (StubDatabaseInfo.Instance);
      Tuple<MemberInfo, IEnumerable<MemberInfo>> result = policy.AdjustMemberInfosForRelation (relationMember, new[] { joinMember });
      
      var expected = new Tuple<MemberInfo, IEnumerable<MemberInfo>> (primaryKeyMember, new[] { joinMember, relationMember });

      Assert.AreEqual (expected.A, result.A);
      Assert.That (result.B.ToArray(), Is.EqualTo (expected.B.ToArray()));
    }

  }
}
