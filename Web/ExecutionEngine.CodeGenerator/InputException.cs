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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.CodeGenerator
{
  public enum InputError
  {
    InvalidSchema = 1,

    [EnumDescription ("Could not detect class declaration.")]
    ClassNotFound = 2,

    [EnumDescription ("Error parsing XML.")]
    XmlError = 3,

    Unknown = 4
  }
  
  public class InputException : Exception
  {
    private string _path;
    private int _line;
    private int _position;
    private int _errorCode;

    public InputException (InputError error, string path, int line, int position, Exception innerException)
      : base (
          (innerException != null) ? innerException.Message : EnumDescription.GetDescription (error), 
          innerException)
    {
      _path = path;
      _line = line;
      _position = position;
      _errorCode = (int) error;
    }

    public InputException (InputError error, string path, int line, int position)
      : this (error, path, line, position, null)
    {
    }

    public string Path
    {
      get { return _path; }
    }

    public int Line
    {
      get { return _line; }
    }

    public int Position
    {
      get { return _position; }
    }

    public int ErrorCode
    {
      get { return _errorCode; }
    }
  }
}