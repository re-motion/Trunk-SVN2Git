// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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

namespace Remotion.Security.UnitTests.TestDomain
{
  [PermanentGuid ("00000000-0000-0000-0002-000000000000")]
  public class PaperFile : File
  {
    private FileState _state;
    private string _location;
    
    public PaperFile ()
    {
    }

    [PermanentGuid ("00000000-0000-0000-0002-000000000001")]
    public FileState State
    {
      get { return _state; }
      set { _state = value; }
    }

    public string Location
    {
      get { return _location; }
      set { _location = value; }
    }
  }
}
