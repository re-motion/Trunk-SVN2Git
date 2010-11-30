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
using System.Collections.Generic;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Mapping
{
[Serializable]
public class PropertyDefinitionCollection : CommonCollection
{
  // types

  // static members and constants

  public static IEnumerable<PropertyDefinition> CreateForAllProperties (ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    return classDefinition.CreateSequence (cd => cd.BaseClass).SelectMany (cd => cd.MyPropertyDefinitions.Cast<PropertyDefinition> ());
  }

  // member fields

  public event PropertyDefinitionAddingEventHandler Adding;
  public event PropertyDefinitionAddedEventHandler Added;

  private readonly ClassDefinition _classDefinition;

  // construction and disposing

  public PropertyDefinitionCollection ()
  {
  }

  public PropertyDefinitionCollection (ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    _classDefinition = classDefinition;
  }

  // standard constructor for collections
  public PropertyDefinitionCollection (IEnumerable<PropertyDefinition> collection, bool makeCollectionReadOnly)  
  {
    ArgumentUtility.CheckNotNull ("collection", collection);

    foreach (PropertyDefinition propertyDefinition in collection)
      Add (propertyDefinition);
    
    SetIsReadOnly (makeCollectionReadOnly);
  }

  // methods and properties

  public void SetReadOnly ()
  {
    SetIsReadOnly (true);
  }

  public bool ContainsColumnName (string columnName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);

    foreach (PropertyDefinition propertyDefinition in this)
    {
      if (propertyDefinition.StoragePropertyDefinition.Name == columnName)
        return true;
    }

    return false;
  }

  public IEnumerable<PropertyDefinition> GetAllPersistent ()
  {
    foreach (PropertyDefinition propertyDefinition in this)
    {
      if (propertyDefinition.StorageClass == StorageClass.Persistent)
        yield return propertyDefinition;
    }
  }

  public ClassDefinition ClassDefinition 
  {
    get { return _classDefinition; }
  }

  protected virtual void OnAdding (PropertyDefinitionAddingEventArgs args)
  {
    if (Adding != null)
      Adding (this, args);
  }

  protected virtual void OnAdded (PropertyDefinitionAddedEventArgs args)
  {
    if (Added != null)
      Added (this, args);
  }

  #region Standard implementation for "add-only" collections

  public bool Contains (PropertyDefinition propertyDefinition)
  {
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

    return BaseContains (propertyDefinition.PropertyName, propertyDefinition);
  }

  public bool Contains (string propertyName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    return BaseContainsKey (propertyName);
  }

  public PropertyDefinition this [int index]  
  {
    get { return (PropertyDefinition) BaseGetObject (index); }
  }

  public PropertyDefinition this [string propertyName]  
  {
    get 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      return (PropertyDefinition) BaseGetObject (propertyName); 
    }
  }

  public int Add (PropertyDefinition value)  
  {
    ArgumentUtility.CheckNotNull ("value", value);

    OnAdding (new PropertyDefinitionAddingEventArgs (value));
    int position = BaseAdd (value.PropertyName, value);
    OnAdded (new PropertyDefinitionAddedEventArgs (value));

    return position;
  }

  #endregion
}
}
