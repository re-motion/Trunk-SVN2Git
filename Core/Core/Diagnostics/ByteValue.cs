/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.Diagnostics
{
  public struct ByteValue
  {
    private readonly long _bytes;

    public ByteValue (long bytes)
        : this ()
    {
      _bytes = bytes;
    }

    public long Bytes
    {
      get { return _bytes; }
    }

    public decimal MegaBytes { get { return Bytes / 1024.0m / 1024.0m; } }

    public override string ToString ()
    {
      if (MegaBytes > 1)
        return MegaBytes.ToString ("N2") + " MB";
      else
        return Bytes.ToString ("N0") + " bytes";
    }

    public string ToDifferenceString ()
    {
      if (Bytes > 0)
        return "+" + ToString ();
      else
        return ToString ();
    }

    public static ByteValue operator + (ByteValue left, ByteValue right)
    {
      return new ByteValue (left.Bytes + right.Bytes);
    }

    public static ByteValue operator - (ByteValue left, ByteValue right)
    {
      return new ByteValue (left.Bytes - right.Bytes);
    }
  }
}