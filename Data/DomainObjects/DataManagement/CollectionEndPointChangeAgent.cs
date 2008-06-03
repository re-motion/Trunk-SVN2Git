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

namespace Remotion.Data.DomainObjects.DataManagement
{
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

  public static CollectionEndPointChangeAgent CreateForAddOrRemove (DomainObjectCollection oppositeDomainObjects, IEndPoint oldEndPoint,
      IEndPoint newEndPoint)
  {
    ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    if (oldEndPoint.IsNull && newEndPoint.IsNull)
      throw new ArgumentException ("Both endPoints cannot be NullEndPoints.", "oldEndPoint, newEndPoint");
    else if (!oldEndPoint.IsNull && !newEndPoint.IsNull)
      throw new ArgumentException ("One endPoint must be a NullEndPoint.", "oldEndPoint, newEndPoint");
    else if (!oldEndPoint.IsNull && newEndPoint.IsNull)
      return CollectionEndPointChangeAgent.CreateForRemove (oppositeDomainObjects, oldEndPoint, newEndPoint);
    else
    {
      Assertion.IsTrue (!newEndPoint.IsNull && oldEndPoint.IsNull);
      return CollectionEndPointChangeAgent.CreateForAdd (oppositeDomainObjects, oldEndPoint, newEndPoint);
    }
  }

  public static CollectionEndPointChangeAgent CreateForAdd (
      DomainObjectCollection oppositeDomainObjects,
      IEndPoint oldEndPoint, 
      IEndPoint newEndPoint)
  {
    ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    return new CollectionEndPointChangeAgent (
        oppositeDomainObjects, oldEndPoint, newEndPoint, OperationType.Add, oppositeDomainObjects.Count);
  }

  public static CollectionEndPointChangeAgent CreateForRemove (
      DomainObjectCollection oppositeDomainObjects,
      IEndPoint oldEndPoint, 
      IEndPoint newEndPoint)
  {
    ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    return new CollectionEndPointChangeAgent (
        oppositeDomainObjects, oldEndPoint, newEndPoint, 
        OperationType.Remove, oppositeDomainObjects.IndexOf (oldEndPoint.ObjectID));
  }

  public static CollectionEndPointChangeAgent CreateForInsert (
      DomainObjectCollection oppositeDomainObjects,
      IEndPoint oldEndPoint, 
      IEndPoint newEndPoint, 
      int insertIndex)
  {
    ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    return new CollectionEndPointChangeAgent (
        oppositeDomainObjects, oldEndPoint, newEndPoint, OperationType.Insert, insertIndex);
  }

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
