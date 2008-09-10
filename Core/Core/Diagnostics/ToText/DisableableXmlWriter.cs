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
using System.IO;
using System.Xml;

namespace Remotion.Diagnostics.ToText
{
  /// <summary>
  /// Wrapper around <see cref="XmlWriter"/> class which supports enabling/disabling of its <see cref="WriteStartElement"/>,etc methods
  /// through its <see cref="Enabled"/> property.
  /// </summary>
  internal class DisableableXmlWriter
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

    public void WriteEndElement ()
    {
      if (Enabled)
      {
        _xmlWriter.WriteEndElement ();
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
  }
}