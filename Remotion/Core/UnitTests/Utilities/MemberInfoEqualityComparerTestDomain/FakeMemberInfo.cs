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

namespace Remotion.UnitTests.Utilities.MemberInfoEqualityComparerTestDomain
{
  public class FakeMemberInfo : MemberInfo
  {
    private readonly Type _declaringType;
    private readonly int _metadataToken;
    private Module _module;

    public FakeMemberInfo (Type declaringType, int metadataToken, Module module)
    {
      _declaringType = declaringType;
      _metadataToken = metadataToken;
      _module = module;
    }

    public override object[] GetCustomAttributes (bool inherit)
    {
      throw new NotImplementedException();
    }

    public override bool IsDefined (Type attributeType, bool inherit)
    {
      throw new NotImplementedException();
    }

    public override MemberTypes MemberType
    {
      get { throw new NotImplementedException(); }
    }

    public override string Name
    {
      get { throw new NotImplementedException(); }
    }

    public override Type DeclaringType
    {
      get { return _declaringType; }
    }

    public override Type ReflectedType
    {
      get { throw new NotImplementedException(); }
    }

    public override object[] GetCustomAttributes (Type attributeType, bool inherit)
    {
      throw new NotImplementedException();
    }

    public override int MetadataToken
    {
      get { return _metadataToken; }
    }

    public override Module Module
    {
      get { return _module; }
    }
  }
}