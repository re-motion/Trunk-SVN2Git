// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System;
using System.Reflection;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  public class SecurableClassDefinitionWrapper
  {
    // types

    // static members

    // member fields

    private SecurableClassDefinition _securableClassDefinition;
    private PropertyInfo _accessTypeReferencesPropertyInfo;

    // construction and disposing

    public SecurableClassDefinitionWrapper (SecurableClassDefinition securableClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("securableClassDefinition", securableClassDefinition);

      _securableClassDefinition = securableClassDefinition;
      _accessTypeReferencesPropertyInfo = _securableClassDefinition.GetPublicDomainObjectType().GetProperty (
          "AccessTypeReferences",
          BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
    }

    // methods and properties

    public SecurableClassDefinition SecurableClassDefinition
    {
      get { return _securableClassDefinition; }
    }

    public DomainObjectCollection AccessTypeReferences
    {
      get { return (DomainObjectCollection) _accessTypeReferencesPropertyInfo.GetValue (_securableClassDefinition, new object[0]); }
    }
  }
}
