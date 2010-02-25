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
using System.Reflection;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.UnitTests.TestDomain;

namespace Remotion.Data.Linq.UnitTests.Backend.FieldResolving
{
  public abstract class FieldAccessPolicyTestBase
  {
    private MainFromClause _studentClause;
    private QuerySourceReferenceExpression _studentReference;
    
    private MemberInfo _studentDetailDetail_StudentDetail_Member;
    private MemberInfo _studentDetail_Student_Member;
    private MemberInfo _industrialSector_StudentDetail_Member;
    private MemberInfo _studentDetailDetail_IndustrialSector_Member;
    private MemberInfo _studentDetail_IndustrialSector_Member;

    public MemberInfo StudentDetailDetail_StudentDetail_Member
    {
      get { return _studentDetailDetail_StudentDetail_Member; }
    }

    public MemberInfo StudentDetail_Student_Member
    {
      get { return _studentDetail_Student_Member; }
    }

    public MainFromClause StudentClause
    {
      get { return _studentClause; }
    }

    public QuerySourceReferenceExpression StudentReference
    {
      get { return _studentReference; }
    }

    public MemberInfo StudentDetailDetail_IndustrialSector_Member
    {
      get { return _studentDetailDetail_IndustrialSector_Member; }
    }

    public MemberInfo StudentDetail_IndustrialSector_Member
    {
      get { return _studentDetail_IndustrialSector_Member; }
    }

    public MemberInfo IndustrialSector_StudentDetail_Member
    {
      get { return _industrialSector_StudentDetail_Member; }
    }

    public virtual void SetUp ()
    {
      _studentClause = ExpressionHelper.CreateMainFromClause_Student ();
      _studentReference = new QuerySourceReferenceExpression (StudentClause);
      _studentDetailDetail_StudentDetail_Member = typeof (Company).GetProperty ("MainKitchen");
      _studentDetail_Student_Member = typeof (Kitchen).GetProperty ("Cook");
      _studentDetailDetail_IndustrialSector_Member = typeof (Company).GetProperty ("Restaurant");
      _studentDetail_IndustrialSector_Member = typeof (Kitchen).GetProperty ("Restaurant");
      _industrialSector_StudentDetail_Member = typeof (Restaurant).GetProperty ("SubKitchen");
    }
  }
}
