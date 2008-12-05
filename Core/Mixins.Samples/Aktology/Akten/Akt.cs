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

namespace Remotion.Mixins.Samples.Aktology.Akten
{
  public interface IAkt
  {
    string ID { get; set; }
    string Inhalt { get; set; }
  }

  public class Akt : IAkt
  {
    private string _id;
    private string _inhalt;

    public string ID
    {
      get { return _id; }
      set { _id = value; }
    }

    public string Inhalt
    {
      get { return _inhalt; }
      set { _inhalt = value; }
    }
  }
}
