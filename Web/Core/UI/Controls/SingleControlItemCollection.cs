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
using System.Collections;
using System.ComponentModel;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{

public class SingleControlItemCollection
  : ICollection, IEnumerable // For Designer Support. (VS2003, VS2005)
{
  private Type[] _supportedTypes;
  private IControlItem _controlItem;

  /// <summary> Creates a new instance. </summary>
  /// <param name="supportedTypes"> Supported types must implement <see cref="IControlItem"/>. </param>
  public SingleControlItemCollection (IControlItem controlItem, Type[] supportedTypes)
  {
    _supportedTypes = supportedTypes;
    ControlItem = controlItem;
  }

  public SingleControlItemCollection (Type[] supportedTypes)
    : this (null, supportedTypes)
  {
  }

  public IControlItem ControlItem
  {
    get { return _controlItem; }
    set 
    {
      if (value != null && ! IsSupportedType (value)) 
        throw new ArgumentTypeException ("value", value.GetType());
      _controlItem = value;
    }
  }

  /// <summary>Tests whether the specified control item's type is supported by the collection. </summary>
  private bool IsSupportedType (IControlItem controlItem)
  {
    Type controlItemType = controlItem.GetType();

    foreach (Type type in _supportedTypes)
    {
      if (type.IsAssignableFrom (controlItemType))
        return true;
    }
    
    return false;
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
     return new SingleControlItemCollectionEnumerator (_controlItem);
  }

  void ICollection.CopyTo (Array array, int index)
  {
    throw new NotSupportedException();
  }

  int ICollection.Count 
  {
    get { return 1; }
  }

  bool ICollection.IsSynchronized 
  { 
    get { return true; }
  }

  object ICollection.SyncRoot
  { 
    get { return this; }
  }

  /// <summary> For Designer Support. (VS2003, VS2005) </summary>
  /// <exclude/>
  [EditorBrowsable (EditorBrowsableState.Never)]
  public IControlItem this[int index]
  {
	  get
	  {
      if (index > 0) throw new NotSupportedException ("Getting an element above index 0 is not implemented.");
      return ControlItem;
	  }
	  set
	  {
      if (index > 0) throw new NotSupportedException ("Setting an element above index 0 is not implemented.");
      ControlItem = value;
	  }
  }

  /// <summary> For Designer Support. (VS2003, VS2005) </summary>
  /// <exclude/>
  [EditorBrowsable (EditorBrowsableState.Never)]
  public int Add (IControlItem value)
  {
    ControlItem = value;
    return 1;
  }
}

public class SingleControlItemCollectionEnumerator: IEnumerator
{
  private IControlItem _controlItem;
  bool _isMoved;
  bool _isEnd;

  internal SingleControlItemCollectionEnumerator (IControlItem controlItem)
  {
    _controlItem = controlItem;
    _isMoved = false;
    _isEnd = false;
  }

  public void Reset()
  {
    _isMoved = false;
    _isEnd = false;
  }

  public object Current
  {
    get
    {
      if (! _isMoved) throw new InvalidOperationException ("The enumerator is positioned before the first element.");
      if (_isEnd) throw new InvalidOperationException ("The enumerator is positioned after the last element.");
      return _controlItem;
    }
  }

  public bool MoveNext()
  {
    if (_isMoved)
      _isEnd = true;
    _isMoved = true;
    if (_controlItem == null)
      _isEnd = true;
    return ! _isEnd;
  }
}

}
