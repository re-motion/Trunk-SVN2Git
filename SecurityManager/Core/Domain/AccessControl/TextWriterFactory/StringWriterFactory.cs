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
using System.Collections.Generic;
using Remotion.Diagnostics.ToText;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.TextWriterFactory
{
  public interface ITextWriterFactory
  {
    TextWriter NewTextWriter (string name);
    string GetRelativePath (string fromName, string toName);
  }

  public class StringWriterFactory : ITextWriterFactory, IToText
  {
    private readonly Dictionary<string, TextWriterData> nameToTextWriterData = new Dictionary<string, TextWriterData>();

    public TextWriter NewTextWriter (string name)
    {
      var textWriterData = new TextWriterData (new StringWriter());
      nameToTextWriterData[name] = textWriterData;
      return textWriterData.TextWriter;
    }

    public string GetRelativePath (string fromName, string toName)
    {
      if (!nameToTextWriterData.ContainsKey (toName))
      {
        throw new ArgumentException (To.String.s ("No TextWriter with name ").e (toName).s (" registered => no relative path exists.").CheckAndConvertToString ());
      }
      return ".\\" + toName;
    }


    public void ToText (IToTextBuilder toTextBuilder)
    {
      toTextBuilder.s ("StringWriterFactory");
      toTextBuilder.sb();
      foreach (KeyValuePair<string, TextWriterData> pair in nameToTextWriterData)
      {
        toTextBuilder.e (pair.Key);
      }
      toTextBuilder.se ();

      foreach (KeyValuePair<string, TextWriterData> pair in nameToTextWriterData)
      {
        toTextBuilder.nl().e (pair.Key).nl().e(pair.Value.TextWriter.ToString()).nl();
      }
    
    }
  }




  public class StreamWriterFactory : ITextWriterFactory, IToText
  {
    private readonly Dictionary<string, TextWriterData> nameToTextWriterData = new Dictionary<string, TextWriterData> ();
    private readonly string _directory;

    public StreamWriterFactory (string directory)
    {
      ArgumentUtility.CheckNotNull ("directory", directory);
      Directory.CreateDirectory (directory);
      _directory = directory;
    }

    public TextWriter NewTextWriter (string name)
    {
      if (nameToTextWriterData.ContainsKey (name))
      {
        throw new ArgumentException (To.String.s ("TextWriter with name ").e (name).s (" already exists.").CheckAndConvertToString ());
      }
      var textWriterData = new TextWriterData (new StreamWriter (Path.Combine (_directory, name)));
      nameToTextWriterData[name] = textWriterData;
      return textWriterData.TextWriter;
    }

    public string GetRelativePath (string fromName, string toName)
    {
      if (!nameToTextWriterData.ContainsKey (toName))
      {
        throw new ArgumentException (To.String.s ("No TextWriter with name ").e (toName).s (" exists => no relative path exists.").CheckAndConvertToString ());
      }
      return ".\\" + toName;
    }


    public void ToText (IToTextBuilder toTextBuilder)
    {
      toTextBuilder.s ("StringWriterFactory");
      toTextBuilder.sb ();
      foreach (KeyValuePair<string, TextWriterData> pair in nameToTextWriterData)
      {
        toTextBuilder.e (pair.Key);
      }
      toTextBuilder.se ();

      foreach (KeyValuePair<string, TextWriterData> pair in nameToTextWriterData)
      {
        toTextBuilder.nl ().e (pair.Key).nl ().e (pair.Value.TextWriter.ToString ()).nl ();
      }

    }
  }





  public class TextWriterData
  {
    public TextWriterData (TextWriter textWriter)
    {
      TextWriter = textWriter;
    }

    public TextWriter TextWriter { get; private set; }
  }
}