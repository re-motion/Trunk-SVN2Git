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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// <see cref="PropertyNotFoundRelationEndPointDefinition"/> represents an invalid relation endpoint where the property could not be found.
  /// </summary>
  public class PropertyNotFoundRelationEndPointDefinition : IRelationEndPointDefinition
  {
    private readonly ClassDefinition _classDefinition;
    private readonly string _propertyName;
    private RelationDefinition _relationDefinition;

    public PropertyNotFoundRelationEndPointDefinition (ClassDefinition classDefinition, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyName", propertyName);

      _classDefinition = classDefinition;
      _propertyName = propertyName;
    }

    public ClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    public string PropertyName
    {
      get { return _propertyName;  }
    }

    public RelationDefinition RelationDefinition
    {
      get { return _relationDefinition; }
    }

    public Type PropertyType
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsPropertyTypeResolved
    {
      get { throw new NotImplementedException(); }
    }

    public string PropertyTypeName
    {
      get { throw new NotImplementedException(); }
    }

    public PropertyInfo PropertyInfo
    {
      get { return null; }
    }

    public bool IsPropertyInfoResolved
    {
      get { return false; }
    }

    public bool IsMandatory
    {
      get { throw new NotImplementedException(); }
    }

    public CardinalityType Cardinality
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsVirtual
    {
      get { return false;  }
    }

    public bool IsAnonymous
    {
      get { return false;  }
    }

    public bool CorrespondsTo (string classID, string propertyName)
    {
      throw new NotImplementedException();
    }

    public void SetRelationDefinition (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

      _relationDefinition = relationDefinition;
    }
  }
}