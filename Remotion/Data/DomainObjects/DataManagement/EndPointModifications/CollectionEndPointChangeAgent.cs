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
using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  /// <summary>
  /// This class performs the actual changes to the <see cref="DomainObjectCollection"/> backing a <see cref="CollectionEndPoint"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When the  <see cref="DomainObjectCollection"/> of a <see cref="CollectionEndPoint"/> is changed from the outside, the collection first 
  /// propagates the changes to the <see cref="CollectionEndPoint"/> via the <see cref="ICollectionChangeDelegate"/> interface. The 
  /// <see cref="CollectionEndPoint"/> again propagates the changes to an implementer of the <see cref="ICollectionEndPointChangeDelegate"/> interface,
  /// usually the  <see cref="RelationEndPointMap"/>.
  /// </para>
  /// <para>
  /// The <see cref="RelationEndPointMap"/> creates one instance of  <see cref="ObjectEndPointSetModification"/> and 
  /// <see cref="CollectionEndPointChangeAgentModification"/> each; these objects contain the exact parameters 
  /// about how to change the individual end points, and they know which events to notify at which stage in the modification process. The 
  /// <see cref="CollectionEndPointChangeAgentModification"/> then creates a <see cref="CollectionEndPointChangeAgent"/>, and this agent finally performs the
  /// changes on the <see cref="DomainObjectCollection"/> via its <see cref="DomainObjectCollection.PerformAdd"/>, 
  /// <see cref="DomainObjectCollection.PerformInsert"/>, and <see cref="DomainObjectCollection.PerformRemove"/> methods.
  /// </para>
  /// <para>
  /// Thus, changes are propagated as follows: DomainObjectCollection => CollectionEndPoint => RelationEndPointMap => 
  /// CollectionEndPointChangeAgentModification => CollectionEndPointChangeAgent => DomainObjectCollection
  /// </para>
  /// </remarks>
  public class CollectionEndPointChangeAgent
  {
    // types

    public enum OperationType
    {
      Add = 0,
      Remove = 1,
      Insert = 2,
      Replace = 3
    }

    // static members and constants

    public static CollectionEndPointChangeAgent CreateForReplace (
        DomainObjectCollection oppositeDomainObjects,
        IEndPoint oldEndPoint, 
        IEndPoint newEndPoint, 
        int replaceIndex)
    {
      ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);
      ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
      ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

      return new CollectionEndPointChangeAgent (
          oppositeDomainObjects, oldEndPoint, newEndPoint, OperationType.Replace, replaceIndex);
    }

    // member fields

    private DomainObjectCollection _oppositeDomainObjects;
    private OperationType _operation;
    private IEndPoint _oldEndPoint;
    private DomainObject _oldRelatedObject;
    private IEndPoint _newEndPoint;
    private int _collectionIndex;

    // construction and disposing

    protected CollectionEndPointChangeAgent (
        DomainObjectCollection oppositeDomainObjects,
        IEndPoint oldEndPoint, 
        IEndPoint newEndPoint,
        OperationType operation,
        int collectionIndex)
    {
      ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);
      ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
      ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);
      ArgumentUtility.CheckValidEnumValue ("operation", operation);

      _oppositeDomainObjects = oppositeDomainObjects;
      _operation = operation;
      _oldEndPoint = oldEndPoint;
      _oldRelatedObject = oldEndPoint.GetDomainObject ();
      _newEndPoint = newEndPoint;
      _collectionIndex = collectionIndex;
    }
    
    // methods and properties

    public virtual void BeginRelationChange ()
    {
      if (MustRemoveObject)
        _oppositeDomainObjects.BeginRemove (OldRelatedObject);

      if (MustAddObject)
        _oppositeDomainObjects.BeginAdd (NewRelatedObject);
    }

    public virtual void PerformRelationChange ()
    {
      switch (_operation)
      {
        case OperationType.Remove:
          _oppositeDomainObjects.PerformRemove (OldRelatedObject);
          break;

        case OperationType.Add:
          _oppositeDomainObjects.PerformAdd (NewRelatedObject);
          break;

        case OperationType.Insert:
          _oppositeDomainObjects.PerformInsert (CollectionIndex, NewRelatedObject);
          break;

        case OperationType.Replace:
          _oppositeDomainObjects.PerformRemove (OldRelatedObject);
          _oppositeDomainObjects.PerformInsert (CollectionIndex, NewRelatedObject);
          break;
      }
    }

    public virtual void EndRelationChange ()
    {
      if (MustRemoveObject)
        _oppositeDomainObjects.EndRemove (OldRelatedObject);

      if (MustAddObject)
        _oppositeDomainObjects.EndAdd (NewRelatedObject);
    }

    protected bool MustRemoveObject
    {
      get 
      { 
        return (_operation == OperationType.Remove 
                || _operation == OperationType.Replace); 
      }
    }

    protected bool MustAddObject
    {
      get 
      { 
        return (_operation == OperationType.Add 
                || _operation == OperationType.Insert
                || _operation == OperationType.Replace); 
      }
    }

    public OperationType Operation
    {
      get { return _operation; }
    }

    public IEndPoint OldEndPoint
    {
      get { return _oldEndPoint; }
    }

    public IEndPoint NewEndPoint
    {
      get { return _newEndPoint; }
    }

    protected DomainObject OldRelatedObject
    {
      get { return _oldRelatedObject; }
    }

    protected DomainObject NewRelatedObject 
    {
      get { return _newEndPoint.GetDomainObject (); }
    }

    protected int CollectionIndex
    {
      get { return _collectionIndex; }
    }
  }
}