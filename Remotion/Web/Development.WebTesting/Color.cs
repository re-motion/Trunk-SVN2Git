// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Represents an RGB color (possiblity transparent) without any known-color-handling (like <see cref="System.Drawing.Color"/>).
  /// </summary>
  /// <remarks>
  /// This struct mainly exists because of some very confusing behaviors of <see cref="System.Drawing.Color"/>. Web test developers often do not have
  /// the sufficient coding skills to understand why <c>Color.FromArgb(255, 0, 0, 0) != Color.Black</c>.
  /// </remarks>
  public struct Color : IEquatable<Color>
  {
    public static readonly Color Transparent = new Color (true);
    public static readonly Color White = new Color (255, 255, 255);
    public static readonly Color Black = new Color (0, 0, 0);

    private readonly byte _red;
    private readonly byte _green;
    private readonly byte _blue;
    private readonly bool _isTransparent;

    public static Color FromRgb (byte r, byte g, byte b)
    {
      return new Color (r, g, b);
    }

    private Color (byte r, byte g, byte b)
    {
      _red = r;
      _green = g;
      _blue = b;
      _isTransparent = false;
    }

    private Color (bool isTransparent)
    {
      _red = 0;
      _green = 0;
      _blue = 0;
      _isTransparent = isTransparent;
    }

    public byte R { get { return _red; } }
    public byte G { get { return _green; } }
    public byte B { get { return _blue; } }
    public bool IsTransparent { get { return _isTransparent; } }

    public bool Equals (Color other)
    {
      return _red == other._red && _green == other._green && _blue == other._blue && _isTransparent == other._isTransparent;
    }

    public override bool Equals (object obj)
    {
      if (ReferenceEquals (null, obj))
        return false;
      return obj is Color && Equals ((Color) obj);
    }

    public override int GetHashCode ()
    {
      unchecked
      {
        var hashCode = _red.GetHashCode();
        hashCode = (hashCode * 397) ^ _green.GetHashCode();
        hashCode = (hashCode * 397) ^ _blue.GetHashCode();
        hashCode = (hashCode * 397) ^ _isTransparent.GetHashCode();
        return hashCode;
      }
    }

    public static bool operator == (Color left, Color right)
    {
      return left.Equals (right);
    }

    public static bool operator != (Color left, Color right)
    {
      return !left.Equals (right);
    }

    public override string ToString ()
    {
      if (IsTransparent)
        return "Transparent";

      return string.Format ("RGB=({0},{1},{2})", R, G, B);
    }
  }
}