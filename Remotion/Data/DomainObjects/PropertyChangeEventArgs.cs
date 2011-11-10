// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides data for a <b>PropertyChanging</b> event.
  /// </summary>
  public class PropertyChangeEventArgs : ValueChangeEventArgs
  {
    private readonly PropertyValue _propertyValue;

    /// <summary>
    /// Initializes a new instance of the <b>ValueChangingEventArgs</b> class.
    /// </summary>
    /// <param name="propertyValue">The <see cref="PropertyValue"/> that is being changed. Must not be <see langword="null"/>.</param>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="propertyValue"/> is <see langword="null"/>.</exception>
    public PropertyChangeEventArgs (PropertyValue propertyValue, object oldValue, object newValue)
        : base (oldValue, newValue)
    {
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);
      _propertyValue = propertyValue;
    }

    /// <summary>
    /// Gets the <see cref="DataManagement.PropertyValue"/> object that is being changed.
    /// </summary>
    public PropertyValue PropertyValue
    {
      get { return _propertyValue; }
    }
  }
}