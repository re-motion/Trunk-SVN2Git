// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Diagnostics;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay("{AttributeType}")]
  public class AttributeDefinition: IVisitableDefinition
  {
    private readonly IAttributableDefinition _declaringDefinition;
    private readonly CustomAttributeData _data;
    private readonly object _instance;
    private readonly bool _isCopyTemplate;

    public AttributeDefinition (IAttributableDefinition declaringDefinition, CustomAttributeData data, bool isCopyTemplate)
    {
      _declaringDefinition = declaringDefinition;
      _data = data;
      _isCopyTemplate = isCopyTemplate;
      _instance = CustomAttributeDataUtility.InstantiateCustomAttributeData (data);
    }

    public CustomAttributeData Data
    {
      get { return _data;}
    }

    public object Instance
    {
      get { return _instance; }
    }

    public Type AttributeType
    {
      get { return _data.Constructor.DeclaringType; }
    }

    public bool IsIntroducible
    {
      get { return AttributeUtility.IsAttributeInherited (AttributeType) || IsCopyTemplate; }
    }

    public bool IsSuppressAttribute
    {
      get { return typeof (SuppressAttributesAttribute).IsAssignableFrom (AttributeType); }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }

    public string FullName
    {
      get { return _data.Constructor.DeclaringType.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return DeclaringDefinition as IVisitableDefinition; }
    }

    public IAttributableDefinition DeclaringDefinition
    {
      get { return _declaringDefinition; }
    }

    public bool IsCopyTemplate
    {
      get { return _isCopyTemplate; }
    }
  }
}
