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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Represents an <see cref="ObjectEndPoint"/> that holds the foreign key in a relation. The foreign key is actually held by a 
  /// <see cref="PropertyValue"/> object, this end point implementation just delegates to the <see cref="PropertyValue"/>.
  /// </summary>
  public class RealObjectEndPoint : ObjectEndPoint
  {
    private PropertyValue _foreignKeyProperty;

    public RealObjectEndPoint (
        ClientTransaction clientTransaction, 
        RelationEndPointID id, 
        PropertyValue foreignKeyProperty)
      : base (
          ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction),
          ArgumentUtility.CheckNotNull ("id", id))
    {
      ArgumentUtility.CheckNotNull ("foreignKeyProperty", foreignKeyProperty);

      if (ID.Definition.IsVirtual)
        throw new ArgumentException ("End point ID must refer to a non-virtual end point.", "id");

      if (foreignKeyProperty.Definition.PropertyType != typeof (ObjectID))
        throw new ArgumentException ("The foreign key property must have a property type of ObjectID.", "foreignKeyProperty");
      
      _foreignKeyProperty = foreignKeyProperty;
    }

    public PropertyValue ForeignKeyProperty
    {
      get { return _foreignKeyProperty; }
    }

    public override ObjectID OppositeObjectID
    {
      get { return (ObjectID) ForeignKeyProperty.GetValueWithoutEvents (ValueAccess.Current); }
      set { _foreignKeyProperty.SetRelationValue (value); }
    }

    public override ObjectID OriginalOppositeObjectID
    {
      get { return (ObjectID) ForeignKeyProperty.GetValueWithoutEvents (ValueAccess.Original); }
    }

    public override bool HasChanged
    {
      get { return ForeignKeyProperty.HasChanged; }
    }

    public override bool HasBeenTouched
    {
      get { return ForeignKeyProperty.HasBeenTouched; }
    }

    public override void Touch ()
    {
      ForeignKeyProperty.Touch ();
      Assertion.IsTrue (HasBeenTouched);
    }

    public override void Commit ()
    {
      ForeignKeyProperty.Commit ();
      Assertion.IsFalse (HasBeenTouched);
      Assertion.IsFalse (HasChanged);
    }

    public override void Rollback ()
    {
      ForeignKeyProperty.Rollback ();
      Assertion.IsFalse (HasBeenTouched);
      Assertion.IsFalse (HasChanged);
    }

    #region Serialization
    protected RealObjectEndPoint (FlattenedDeserializationInfo info)
      : base (info)
    {
      // TODO 1883: The next line should be removed when a better serialization mechanism is found, _foreignKeyProperty should again be readonly.
      info.DeserializationFinished += (sender, args) =>
          _foreignKeyProperty = Definition.IsVirtual
              ? null
              : ClientTransaction.DataManager.DataContainerMap[ObjectID].PropertyValues[PropertyName];
    }
    #endregion

  }
}