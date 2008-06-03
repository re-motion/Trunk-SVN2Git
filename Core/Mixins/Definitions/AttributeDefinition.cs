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
    private readonly bool _isCopyTemplate;

    public AttributeDefinition (IAttributableDefinition declaringDefinition, CustomAttributeData data, bool isCopyTemplate)
    {
      _declaringDefinition = declaringDefinition;
      _data = data;
      _isCopyTemplate = isCopyTemplate;
    }

    public CustomAttributeData Data
    {
      get { return _data;}
    }

    public Type AttributeType
    {
      get { return _data.Constructor.DeclaringType; }
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
