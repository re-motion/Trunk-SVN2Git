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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{
  public class NullMemberResolver : IMemberResolver
  {
    public IMethodInformation GetMethodInformation (Type type, string methodName, MemberAffiliation memberAffiliation)
    {
      return new NullMethodInformation();
    }

    public IPropertyInformation GetPropertyInformation (Type type, string propertyName)
    {
      return new NullPropertyInformation();
    }

    public IMethodInformation GetMethodInformation (Type type, MethodInfo methodInfo, MemberAffiliation memberAffiliation)
    {
      return new NullMethodInformation();
    }

    public IPropertyInformation GetPropertyInformation (Type type, PropertyInfo propertyInfo)
    {
      return new NullPropertyInformation();
    }

    public bool IsNull
    {
      get { return true; }
    }

    public override bool Equals (object obj)
    {
      var other = obj as NullMemberResolver;

      return other != null && other.GetType() == GetType();
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (this);
    }
  }
}