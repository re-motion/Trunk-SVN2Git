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
using System.Collections.Specialized;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport
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