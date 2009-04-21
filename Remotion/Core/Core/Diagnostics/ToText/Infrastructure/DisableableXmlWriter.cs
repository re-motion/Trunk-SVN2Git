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
using System.IO;
using System.Xml;

namespace Remotion.Diagnostics.ToText.Infrastructure
{
  /// <summary>
  /// Wrapper around <see cref="XmlWriter"/> class which supports enabling/disabling of its <see cref="WriteStartElement"/>,etc methods
  /// through its <see cref="Enabled"/> property.
  /// </summary>
  internal class DisableableXmlWriter : IDisposable
  {
    private readonly XmlWriter _xmlWriter;


    public DisableableXmlWriter (XmlWriter xmlWriter)
    {
      _xmlWriter = xmlWriter;
      Enabled = true;
    }

    public bool Enabled { get; set; }

    public void WriteStartElement (string tag)
    {
      if (Enabled)
      {
        _xmlWriter.WriteStartElement (tag);
      }
    }


    public void WriteEndElementAlways ()
    {
      _xmlWriter.WriteEndElement ();
    }

    public void WriteEndElement ()
    {
      if (Enabled)
      {
        //_xmlWriter.WriteEndElement ();
        WriteEndElementAlways();
      }
    }


    public void WriteValue (char c)
    {
      if (Enabled)
      {
        _xmlWriter.WriteValue (Char.ToString(c));
      }
    }
    
    public void WriteValue (object obj)
    {
      if (Enabled)
      {
        _xmlWriter.WriteValue (obj);
      }
    }


    public void WriteAttribute (string name, string value)
    {
      if (Enabled)
      {
        _xmlWriter.WriteAttributeString (name, value);
      }
    }


    public void Flush ()
    {
      _xmlWriter.Flush ();
    }

    public void WriteAttributeIfNotEmpty (string name, string value)
    {
      if (!String.IsNullOrEmpty(value))
      {
        WriteAttribute (name, value);
      }
    }

    public void Close ()
    {
      _xmlWriter.Close();
    }

    void IDisposable.Dispose ()
    {
      Close();
    }
  }
}