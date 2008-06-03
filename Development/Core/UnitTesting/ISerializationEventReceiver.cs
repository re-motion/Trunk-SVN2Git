/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Runtime.Serialization;

namespace Remotion.Development.UnitTesting
{
  public interface ISerializationEventReceiver
  {
    void OnDeserialization (object sender);
    void OnDeserialized (StreamingContext context);
    void OnDeserializing (StreamingContext context);
    void OnSerialized (StreamingContext context);
    void OnSerializing (StreamingContext context);
  }
}
