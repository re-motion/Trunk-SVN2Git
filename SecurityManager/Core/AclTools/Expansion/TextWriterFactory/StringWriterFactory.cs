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

namespace Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory
{
  public interface ITextWriterFactory
  {
    TextWriter NewTextWriter (string name);
    string GetRelativePath (string fromName, string toName);

    string Directory { get; set; }
    string Extension { get; set; }
  }

  public class StringWriterFactory : TextWriterFactoryBase, IToText
  {
    //private readonly Dictionary<string, TextWriterData> nameToTextWriterData = new Dictionary<string, TextWriterData>();

    public override TextWriter NewTextWriter (string name)
    {
      //string nameWithExtension = AppendExtension (name);
      var textWriterData = new TextWriterData (new StringWriter(),Directory,Extension);
      nameToTextWriterData[name] = textWriterData;
      return textWriterData.TextWriter;
    }

    //public override string GetRelativePath (string fromName, string toName)
    //{
    //  if (!nameToTextWriterData.ContainsKey (toName))
    //  {
    //    throw new ArgumentException (To.String.s ("No TextWriter with name ").e (toName).s (" registered => no relative path exists.").CheckAndConvertToString ());
    //  }
    //  return ".\\" + toName;
    //}

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

  public abstract class TextWriterFactoryBase : ITextWriterFactory
  {
    protected readonly Dictionary<string, TextWriterData> nameToTextWriterData = new Dictionary<string, TextWriterData> ();
    public abstract TextWriter NewTextWriter (string name);
    //public abstract string GetRelativePath (string fromName, string toName);
    public string Directory { get; set; }
    public string Extension { get; set; }
    protected string AppendExtension(string name)
    {
      if (String.IsNullOrEmpty (Extension))
      {
        return name;
      }
      else
      {
        return name + "." + Extension;
      }
    }

    public string GetRelativePath (string fromName, string toName)
    {
      if (!nameToTextWriterData.ContainsKey (toName))
      {
        throw new ArgumentException (To.String.s ("No TextWriter with name ").e (toName).s (" registered => no relative path exists.").CheckAndConvertToString ());
      }
      return ".\\" + AppendExtension(toName);
    }

    public TextWriterData GetTextWriterData (string name)
    {
      return nameToTextWriterData[name];
    }

    public int Count { get { return nameToTextWriterData.Count; } }
  }

  public class StreamWriterFactory : TextWriterFactoryBase, ITextWriterFactory, IToText
  {
    //private readonly Dictionary<string, TextWriterData> nameToTextWriterData = new Dictionary<string, TextWriterData> ();

    //public StreamWriterFactory (string directory)
    //{
    //  ArgumentUtility.CheckNotNull ("directory", directory);
    //  Directory.CreateDirectory (directory);
    //  _directory = directory;
    //}

    public StreamWriterFactory ()
    {
    }


    public override TextWriter NewTextWriter (string name)
    {
      Assertion.IsNotNull (Directory, "Directory must not be null. Set using \"Directory\"-property before calling \"NewTextWriter\"");
      if (!System.IO.Directory.Exists (Directory))
      {
        System.IO.Directory.CreateDirectory (Directory);
      }

      if (nameToTextWriterData.ContainsKey (name))
      {
        throw new ArgumentException (To.String.s ("TextWriter with name ").e (name).s (" already exists.").CheckAndConvertToString ());
      }
      var textWriterData = new TextWriterData (new StreamWriter (Path.Combine (Directory, AppendExtension(name))), Directory, Extension);
      nameToTextWriterData[name] = textWriterData;
      return textWriterData.TextWriter;
    }

    //public override string GetRelativePath (string fromName, string toName)
    //{
    //  if (!nameToTextWriterData.ContainsKey (toName))
    //  {
    //    throw new ArgumentException (To.String.s ("No TextWriter with name ").e (toName).s (" exists => no relative path exists.").CheckAndConvertToString ());
    //  }
    //  return ".\\" + toName;
    //}


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
    public TextWriterData (TextWriter textWriter, string directory, string extension)
    {
      TextWriter = textWriter;
      Directory = directory;
      Extension = extension;
    }

    public TextWriter TextWriter { get; private set; }
    public string Directory { get; private set; }
    public string Extension { get; private set; }
  }
}