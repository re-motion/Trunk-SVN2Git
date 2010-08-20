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

namespace Remotion.Implementation
{
  [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class ConcreteImplementationAttribute : Attribute
  {
    private readonly string _typeNameTemplate;
    private LifetimeKind _lifeTime = LifetimeKind.Instance;

    public ConcreteImplementationAttribute (string typeNameTemplate)
    {
      _typeNameTemplate = ArgumentUtility.CheckNotNull ("typeNameTemplate", typeNameTemplate);
    }

    public ConcreteImplementationAttribute (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      _typeNameTemplate = type.AssemblyQualifiedName;
    }

    public string TypeNameTemplate
    {
      get { return _typeNameTemplate; }
    }

    public LifetimeKind LifeTime
    {
      get { return _lifeTime; }
      set { _lifeTime = value; }
    }
    
  }
}
