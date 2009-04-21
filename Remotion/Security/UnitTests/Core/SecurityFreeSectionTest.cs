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
      Assert.IsFalse (SecurityFreeSection.IsActive);
      SecurityFreeSection section = new SecurityFreeSection ();

      Assert.IsTrue (SecurityFreeSection.IsActive);

      section.Leave ();
      Assert.IsFalse (SecurityFreeSection.IsActive);
    }

    [Test]
    public void Enter_IsActive_Leave_WithNestedSecurityFreeSections ()
    {
      Assert.IsFalse (SecurityFreeSection.IsActive);
      SecurityFreeSection section1 = new SecurityFreeSection ();

      Assert.IsTrue (SecurityFreeSection.IsActive);

      using (new SecurityFreeSection ())
      {
        Assert.IsTrue (SecurityFreeSection.IsActive);
      }

      Assert.IsTrue (SecurityFreeSection.IsActive);

      section1.Leave ();
      Assert.IsFalse (SecurityFreeSection.IsActive);
    }

    [Test]
    public void Enter_IsActive_Leave_WithNestedSecurityFreeSectionsUnorderd ()
    {
      Assert.IsFalse (SecurityFreeSection.IsActive);
      SecurityFreeSection section1 = new SecurityFreeSection ();

      Assert.IsTrue (SecurityFreeSection.IsActive);

      SecurityFreeSection section2 = new SecurityFreeSection ();
      Assert.IsTrue (SecurityFreeSection.IsActive);

      section1.Leave ();
      Assert.IsTrue (SecurityFreeSection.IsActive);

      section2.Leave ();
      Assert.IsFalse (SecurityFreeSection.IsActive);
    }

    [Test]
    public void Dispose ()
    {
      Assert.IsFalse (SecurityFreeSection.IsActive);
      IDisposable section = new SecurityFreeSection ();

      Assert.IsTrue (SecurityFreeSection.IsActive);

      section.Dispose ();
      Assert.IsFalse (SecurityFreeSection.IsActive);
    }

    [Test]
    public void Enter_IsActive_Leave_Enter ()
    {
      Assert.IsFalse (SecurityFreeSection.IsActive);
      SecurityFreeSection section = new SecurityFreeSection ();

      Assert.IsTrue (SecurityFreeSection.IsActive);

      section.Leave ();
      section.Leave ();
      Assert.IsFalse (SecurityFreeSection.IsActive);

      using (new SecurityFreeSection ())
      {
        Assert.IsTrue (SecurityFreeSection.IsActive);
      }
    }

    [Test]
    public void Threading ()
    {
      Assert.IsFalse (SecurityFreeSection.IsActive);
      SecurityFreeSection section = new SecurityFreeSection ();
      Assert.IsTrue (SecurityFreeSection.IsActive);

      ThreadRunner.Run (delegate ()
          {
            Assert.IsFalse (SecurityFreeSection.IsActive);
            using (new SecurityFreeSection ())
            {
              Assert.IsTrue (SecurityFreeSection.IsActive);
            }
            Assert.IsFalse (SecurityFreeSection.IsActive);
          });

      section.Leave ();
      Assert.IsFalse (SecurityFreeSection.IsActive);
    }
  }
}
