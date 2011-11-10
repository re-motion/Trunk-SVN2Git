// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Core.Configuration
{
  public class MemberResolverMock : IMemberResolver
  {
    public IMethodInformation GetMethodInformation (Type type, string methodName, MemberAffiliation memberAffiliation)
    {
      throw new NotImplementedException();
    }

    public IPropertyInformation GetPropertyInformation (Type type, string propertyName)
    {
      throw new NotImplementedException();
    }

    public IMethodInformation GetMethodInformation (Type type, MethodInfo methodInfo, MemberAffiliation memberAffiliation)
    {
      throw new NotImplementedException();
    }

    public IPropertyInformation GetPropertyInformation (Type type, PropertyInfo propertyInfo)
    {
      throw new NotImplementedException();
    }

    public bool IsNull
    {
      get { throw new NotImplementedException(); }
    }
  }
}