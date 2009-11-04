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
