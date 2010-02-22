// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.IO;
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class AssemblySaverTest
  {
    [Test]
    public void SaveAssemblyNoGeneratedTypes ()
    {
      ModuleScope scope = new ModuleScope (true);
      string[] paths = AssemblySaver.SaveAssemblies (scope);
      Assert.AreEqual (0, paths.Length);
    }

    [Test]
    public void SaveAssemblySigned ()
    {
      ModuleScope scope = new ModuleScope (true);
      CustomClassEmitter emitter = new CustomClassEmitter (scope, "SignedType", typeof (object));
      emitter.BuildType ();
      string[] paths = AssemblySaver.SaveAssemblies (scope);
      Assert.AreEqual (1, paths.Length);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, scope.StrongNamedModuleName), paths[0]);
      File.Delete (paths[0]);
      File.Delete (paths[0].Replace (".dll", ".pdb"));
    }

    [Test]
    public void SaveAssemblyUnsigned ()
    {
      ModuleScope scope = new ModuleScope (true);
      CustomClassEmitter emitter = new CustomClassEmitter (scope, "UnsignedType", typeof (object), Type.EmptyTypes, TypeAttributes.Public, true);
      emitter.BuildType ();
      string[] paths = AssemblySaver.SaveAssemblies (scope);
      Assert.AreEqual (1, paths.Length);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, scope.WeakNamedModuleName), paths[0]);
      File.Delete (paths[0]);
      File.Delete (paths[0].Replace (".dll", ".pdb"));
    }
  }
}
