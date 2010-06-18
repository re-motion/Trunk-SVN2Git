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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides data for a <b>RelationChanged</b> event.
  /// </summary>
  public class RelationChangedEventArgs : EventArgs
  {
    private readonly string _propertyName;

    /// <summary>
    /// Initializes a new instance of the <b>RelationChangingEventArgs</b> class.
    /// </summary>
    /// <param name="propertyName">The name of the <see cref="PropertyValue"/> that is being changed due to the relation change. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    public RelationChangedEventArgs (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      _propertyName = propertyName;
    }

    /// <summary>
    /// Gets the name of the <see cref="PropertyValue"/> that has been changed due to the relation change.
    /// </summary>
    public string PropertyName
    {
      get { return _propertyName; }
    }
  }
}