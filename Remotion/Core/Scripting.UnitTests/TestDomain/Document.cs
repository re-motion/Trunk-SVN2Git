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

namespace Remotion.Scripting.UnitTests.TestDomain
{
  public class Document 
  {
    public string DocumentName { get; set; }
    public int DocumentNumber { get; set; }

    public Document (string name) : this (name,0)
    {
    }

    public Document (string name, int number)
    {
      DocumentName = name;
      DocumentNumber = number;
    }
  }
}