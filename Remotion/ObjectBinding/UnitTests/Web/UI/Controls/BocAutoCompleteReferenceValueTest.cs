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
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.Configuration;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{
  [TestFixture]
  public class BocAutoCompleteReferenceValueTest : BocTest
  {
    private class GetObjectService : IGetObjectService
    {
      private readonly IBusinessObjectWithIdentity _objectToReturn;

      public GetObjectService (IBusinessObjectWithIdentity objectToReturn)
      {
        _objectToReturn = objectToReturn;
      }

      public IBusinessObjectWithIdentity GetObject (BindableObjectClassWithIdentity classWithIdentity, string uniqueIdentifier)
      {
        return _objectToReturn;
      }
    }

    private Page _page;
    private BocAutoCompleteReferenceValueMock _control;
    private TypeWithReference _businessObject;
    private IBusinessObjectDataSource _dataSource;
    private IBusinessObjectReferenceProperty _propertyReferenceValue;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _control = new BocAutoCompleteReferenceValueMock();
      _control.ID = "BocAutoCompleteReferenceValue";
      _control.Value = (IBusinessObjectWithIdentity) _businessObject;

      MockRepository mockRepository = new MockRepository ();
      _page = mockRepository.PartialMultiMock<Page> (typeof (ISmartPage));
      ((ISmartPage) _page).Stub (stub => stub.Context).Return (new HttpContextWrapper (HttpContext.Current));
      _page.Replay ();
      _page.Controls.Add (_control);

      _businessObject = TypeWithReference.Create();

      _propertyReferenceValue =
          (IBusinessObjectReferenceProperty) ((IBusinessObject) _businessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue");

      _dataSource = new BusinessObjectReferenceDataSource();
      _dataSource.BusinessObject = (IBusinessObject) _businessObject;

      ((IBusinessObject) _businessObject).BusinessObjectClass.BusinessObjectProvider.AddService<IGetObjectService>
          (new GetObjectService ((IBusinessObjectWithIdentity) TypeWithReference.Create()));
      ((IBusinessObject) _businessObject).BusinessObjectClass.BusinessObjectProvider.AddService<IBusinessObjectWebUIService>
          (new ReflectionBusinessObjectWebUIService());
    }

    [Test]
    public void EvaluateWaiConformityDebugLevelUndefined ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
      _control.EvaluateWaiConformity();

      Assert.IsFalse (WcagHelperMock.HasWarning);
      Assert.IsFalse (WcagHelperMock.HasError);
    }

    [Test]
    public void EvaluateWaiConformityDebugLevelA ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA ();
      _control.EvaluateWaiConformity ();

      Assert.IsFalse (WcagHelperMock.HasWarning);
      Assert.IsTrue (WcagHelperMock.HasError);
      Assert.That (WcagHelperMock.Control, Is.EqualTo (_control));
    }

    [Test]
    public void IsEventCommandDisabledWithWcagOverride ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
      _control.Command.Type = CommandType.Event;
      Assert.IsFalse (_control.IsCommandEnabled (false));
    }

    [Test]
    public void IsEventCommandEnabledWithoutWcagOverride ()
    {
      _businessObject.ReferenceValue = TypeWithReference.Create();
      _control.DataSource = _dataSource;
      _control.Property = _propertyReferenceValue;
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
      _control.Command.Type = CommandType.Event;
      Assert.IsTrue (_control.IsCommandEnabled (false));
    }

    [Test]
    public void IsWxeFunctionCommandDisabledWithWcagOverride ()
    {
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
      _control.Command.Type = CommandType.WxeFunction;
      Assert.IsFalse (_control.IsCommandEnabled (false));
    }

    [Test]
    public void IsWxeFunctionCommandEnabledWithoutWcagOverride ()
    {
      _businessObject.ReferenceValue = TypeWithReference.Create();
      _control.DataSource = _dataSource;
      _control.Property = _propertyReferenceValue;
      WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
      _control.Command.Type = CommandType.WxeFunction;
      Assert.IsTrue (_control.IsCommandEnabled (false));
    }

    [Test]
    public void GetTrackedClientIDsInReadOnlyMode ()
    {
      _control.ReadOnly = true;
      string[] actual = _control.GetTrackedClientIDs();
      Assert.IsNotNull (actual);
      Assert.AreEqual (0, actual.Length);
    }

    [Test]
    public void GetTrackedClientIDsInEditMode ()
    {
      _control.ReadOnly = false;
      string[] actual = _control.GetTrackedClientIDs();
      Assert.IsNotNull (actual);
      Assert.AreEqual (1, actual.Length);
      Assert.AreEqual (_control.TextBoxClientID, actual[0]);
    }


    [Test]
    public void SetValueToObject ()
    {
      IBusinessObjectWithIdentity referencedObject = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _control.IsDirty = false;
      _control.Value = referencedObject;
      Assert.AreEqual (referencedObject, _control.Value);
      Assert.IsTrue (_control.IsDirty);
    }

    [Test]
    public void SetValueToNull ()
    {
      _control.IsDirty = false;
      _control.Value = null;
      Assert.AreEqual (null, _control.Value);
      Assert.IsTrue (_control.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimTrue ()
    {
      _businessObject.ReferenceValue = TypeWithReference.Create();
      _control.DataSource = _dataSource;
      _control.Property = _propertyReferenceValue;
      _control.Value = null;
      _control.IsDirty = true;

      _control.LoadValue (true);
      Assert.AreEqual (null, _control.Value);
      Assert.IsTrue (_control.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithObject ()
    {
      _businessObject.ReferenceValue = TypeWithReference.Create();
      _control.DataSource = _dataSource;
      _control.Property = _propertyReferenceValue;
      _control.Value = null;
      _control.IsDirty = true;

      _control.LoadValue (false);
      Assert.AreEqual (_businessObject.ReferenceValue, _control.Value);
      Assert.IsFalse (_control.IsDirty);
    }

    [Test]
    public void LoadValueAndInterimFalseWithNull ()
    {
      _businessObject.ReferenceValue = null;
      _control.DataSource = _dataSource;
      _control.Property = _propertyReferenceValue;
      _control.Value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _control.IsDirty = true;

      _control.LoadValue (false);
      Assert.AreEqual (_businessObject.ReferenceValue, _control.Value);
      Assert.IsFalse (_control.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimTrue ()
    {
      IBusinessObjectWithIdentity value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _control.Property = _propertyReferenceValue;
      _control.Value = null;
      _control.IsDirty = true;

      _control.LoadUnboundValue (value, true);
      Assert.AreEqual (null, _control.Value);
      Assert.IsTrue (_control.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithObject ()
    {
      IBusinessObjectWithIdentity value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _control.Property = _propertyReferenceValue;
      _control.Value = null;
      _control.IsDirty = true;

      _control.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _control.Value);
      Assert.IsFalse (_control.IsDirty);
    }

    [Test]
    public void LoadUnboundValueAndInterimFalseWithNull ()
    {
      const IBusinessObjectWithIdentity value = null;
      _control.Property = _propertyReferenceValue;
      _control.Value = (IBusinessObjectWithIdentity) TypeWithReference.Create();
      _control.IsDirty = true;

      _control.LoadUnboundValue (value, false);
      Assert.AreEqual (value, _control.Value);
      Assert.IsFalse (_control.IsDirty);
    }

    [Test]
    public void SaveControlState ()
    {
      _control.Value = (IBusinessObjectWithIdentity) _businessObject;

      object state = _control.SaveControlState();
      Assert.That (state is object[]);

      object[] stateArray = (object[]) state;
      Assert.That (stateArray.Length, Is.EqualTo (3));

      Assert.That (stateArray[1], Is.EqualTo (_control.Value.UniqueIdentifier));
      Assert.That (stateArray[2], Is.EqualTo (_control.Value.DisplayName));
    }

    [Test]
    public void LoadControlState ()
    {
      object parentState = ((object[]) _control.SaveControlState())[0];
      object[] state = new object[3];

      Guid uniqueIdentifier = Guid.NewGuid();
      state[0] = parentState;
      state[1] = uniqueIdentifier.ToString();
      state[2] = "DisplayName";

      _control.LoadControlState (state);
      Assert.That (_control.BusinessObjectDisplayName, Is.EqualTo ("DisplayName"));
      Assert.That (_control.BusinessObjectUniqueIdentifier, Is.EqualTo (uniqueIdentifier.ToString()));
    }

    [Test]
    public void LoadPostDataNotRequired ()
    {
      PrivateInvoke.SetNonPublicField (_control, "_hasBeenRenderedInPreviousLifecycle", false);

      bool result = ((IPostBackDataHandler) _control).LoadPostData (null, null);
      Assert.IsFalse (result);
    }

    [Test]
    public void LoadPostDataNullValue ()
    {
      PrivateInvoke.InvokeNonPublicMethod (_control, "CreateChildControls");

      var key = _control.HiddenFieldClientID;
      var postbackCollection = new NameValueCollection();

      postbackCollection.Add (key, null);

      _control.IsDirty = false;
      PrivateInvoke.SetNonPublicField (_control, "_hasBeenRenderedInPreviousLifecycle", true);
      ((ISmartPage) _control.Page).Stub (stub => stub.GetPostBackCollection ()).Return (postbackCollection);

      bool result = ((IPostBackDataHandler) _control).LoadPostData (key, postbackCollection);
      Assert.IsFalse (_control.IsDirty);
      Assert.IsFalse (result);
    }

    [Test]
    public void LoadPostDataEmptyValue ()
    {
      PrivateInvoke.InvokeNonPublicMethod (_control, "CreateChildControls");

      var key = _control.HiddenFieldUniqueID;
      var postbackCollection = new NameValueCollection ();

      postbackCollection.Add (key, string.Empty);

      _control.IsDirty = false;
      PrivateInvoke.SetNonPublicField (_control, "_hasBeenRenderedInPreviousLifecycle", true);
      ((ISmartPage) _control.Page).Stub (stub => stub.GetPostBackCollection ()).Return (postbackCollection);

      bool result = ((IPostBackDataHandler) _control).LoadPostData (key, postbackCollection);
      Assert.That (_control.IsDirty);
      Assert.IsTrue (result);
      Assert.That (_control.Value, Is.Null);
    }

    [Test]
    public void LoadPostDataReferenceValue ()
    {
      PrivateInvoke.InvokeNonPublicMethod (_control, "CreateChildControls");

      var postbackCollection = new NameValueCollection();

      Guid value = Guid.NewGuid();
      postbackCollection.Add (_control.HiddenFieldUniqueID, value.ToString ());
      postbackCollection.Add (_control.TextBoxClientID, "NewValue");

      _control.IsDirty = false;
      
      PrivateInvoke.SetNonPublicField (_control, "_hasBeenRenderedInPreviousLifecycle", true);
      ((ISmartPage) _control.Page).Stub (stub => stub.GetPostBackCollection ()).Return (postbackCollection);

      bool result = ((IPostBackDataHandler) _control).LoadPostData (_control.HiddenFieldClientID, postbackCollection);
      Assert.That (_control.IsDirty);
      Assert.IsTrue (result);
      Assert.That (_control.BusinessObjectDisplayName, Is.EqualTo ("NewValue"));
      Assert.That (_control.BusinessObjectUniqueIdentifier, Is.EqualTo (value.ToString()));
    }

    [Test]
    public void LoadPostDataSameReferenceValue ()
    {
      PrivateInvoke.InvokeNonPublicMethod (_control, "CreateChildControls");

      var postbackCollection = new NameValueCollection ();

      string value = _control.Value.UniqueIdentifier;
      postbackCollection.Add (_control.HiddenFieldClientID, value);

      _control.IsDirty = false;

      PrivateInvoke.SetNonPublicField (_control, "_hasBeenRenderedInPreviousLifecycle", true);
      ((ISmartPage) _control.Page).Stub (stub => stub.GetPostBackCollection ()).Return (postbackCollection);

      bool result = ((IPostBackDataHandler) _control).LoadPostData (_control.HiddenFieldClientID, postbackCollection);
      Assert.IsFalse (_control.IsDirty);
      Assert.IsFalse (result);
    }

    [Test]
    public void LoadValueInterim ()
    {
      _control.IsDirty = true;

      _control.Property = _propertyReferenceValue;
      _control.DataSource = _dataSource;

      var newValue = (IBusinessObjectWithIdentity) TypeWithReference.Create ();
      _control.Value = newValue;
      Assert.That (_control.Value, Is.EqualTo (newValue));
      
      _control.LoadValue (true);

      Assert.That (_control.Value, Is.EqualTo (newValue));
      Assert.That (_control.IsDirty);
    }

    [Test]
    public void LoadValueInitial ()
    {
      _control.IsDirty = true;

      _control.Property = _propertyReferenceValue;
      _control.DataSource = _dataSource;

      var propertyValue = _dataSource.BusinessObject.GetProperty(_propertyReferenceValue);
      var newValue = (IBusinessObjectWithIdentity) TypeWithReference.Create ();
      _control.Value = newValue;
      Assert.That (_control.Value, Is.EqualTo (newValue));

      _control.LoadValue (false);

      Assert.That (_control.Value, Is.EqualTo (propertyValue));
      Assert.That (!_control.IsDirty);
    }

    [Test]
    public void LoadUnboundValueInterim ()
    {
      var oldValue = _control.Value;
      var newValue = (IBusinessObjectWithIdentity) TypeWithReference.Create ();

      _control.LoadUnboundValue (newValue, true);

      Assert.That (_control.Value, Is.EqualTo (oldValue));
      Assert.That (_control.IsDirty);
    }

    [Test]
    public void LoadUnboundValueInitial ()
    {
      var newValue = (IBusinessObjectWithIdentity) TypeWithReference.Create ();

      _control.LoadUnboundValue (newValue, false);

      Assert.That (_control.Value, Is.EqualTo (newValue));
      Assert.That (!_control.IsDirty);
    }

    [Test]
    public void SaveValueInterim ()
    {
      _control.IsDirty = true;

      _control.Property = _propertyReferenceValue;
      _control.DataSource = _dataSource;

      var propertyValue = _dataSource.BusinessObject.GetProperty (_propertyReferenceValue);
      var newValue = (IBusinessObjectWithIdentity) TypeWithReference.Create ();
      _control.Value = newValue;

      _control.SaveValue (true);

      Assert.That (_dataSource.BusinessObject.GetProperty (_propertyReferenceValue), Is.EqualTo (propertyValue));
      Assert.That (_control.IsDirty);
    }

    [Test]
    public void SaveValueCommit ()
    {
      _control.IsDirty = true;

      _control.Property = _propertyReferenceValue;
      _control.DataSource = _dataSource;

      var newValue = (IBusinessObjectWithIdentity) TypeWithReference.Create ();
      _control.Value = newValue;

      _control.SaveValue (false);

      Assert.That (_dataSource.BusinessObject.GetProperty (_propertyReferenceValue), Is.EqualTo (newValue));
      Assert.That (!_control.IsDirty);
    }

    [Test]
    public void CreateValidatorsReadOnly ()
    {
      _control.ErrorMessage = "ErrorMessage";
      _control.ReadOnly = true;
      _control.Required = true;
      BaseValidator[] validators = _control.CreateValidators();
      Assert.That (validators.Length, Is.EqualTo (0));
    }

    [Test]
    public void CreateValidatorsNotRequired ()
    {
      _control.ErrorMessage = "ErrorMessage";
      _control.ReadOnly = false;
      _control.Required = false;
      BaseValidator[] validators = _control.CreateValidators ();
      Assert.That (validators.Length, Is.EqualTo (0));
    }

    [Test]
    public void CreateValidatorsEditableRequired ()
    {
      _control.ErrorMessage = "ErrorMessage";
      _control.ReadOnly = false;
      _control.Required = true;
      BaseValidator[] validators = _control.CreateValidators ();
      Assert.That (validators.Length, Is.EqualTo (1));
      Assert.That (validators[0] is RequiredFieldValidator);
    }

    [Test]
    public void RaisePostDataChangedEvent ()
    {
      bool eventHandlerCalled = false;
      _control.SelectionChanged += (sender, e) => { eventHandlerCalled = true; };
      ((IPostBackDataHandler) _control).RaisePostDataChangedEvent();

      Assert.That (eventHandlerCalled);
    }

    [Test]
    public void RaisePostDataChangedEventReadOnly ()
    {
      _control.ReadOnly = true;
      bool eventHandlerCalled = false;
      _control.SelectionChanged += (sender, e) => { eventHandlerCalled = true; };
      ((IPostBackDataHandler) _control).RaisePostDataChangedEvent ();

      Assert.That (!eventHandlerCalled);
    }

    [Test]
    public void RaisePostDataChangedEventDisabled ()
    {
      _control.Enabled = false;
      bool eventHandlerCalled = false;
      _control.SelectionChanged += (sender, e) => { eventHandlerCalled = true; };
      ((IPostBackDataHandler) _control).RaisePostDataChangedEvent ();

      Assert.That (!eventHandlerCalled);
    }
  }
}
