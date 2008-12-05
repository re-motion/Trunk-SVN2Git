// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core
{
  [TestFixture]
  public class BusinessObjectDataSourceTest
  {
    private MockRepository _mockRepository;
    private IBusinessObjectDataSource _dataSource;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _dataSource = new StubBusinessObjectDataSource (null);
    }

    [Test]
    public void GetBusinessObjectProvider ()
    {
      IBusinessObjectClass stubBusinessObjectClass = _mockRepository.Stub<IBusinessObjectClass>();
      IBusinessObjectProvider stubBusinessObjectProvider = _mockRepository.Stub<IBusinessObjectProvider>();
      IBusinessObjectDataSource dataSource = new StubBusinessObjectDataSource (stubBusinessObjectClass);

      SetupResult.For (stubBusinessObjectClass.BusinessObjectProvider).Return (stubBusinessObjectProvider);
      _mockRepository.ReplayAll();

      Assert.That (dataSource.BusinessObjectProvider, Is.SameAs (stubBusinessObjectProvider));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetBusinessObjectProvider_WithoutbusinessObjectClass ()
    {
      _dataSource = new StubBusinessObjectDataSource (null);
      Assert.That (_dataSource.BusinessObjectProvider, Is.Null);
    }

    [Test]
    public void RegisterAndGetBoundControls ()
    {
      IBusinessObjectBoundControl stubControl1 = _mockRepository.Stub<IBusinessObjectBoundControl>();
      IBusinessObjectBoundControl stubControl2 = _mockRepository.Stub<IBusinessObjectBoundControl>();

      SetupResult.For (stubControl1.HasValidBinding).Return (true);
      SetupResult.For (stubControl2.HasValidBinding).Return (true);
      _mockRepository.ReplayAll();

      _dataSource.Register (stubControl1);
      _dataSource.Register (stubControl2);
      Assert.That (_dataSource.BoundControls, Is.EqualTo (new IBusinessObjectBoundControl[] {stubControl1, stubControl2}));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Unregister ()
    {
      IBusinessObjectBoundControl stubControl1 = _mockRepository.Stub<IBusinessObjectBoundControl> ();
      IBusinessObjectBoundControl stubControl2 = _mockRepository.Stub<IBusinessObjectBoundControl> ();

      SetupResult.For (stubControl1.HasValidBinding).Return (true);
      SetupResult.For (stubControl2.HasValidBinding).Return (true);
      _mockRepository.ReplayAll ();

      _dataSource.Register (stubControl1);
      _dataSource.Register (stubControl2);
      Assert.That (_dataSource.BoundControls, Is.EqualTo (new IBusinessObjectBoundControl[] { stubControl1, stubControl2 }));

      _dataSource.Unregister (stubControl1);
      Assert.That (_dataSource.BoundControls, Is.EqualTo (new IBusinessObjectBoundControl[] { stubControl2 }));

      _dataSource.Unregister (stubControl2);
      Assert.That (_dataSource.BoundControls, Is.EqualTo (new IBusinessObjectBoundControl[0]));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Register_SameTwice ()
    {
      IBusinessObjectBoundControl stubControl1 = _mockRepository.Stub<IBusinessObjectBoundControl> ();
      IBusinessObjectBoundControl stubControl2 = _mockRepository.Stub<IBusinessObjectBoundControl> ();

      SetupResult.For (stubControl1.HasValidBinding).Return (true);
      SetupResult.For (stubControl2.HasValidBinding).Return (true);
      _mockRepository.ReplayAll ();

      _dataSource.Register (stubControl1);
      _dataSource.Register (stubControl2);
      _dataSource.Register (stubControl1);
      Assert.That (_dataSource.BoundControls, Is.EqualTo (new IBusinessObjectBoundControl[] { stubControl1, stubControl2 }));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Unegister_SameTwice ()
    {
      IBusinessObjectBoundControl stubControl1 = _mockRepository.Stub<IBusinessObjectBoundControl> ();
      IBusinessObjectBoundControl stubControl2 = _mockRepository.Stub<IBusinessObjectBoundControl> ();

      SetupResult.For (stubControl1.HasValidBinding).Return (true);
      SetupResult.For (stubControl2.HasValidBinding).Return (true);
      _mockRepository.ReplayAll ();

      _dataSource.Register (stubControl1);
      _dataSource.Register (stubControl2);
      Assert.That (_dataSource.BoundControls, Is.EqualTo (new IBusinessObjectBoundControl[] { stubControl1, stubControl2 }));

      _dataSource.Unregister (stubControl1);
      _dataSource.Unregister (stubControl1);
      Assert.That (_dataSource.BoundControls, Is.EqualTo (new IBusinessObjectBoundControl[] { stubControl2 }));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void LoadValues ()
    {
      IBusinessObjectBoundControl mockControl1 = _mockRepository.StrictMock<IBusinessObjectBoundControl> ();
      IBusinessObjectBoundControl mockControl2 = _mockRepository.StrictMock<IBusinessObjectBoundControl> ();

      SetupResult.For (mockControl1.HasValidBinding).Return (true);
      SetupResult.For (mockControl2.HasValidBinding).Return (true);
      mockControl1.LoadValue (true);
      mockControl2.LoadValue (true);
      _mockRepository.ReplayAll ();

      _dataSource.Register (mockControl1);
      _dataSource.Register (mockControl2);
      _dataSource.LoadValues (true);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void SaveValues ()
    {
      IBusinessObjectBoundEditableControl mockControl1 = _mockRepository.StrictMock<IBusinessObjectBoundEditableControl> ();
      IBusinessObjectBoundEditableControl mockControl2 = _mockRepository.StrictMock<IBusinessObjectBoundEditableControl> ();

      SetupResult.For (mockControl1.HasValidBinding).Return (true);
      SetupResult.For (mockControl2.HasValidBinding).Return (true);
      mockControl1.SaveValue (true);
      mockControl2.SaveValue (true);
      _mockRepository.ReplayAll ();

      _dataSource.Register (mockControl1);
      _dataSource.Register (mockControl2);
      _dataSource.SaveValues (true);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void SetAndGetMode ()
    {
      _dataSource.Mode = DataSourceMode.Edit;
      Assert.That (_dataSource.Mode, Is.EqualTo (DataSourceMode.Edit));
    }
  }
}
