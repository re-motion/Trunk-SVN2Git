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
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects.Mapping
{
[Serializable]
public class MappingException : ConfigurationException
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public MappingException () : this ("A mapping exception occurred.") {}
  public MappingException (string message) : base (message) {}
  public MappingException (string message, Exception inner) : base (message, inner) {}
  protected MappingException (SerializationInfo info, StreamingContext context) : base (info, context) {}

  // methods and properties

}
}
