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
