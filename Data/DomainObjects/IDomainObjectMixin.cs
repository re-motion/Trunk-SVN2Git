// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Allows a mixin applied to a <see cref="DomainObject"/> to react on events related to the <see cref="DomainObject"/> instance.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Implement this interface on a mixin applied to a <see cref="DomainObject"/> to be informed about when the <see cref="DomainObject"/> instance
  /// is created or loaded.
  /// </para>
  /// <para>
  /// The hook methods defined on
  /// this interface are called by the <see cref="DomainObjects"/> infrastructure at points of time when it is safe to access the domain object's
  /// ID and properties. Use them instead of <see cref="Mixin{TThis}.OnInitialized"/> to execute mixin initialization code that must access
  /// the domain object's properties.
  /// </para>
  /// </remarks>
  public interface IDomainObjectMixin
  {
    /// <summary>
    /// Called when the mixin's target domain object has been created.
    /// </summary>
    void OnDomainObjectCreated ();
    
    /// <summary>
    /// Called when the mixin's target domain object has been loaded.
    /// </summary>
    /// <param name="loadMode">Specifies whether the whole domain object or only the <see cref="Remotion.Data.DomainObjects.DataContainer"/> has been
    /// newly loaded.</param>
    void OnDomainObjectLoaded (LoadMode loadMode);
  }
}
