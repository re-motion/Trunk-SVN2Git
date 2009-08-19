// This file is part of the re-motion Core Framework (www.re-motion.org)
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

namespace Remotion.UnitTests.Mixins.Definitions.TestDomain
{
  public class ClassWithDifferentMemberVisibilities
  {
    // ReSharper disable EventNeverSubscribedTo.Local
    // ReSharper disable UnusedMember.Local
    // ReSharper disable UnusedAutoPropertyAccessor.Local
    public static void PublicStaticMethod ()
    {
    }

    public void PublicMethod ()
    {
    }

    protected void ProtectedMethod ()
    {
    }

    protected internal void ProtectedInternalMethod ()
    {
    }

    internal void InternalMethod ()
    {
    }

    private void PrivateMethod ()
    {
    }

    public int PublicProperty { get; set; }
    protected int ProtectedProperty { get; set; }
    protected internal int ProtectedInternalProperty { get; set; }
    internal int InternalProperty { get; set; }
    private int PrivateProperty { get; set; }
    public int PropertyWithPrivateSetter { get; private set; }

    public event EventHandler PublicEvent;
    protected event EventHandler ProtectedEvent;
    protected internal event EventHandler ProtectedInternalEvent;
    internal event EventHandler InternalEvent;
    private event EventHandler PrivateEvent;
    // ReSharper restore UnusedAutoPropertyAccessor.Local
    // ReSharper restore UnusedMember.Local
    // ReSharper restore EventNeverSubscribedTo.Local
  }
}