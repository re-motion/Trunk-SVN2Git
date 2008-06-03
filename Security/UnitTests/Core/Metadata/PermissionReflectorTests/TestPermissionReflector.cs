/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Reflection;
using Remotion.Collections;
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Core.Metadata.PermissionReflectorTests
{
  public class TestPermissionReflector : PermissionReflector
  {
    // types

    // static members

    public new static Cache<Tuple<Type, Type, string, BindingFlags>, Enum[]> Cache
    {
      get { return PermissionReflector.Cache; }
    }

    // member fields

    // construction and disposing

    public TestPermissionReflector ()
    {
    }

    // methods and properties
  }
}
