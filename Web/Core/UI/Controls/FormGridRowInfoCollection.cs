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
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{

/// <summary> A read only collection of <see cref="FormGridManager.FormGridRow"/> objects. </summary>
public sealed class FormGridRowInfoCollection : CollectionBase
{
  /// <summary> 
  ///   Initalizates a new <see cref="FormGridRowInfoCollection"/> class
  ///   with an array of <see cref="FormGridRowInfo"/> objects.
  /// </summary>
  /// <param name="values">
  ///   The array of <see cref="FormGridRowInfo"/> object to be managed by this collection.
  ///   May not contain <see langword="null"/>.
  /// </param>
  public FormGridRowInfoCollection (FormGridRowInfo[] values)
  {
    ArgumentUtility.CheckNotNull ("values", values);

    for (int index = 0; index < values.Length; index++)
    {
      if (values[index] == null)
        throw new ArgumentNullException ("values[" + index + "]");
    }

    InnerList.AddRange (values);
  }

  /// <summary> Initalizates a new <see cref="FormGridRowInfoCollection"/> class. </summary>
  public FormGridRowInfoCollection()
  {}

  /// <summary> Allows only the insertion of form grid row prototypes. </summary>
  /// <param name="index"> The zero-based index at which to insert value. </param>
  /// <param name="value"> The new value of the element at index. </param>
  protected override void OnInsert (int index, object value)
  {
    ArgumentUtility.CheckNotNullAndType<FormGridRowInfo> ("value", value);
    base.OnInsert (index, value);
  }

  /// <summary> Adds the form grid row prototype to the end of the list. </summary>
  /// <param name="value"> The new form grid row prototype. </param>
  public void Add (FormGridRowInfo value)
  {
    OnInsert (InnerList.Count, value);
    int index = InnerList.Add (value);
    OnInsertComplete (index, value);
  }
}

}
