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
using System.Collections;
using System.Web;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ServiceLocation;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocColumnRendererArrayBuilderTest
  {
    private StubColumnDefinition _stubColumnDefinition;
    private IServiceLocator _serviceLocator;
    private WcagHelper _wcagHelperStub;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = new DefaultServiceLocator();
      _stubColumnDefinition = new StubColumnDefinition();
      _wcagHelperStub = MockRepository.GenerateStub<WcagHelper>();
    }

    [Test]
    public void GetColumnRenderers ()
    {
      var builder = new BocColumnRendererArrayBuilder (new[] { _stubColumnDefinition }, _serviceLocator, _wcagHelperStub);

      var bocColumnRenderers = builder.CreateColumnRenderers ();

      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (PrivateInvoke.GetNonPublicField (bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf (typeof (StubColumnRenderer)));
      Assert.That (bocColumnRenderers[0].ColumnDefinition, Is.SameAs (_stubColumnDefinition));
      Assert.That (bocColumnRenderers[0].ColumnIndex, Is.EqualTo (0));
    }

    [Test]
    public void GetColumnRenderers_PrepareSorting_IsClientSideSortingEnabledIsFalseAndHasSortingKeysIsFalse ()
    {
      var builder = new BocColumnRendererArrayBuilder (new[] { new BocSimpleColumnDefinition() }, _serviceLocator, _wcagHelperStub);
      builder.SortingOrder = new ArrayList (new[] { new BocListSortingOrderEntryMock (0, SortingDirection.Ascending) });
      builder.IsClientSideSortingEnabled = false;
      builder.HasSortingKeys = false;

      var bocColumnRenderers = builder.CreateColumnRenderers ();

      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].SortingDirection, Is.EqualTo (SortingDirection.None));
      Assert.That (bocColumnRenderers[0].OrderIndex, Is.EqualTo (-1));
    }

    [Test]
    public void GetColumnRenderers_PrepareSorting_IsClientSideSortingEnabledIsTrueAndHasSortingKeysIsFalse ()
    {
      var builder = new BocColumnRendererArrayBuilder (new[] { new BocSimpleColumnDefinition() }, _serviceLocator, _wcagHelperStub);
      builder.SortingOrder = new ArrayList (new[] { new BocListSortingOrderEntryMock (0, SortingDirection.Ascending) });
      builder.IsClientSideSortingEnabled = true;
      builder.HasSortingKeys = false;

      var bocColumnRenderers = builder.CreateColumnRenderers ();

      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].SortingDirection, Is.EqualTo (SortingDirection.Ascending));
      Assert.That (bocColumnRenderers[0].OrderIndex, Is.EqualTo (0));
    }

    [Test]
    public void GetColumnRenderers_PrepareSorting_IsClientSideSortingEnabledIsFalseAndHasSortingKeysIsTrue ()
    {
      var builder = new BocColumnRendererArrayBuilder (new[] { new BocSimpleColumnDefinition() }, _serviceLocator, _wcagHelperStub);
      builder.SortingOrder = new ArrayList (new[] { new BocListSortingOrderEntryMock (0, SortingDirection.Ascending) });
      builder.IsClientSideSortingEnabled = false;
      builder.HasSortingKeys = true;

      var bocColumnRenderers = builder.CreateColumnRenderers ();

      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].SortingDirection, Is.EqualTo (SortingDirection.Ascending));
      Assert.That (bocColumnRenderers[0].OrderIndex, Is.EqualTo (0));
    }

    [Test]
    public void GetColumnRenderers_PrepareSorting_SeveralColumns ()
    {
      var builder =
          new BocColumnRendererArrayBuilder (
              new[] { new BocSimpleColumnDefinition(), new BocSimpleColumnDefinition(), new BocSimpleColumnDefinition() },
              _serviceLocator,
              _wcagHelperStub);
      builder.SortingOrder =
          new ArrayList (
              new[]
              {
                  new BocListSortingOrderEntryMock (0, SortingDirection.Ascending), 
                  new BocListSortingOrderEntryMock (1, SortingDirection.Descending),
                  new BocListSortingOrderEntryMock (2, SortingDirection.None)
              });
      builder.IsClientSideSortingEnabled = true;
      builder.HasSortingKeys = true;

      var bocColumnRenderers = builder.CreateColumnRenderers ();

      Assert.That (bocColumnRenderers.Length, Is.EqualTo (3));
      Assert.That (bocColumnRenderers[0].SortingDirection, Is.EqualTo (SortingDirection.Ascending));
      Assert.That (bocColumnRenderers[0].OrderIndex, Is.EqualTo (0));
      Assert.That (bocColumnRenderers[1].SortingDirection, Is.EqualTo (SortingDirection.Descending));
      Assert.That (bocColumnRenderers[1].OrderIndex, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[2].SortingDirection, Is.EqualTo (SortingDirection.None));
      Assert.That (bocColumnRenderers[2].OrderIndex, Is.EqualTo (-1));
    }

    [Test]
    public void GetColumnRenderers_BocValueColumnDefinition_EnableIconIsFalse ()
    {
      var builder = new BocColumnRendererArrayBuilder (new[] { new BocSimpleColumnDefinition() }, _serviceLocator, _wcagHelperStub);
      builder.EnableIcon = false;

      var bocColumnRenderers = builder.CreateColumnRenderers ();

      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].ShowIcon, Is.False);
    }

    [Test]
    public void GetColumnRenderers_BocValueColumnDefinition_EnableIconIsTrue ()
    {
      var builder = new BocColumnRendererArrayBuilder (new[] { new BocSimpleColumnDefinition() }, _serviceLocator, _wcagHelperStub);
      builder.EnableIcon = true;

      var bocColumnRenderers = builder.CreateColumnRenderers ();

      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].ShowIcon, Is.True);
    }

    [Test]
    public void GetColumnRenderers_SeveralBocValueColumnDefinitions_EnableIconIsTrue ()
    {
      var builder =
          new BocColumnRendererArrayBuilder (
              new[] { new BocSimpleColumnDefinition(), new BocSimpleColumnDefinition(), new BocSimpleColumnDefinition() },
              _serviceLocator,
              _wcagHelperStub);
      builder.EnableIcon = true;

      var bocColumnRenderers = builder.CreateColumnRenderers ();

      Assert.That (bocColumnRenderers.Length, Is.EqualTo (3));
      Assert.That (bocColumnRenderers[0].ShowIcon, Is.True);
      Assert.That (bocColumnRenderers[1].ShowIcon, Is.False);
      Assert.That (bocColumnRenderers[2].ShowIcon, Is.False);
    }

    [Test]
    public void IsColumnVisible_BocCommandColumnDefinition_IsWaiConformanceLevelARequiredIsTrueAndCommandTypeIsNone ()
    {
      var columnDefinition = new BocCommandColumnDefinition();
      columnDefinition.Command = new BocListItemCommand (CommandType.None);
      var builder = new BocColumnRendererArrayBuilder (new[] { columnDefinition }, _serviceLocator, _wcagHelperStub);

      _wcagHelperStub.Stub (stub => stub.IsWaiConformanceLevelARequired()).Return (true);

      var bocColumnRenderers = builder.CreateColumnRenderers ();
      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].IsVisibleColumn, Is.True);
      Assert.That (PrivateInvoke.GetNonPublicField (bocColumnRenderers[0], "_columnRenderer"), Is.Not.TypeOf (typeof (NullColumnRenderer)));
    }

    [Test]
    public void IsColumnVisible_BocCommandColumnDefinition_IsWaiConformanceLevelARequiredIsFalseAndCommandTypeIsEvent ()
    {
      var columnDefinition = new BocCommandColumnDefinition();
      columnDefinition.Command = new BocListItemCommand (CommandType.Event);
      var builder = new BocColumnRendererArrayBuilder (new[] { columnDefinition }, _serviceLocator, _wcagHelperStub);

      _wcagHelperStub.Stub (stub => stub.IsWaiConformanceLevelARequired()).Return (false);

      var bocColumnRenderers = builder.CreateColumnRenderers ();
      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].IsVisibleColumn, Is.True);
      Assert.That (PrivateInvoke.GetNonPublicField (bocColumnRenderers[0], "_columnRenderer"), Is.Not.TypeOf (typeof (NullColumnRenderer)));
    }

    [Test]
    public void IsColumnVisible_BocCommandColumnDefinition_IsWaiConformanceLevelARequiredIsFalseAndCommandTypeIsWxeFunction ()
    {
      var columnDefinition = new BocCommandColumnDefinition();
      columnDefinition.Command = new BocListItemCommand (CommandType.WxeFunction);
      var builder = new BocColumnRendererArrayBuilder (new[] { columnDefinition }, _serviceLocator, _wcagHelperStub);

      _wcagHelperStub.Stub (stub => stub.IsWaiConformanceLevelARequired()).Return (false);

      var bocColumnRenderers = builder.CreateColumnRenderers ();
      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].IsVisibleColumn, Is.True);
      Assert.That (PrivateInvoke.GetNonPublicField (bocColumnRenderers[0], "_columnRenderer"), Is.Not.TypeOf (typeof (NullColumnRenderer)));
    }

    [Test]
    public void IsColumnVisible_BocCommandColumnDefinition_IsWaiConformanceLevelARequiredIsTrueAndCommandTypeIsEvent ()
    {
      var columnDefinition = new BocCommandColumnDefinition();
      columnDefinition.Command = new BocListItemCommand (CommandType.Event);
      var builder = new BocColumnRendererArrayBuilder (new[] { columnDefinition }, _serviceLocator, _wcagHelperStub);

      _wcagHelperStub.Stub (stub => stub.IsWaiConformanceLevelARequired()).Return (true);

      var bocColumnRenderers = builder.CreateColumnRenderers ();
      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].IsVisibleColumn, Is.False);
      Assert.That (PrivateInvoke.GetNonPublicField (bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf (typeof (NullColumnRenderer)));
    }

    [Test]
    public void IsColumnVisible_BocCommandColumnDefinition_IsWaiConformanceLevelARequiredIsTrueAndCommandTypeIsWxeFunction ()
    {
      var columnDefinition = new BocCommandColumnDefinition();
      columnDefinition.Command = new BocListItemCommand (CommandType.WxeFunction);
      var builder = new BocColumnRendererArrayBuilder (new[] { columnDefinition }, _serviceLocator, _wcagHelperStub);

      _wcagHelperStub.Stub (stub => stub.IsWaiConformanceLevelARequired()).Return (true);

      var bocColumnRenderers = builder.CreateColumnRenderers ();
      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].IsVisibleColumn, Is.False);
      Assert.That (PrivateInvoke.GetNonPublicField (bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf (typeof (NullColumnRenderer)));
    }

    [Test]
    public void IsColumnVisible_BocRowEditModeColumnDefinition_IsWaiConformanceLevelARequiredIsTrue ()
    {
      var columnDefinition = new BocRowEditModeColumnDefinition();
      var builder = new BocColumnRendererArrayBuilder (new[] { columnDefinition }, _serviceLocator, _wcagHelperStub);

      _wcagHelperStub.Stub (stub => stub.IsWaiConformanceLevelARequired()).Return (true);

      var bocColumnRenderers = builder.CreateColumnRenderers ();
      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].IsVisibleColumn, Is.False);
      Assert.That (PrivateInvoke.GetNonPublicField (bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf (typeof (NullColumnRenderer)));
    }

    [Test]
    public void IsColumnVisible_BocRowEditModeColumnDefinition_IsWaiConformanceLevelARequiredIsFalseAndIsListReadOnlyIsTrueAndEditModeIsFalse ()
    {
      var columnDefinition = new BocRowEditModeColumnDefinition();
      columnDefinition.Show = BocRowEditColumnDefinitionShow.Always;
      var builder = new BocColumnRendererArrayBuilder (new[] { columnDefinition }, _serviceLocator, _wcagHelperStub);
      builder.IsListReadOnly = true;

      _wcagHelperStub.Stub (stub => stub.IsWaiConformanceLevelARequired()).Return (false);

      var bocColumnRenderers = builder.CreateColumnRenderers ();
      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].IsVisibleColumn, Is.True);
      Assert.That (PrivateInvoke.GetNonPublicField (bocColumnRenderers[0], "_columnRenderer"), Is.Not.TypeOf (typeof (NullColumnRenderer)));
    }

    [Test]
    public void IsColumnVisible_BocRowEditModeColumnDefinition_IsWaiConformanceLevelARequiredIsFalseAndIsListReadOnlyIsFalseAndEditModeIsTrue ()
    {
      var columnDefinition = new BocRowEditModeColumnDefinition();
      columnDefinition.Show = BocRowEditColumnDefinitionShow.EditMode;
      var builder = new BocColumnRendererArrayBuilder (new[] { columnDefinition }, _serviceLocator, _wcagHelperStub);
      builder.IsListReadOnly = false;

      _wcagHelperStub.Stub (stub => stub.IsWaiConformanceLevelARequired()).Return (false);

      var bocColumnRenderers = builder.CreateColumnRenderers ();
      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].IsVisibleColumn, Is.True);
      Assert.That (PrivateInvoke.GetNonPublicField (bocColumnRenderers[0], "_columnRenderer"), Is.Not.TypeOf (typeof (NullColumnRenderer)));
    }

    [Test]
    public void IsColumnVisible_BocRowEditModeColumnDefinition_IsWaiConformanceLevelARequiredIsFalseAndIsListReadOnlyIsTrueAndEditModeIsTrue ()
    {
      var columnDefinition = new BocRowEditModeColumnDefinition();
      columnDefinition.Show = BocRowEditColumnDefinitionShow.EditMode;
      var builder = new BocColumnRendererArrayBuilder (new[] { columnDefinition }, _serviceLocator, _wcagHelperStub);
      builder.IsListReadOnly = true;

      _wcagHelperStub.Stub (stub => stub.IsWaiConformanceLevelARequired()).Return (false);

      var bocColumnRenderers = builder.CreateColumnRenderers ();
      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].IsVisibleColumn, Is.False);
      Assert.That (PrivateInvoke.GetNonPublicField (bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf (typeof (NullColumnRenderer)));
    }

    [Test]
    public void IsColumnVisible_BocRowEditModeColumnDefinition_IsListEditModeActiveIsFalse ()
    {
      var columnDefinition = new BocRowEditModeColumnDefinition();
      var builder = new BocColumnRendererArrayBuilder (new[] { columnDefinition }, _serviceLocator, _wcagHelperStub);
      builder.IsListEditModeActive = false;

      _wcagHelperStub.Stub (stub => stub.IsWaiConformanceLevelARequired()).Return (false);

      var bocColumnRenderers = builder.CreateColumnRenderers ();
      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].IsVisibleColumn, Is.True);
      Assert.That (PrivateInvoke.GetNonPublicField (bocColumnRenderers[0], "_columnRenderer"), Is.Not.TypeOf (typeof (NullColumnRenderer)));
    }

    [Test]
    public void IsColumnVisible_BocRowEditModeColumnDefinition_IsListEditModeActiveIsTrue ()
    {
      var columnDefinition = new BocRowEditModeColumnDefinition();
      var builder = new BocColumnRendererArrayBuilder (new[] { columnDefinition }, _serviceLocator, _wcagHelperStub);
      builder.IsListEditModeActive = true;

      _wcagHelperStub.Stub (stub => stub.IsWaiConformanceLevelARequired()).Return (false);

      var bocColumnRenderers = builder.CreateColumnRenderers ();
      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].IsVisibleColumn, Is.False);
      Assert.That (PrivateInvoke.GetNonPublicField (bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf (typeof (NullColumnRenderer)));
    }

    [Test]
    public void IsColumnVisible_BocDropDownMenuColumnDefinition_IsWaiConformanceLevelARequiredIsTrue ()
    {
      var columnDefinition = new BocDropDownMenuColumnDefinition();
      var builder = new BocColumnRendererArrayBuilder (new[] { columnDefinition }, _serviceLocator, _wcagHelperStub);

      _wcagHelperStub.Stub (stub => stub.IsWaiConformanceLevelARequired()).Return (true);

      var bocColumnRenderers = builder.CreateColumnRenderers ();
      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].IsVisibleColumn, Is.False);
      Assert.That (PrivateInvoke.GetNonPublicField (bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf (typeof (NullColumnRenderer)));
    }

    [Test]
    public void IsColumnVisible_BocDropDownMenuColumnDefinition_IsWaiConformanceLevelARequiredIsFalseAndIBrowserCapableOfScriptingIsFalse ()
    {
      var columnDefinition = new BocDropDownMenuColumnDefinition();
      var builder = new BocColumnRendererArrayBuilder (new[] { columnDefinition }, _serviceLocator, _wcagHelperStub);
      builder.IsBrowserCapableOfScripting = false;

      _wcagHelperStub.Stub (stub => stub.IsWaiConformanceLevelARequired()).Return (false);

      var bocColumnRenderers = builder.CreateColumnRenderers ();
      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].IsVisibleColumn, Is.False);
      Assert.That (PrivateInvoke.GetNonPublicField (bocColumnRenderers[0], "_columnRenderer"), Is.TypeOf (typeof (NullColumnRenderer)));
    }

    [Test]
    public void IsColumnVisible_BocDropDownMenuColumnDefinition_IsWaiConformanceLevelARequiredIsFalseAndIBrowserCapableOfScriptingIsTrue ()
    {
      var columnDefinition = new BocDropDownMenuColumnDefinition();
      var builder = new BocColumnRendererArrayBuilder (new[] { columnDefinition }, _serviceLocator, _wcagHelperStub);
      builder.IsBrowserCapableOfScripting = true;

      _wcagHelperStub.Stub (stub => stub.IsWaiConformanceLevelARequired()).Return (false);

      var bocColumnRenderers = builder.CreateColumnRenderers ();
      Assert.That (bocColumnRenderers.Length, Is.EqualTo (1));
      Assert.That (bocColumnRenderers[0].IsVisibleColumn, Is.True);
      Assert.That (PrivateInvoke.GetNonPublicField (bocColumnRenderers[0], "_columnRenderer"), Is.Not.TypeOf (typeof (NullColumnRenderer)));
    }
  }
}