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
using System.Collections.Generic;

namespace Remotion.Security.Metadata
{

  public class SecurableClassInfo : MetadataInfo
  {
    // types

    // static members

    // member fields

    private List<StatePropertyInfo> _properties = new List<StatePropertyInfo>();
    private List<EnumValueInfo> _accessTypes = new List<EnumValueInfo>();
    private SecurableClassInfo _baseClass;
    private List<SecurableClassInfo> _derivedClasses = new List<SecurableClassInfo> ();

    // construction and disposing

    public SecurableClassInfo ()
    {
    }

    // methods and properties

    public List<StatePropertyInfo> Properties
    {
      get { return _properties; }
    }

    public List<EnumValueInfo> AccessTypes
    {
      get { return _accessTypes; }
    }

    public SecurableClassInfo BaseClass
    {
      get { return _baseClass; }
      set { _baseClass = value; }
    }

    public List<SecurableClassInfo> DerivedClasses
    {
      get { return _derivedClasses; }
    }
	
  }
}
