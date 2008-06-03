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

namespace Remotion.Data.DomainObjects.Infrastructure
{
  public class FlattenedSerializationReader<T>
  {
    private readonly T[] _data;
    private int _readPosition = 0;

    public FlattenedSerializationReader (T[] data)
    {
      ArgumentUtility.CheckNotNull ("data", data);

      _data = data;
    }

    public int ReadPosition
    {
      get { return _readPosition; }
    }

    public T ReadValue ()
    {
      if (_readPosition >= _data.Length)
        throw new InvalidOperationException (string.Format ("There is no more data in the serialization stream at position {0}.", _readPosition));

      T value = _data[_readPosition];
      ++_readPosition;
      return value;
    }
  }
}
