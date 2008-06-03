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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{

public delegate void PropertyDefinitionAddingEventHandler (object sender, PropertyDefinitionAddingEventArgs args);
public delegate void PropertyDefinitionAddedEventHandler (object sender, PropertyDefinitionAddedEventArgs args);

public class PropertyDefinitionAddingEventArgs : EventArgs
{
  private PropertyDefinition _propertyDefinition;

  public PropertyDefinitionAddingEventArgs (PropertyDefinition propertyDefinition)
  {
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

    _propertyDefinition = propertyDefinition;
  }

  public PropertyDefinition PropertyDefinition
  {
    get { return _propertyDefinition; }
  }
}

public class PropertyDefinitionAddedEventArgs : EventArgs
{
  private PropertyDefinition _propertyDefinition;

  public PropertyDefinitionAddedEventArgs (PropertyDefinition propertyDefinition)
  {
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

    _propertyDefinition = propertyDefinition;
  }

  public PropertyDefinition PropertyDefinition
  {
    get { return _propertyDefinition; }
  }
}
}
