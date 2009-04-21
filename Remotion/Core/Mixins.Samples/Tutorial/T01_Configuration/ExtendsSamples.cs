﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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

namespace Remotion.Mixins.Samples.Tutorial.T01_Configuration
{
  namespace ExtendsSamples
  {
    [Extends (typeof (MyClass))]
    public class MyMixin
    {
    }

    public class MyClass
    {
    }

    public class File
    {
    }

    public class CarFile : File
    {
    }

    public class PersonFile : File
    {
    }

    [Extends (typeof (File))]
    public class NumberedFileMixin : INumberedFile
    {
      private readonly string _id = Guid.NewGuid ().ToString();
      
      public string GetFileNumber ()
      {
        return _id;
      }
    }

    public interface INumberedFile
    {
      string GetFileNumber ();
    }
  }
}