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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver
{
  public class PropertyDefinitionCollectionEventReceiver : EventReceiverBase
  {
    // types

    // static members and constants

    // member fields

    private bool _cancel;
    private PropertyDefinition _addingPropertyDefinition;
    private PropertyDefinition _addedPropertyValue;

    // construction and disposing

    public PropertyDefinitionCollectionEventReceiver (PropertyDefinitionCollection collection, bool cancel)
    {
      _cancel = cancel;

      collection.Adding += new PropertyDefinitionAddingEventHandler (
          PropertyDefinitionCollection_Adding);

      collection.Added += new PropertyDefinitionAddedEventHandler (
          PropertyDefinitionCollection_Added);
    }

    // methods and properties

    public PropertyDefinition AddingPropertyDefinition
    {
      get { return _addingPropertyDefinition; }
    }

    public PropertyDefinition AddedPropertyDefinition
    {
      get { return _addedPropertyValue; }
    }

    private void PropertyDefinitionCollection_Adding (object sender, PropertyDefinitionAddingEventArgs args)
    {
      _addingPropertyDefinition = args.PropertyDefinition;

      if (_cancel)
        CancelOperation ();
    }

    private void PropertyDefinitionCollection_Added (object sender, PropertyDefinitionAddedEventArgs args)
    {
      _addedPropertyValue = args.PropertyDefinition;
    }
  }
}
