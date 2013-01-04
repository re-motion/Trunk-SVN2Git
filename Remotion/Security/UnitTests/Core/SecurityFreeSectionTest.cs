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
using NUnit.Framework;
using Remotion.Development.UnitTesting;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class SecurityFreeSectionTest
  {
    [Test]
    public void Enter_IsActive_Leave_WithSingleSecurityFreeSection ()
    {
      Assert.That (SecurityFreeSection.IsActive, Is.False);
      SecurityFreeSection section = new SecurityFreeSection ();

      Assert.That (SecurityFreeSection.IsActive, Is.True);

      section.Leave ();
      Assert.That (SecurityFreeSection.IsActive, Is.False);
    }

    [Test]
    public void Enter_IsActive_Leave_WithNestedSecurityFreeSections ()
    {
      Assert.That (SecurityFreeSection.IsActive, Is.False);
      SecurityFreeSection section1 = new SecurityFreeSection ();

      Assert.That (SecurityFreeSection.IsActive, Is.True);

      using (new SecurityFreeSection ())
      {
        Assert.That (SecurityFreeSection.IsActive, Is.True);
      }

      Assert.That (SecurityFreeSection.IsActive, Is.True);

      section1.Leave ();
      Assert.That (SecurityFreeSection.IsActive, Is.False);
    }

    [Test]
    public void Enter_IsActive_Leave_WithNestedSecurityFreeSectionsUnorderd ()
    {
      Assert.That (SecurityFreeSection.IsActive, Is.False);
      SecurityFreeSection section1 = new SecurityFreeSection ();

      Assert.That (SecurityFreeSection.IsActive, Is.True);

      SecurityFreeSection section2 = new SecurityFreeSection ();
      Assert.That (SecurityFreeSection.IsActive, Is.True);

      section1.Leave ();
      Assert.That (SecurityFreeSection.IsActive, Is.True);

      section2.Leave ();
      Assert.That (SecurityFreeSection.IsActive, Is.False);
    }

    [Test]
    public void Dispose ()
    {
      Assert.That (SecurityFreeSection.IsActive, Is.False);
      IDisposable section = new SecurityFreeSection ();

      Assert.That (SecurityFreeSection.IsActive, Is.True);

      section.Dispose ();
      Assert.That (SecurityFreeSection.IsActive, Is.False);
    }

    [Test]
    public void Enter_IsActive_Leave_Enter ()
    {
      Assert.That (SecurityFreeSection.IsActive, Is.False);
      SecurityFreeSection section = new SecurityFreeSection ();

      Assert.That (SecurityFreeSection.IsActive, Is.True);

      section.Leave ();
      section.Leave ();
      Assert.That (SecurityFreeSection.IsActive, Is.False);

      using (new SecurityFreeSection ())
      {
        Assert.That (SecurityFreeSection.IsActive, Is.True);
      }
    }

    [Test]
    public void Threading ()
    {
      Assert.That (SecurityFreeSection.IsActive, Is.False);
      SecurityFreeSection section = new SecurityFreeSection ();
      Assert.That (SecurityFreeSection.IsActive, Is.True);

      ThreadRunner.Run (delegate ()
          {
            Assert.That (SecurityFreeSection.IsActive, Is.False);
            using (new SecurityFreeSection ())
            {
              Assert.That (SecurityFreeSection.IsActive, Is.True);
            }
            Assert.That (SecurityFreeSection.IsActive, Is.False);
          });

      section.Leave ();
      Assert.That (SecurityFreeSection.IsActive, Is.False);
    }
  }
}
