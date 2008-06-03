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
using System.Collections.Specialized;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList
{

[Serializable]
public class EditableRowIDProvider
{
  // types

  // static members and constants

  // member fields

  private string _idFormat;
  private int _nextID;
  private StringCollection _excludedIDs = new StringCollection();

  // construction and disposing

  public EditableRowIDProvider (string idFormat)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("idFormat", idFormat);

    _idFormat = idFormat;
    _nextID = 0;
  }

  // methods and properties

  public string GetNextID ()
  {
    string id;
    do {
      id = string.Format (_idFormat, _nextID);
      _nextID++;
    } while (_excludedIDs.Contains (id));

    return id;
  }

  public void ExcludeID (string id)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("id", id);

    if (! _excludedIDs.Contains (id))
      _excludedIDs.Add (id);
  }

  public void Reset ()
  {
    _nextID = 0;
  }

  public string[] GetExcludedIDs ()
  {
    string[] ids = new string[_excludedIDs.Count];
    _excludedIDs.CopyTo (ids, 0);
    
    return ids;
  }
}

}
