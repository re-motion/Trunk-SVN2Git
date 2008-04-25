using System;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

[TestFixture]
public class WxeParameterConverterTest
{
  private const string c_param = "param";

  private NameObjectCollection _callerParameters;

  private WxeParameterDeclaration _requiredObjectInParameter;
  private WxeParameterDeclaration _requiredStringInParameter;
  private WxeParameterDeclaration _requiredInt32InParameter;
  private WxeParameterDeclaration _requiredNaInt32InParameter;
  private WxeParameterDeclaration _requiredOutParameter;
  private WxeParameterDeclaration _stringInParameter;
  private WxeParameterDeclaration _int32InParameter;

  [SetUp]
  public virtual void SetUp()
  {
    _requiredObjectInParameter = new WxeParameterDeclaration (c_param, true, WxeParameterDirection.In, typeof (object));
    _requiredStringInParameter = new WxeParameterDeclaration (c_param, true, WxeParameterDirection.In, typeof (string));
    _requiredInt32InParameter = new WxeParameterDeclaration (c_param, true, WxeParameterDirection.In, typeof (Int32));
    _requiredNaInt32InParameter = new WxeParameterDeclaration (c_param, true, WxeParameterDirection.In, typeof (Int32?));
    _requiredOutParameter = new WxeParameterDeclaration (c_param, true, WxeParameterDirection.Out, typeof (string));

    _stringInParameter = new WxeParameterDeclaration (c_param, false, WxeParameterDirection.In, typeof (string));
    _int32InParameter = new WxeParameterDeclaration (c_param, false, WxeParameterDirection.In, typeof (Int32));

    _callerParameters = new NameObjectCollection();
  }

  [Test]
  public void ConvertObjectToStringRequiredStringInParameter()
  {
    string value = "Hello World!";
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredStringInParameter);
    Assert.AreEqual (value, converter.ConvertObjectToString (value));
  }

  [Test]
  public void ConvertObjectToStringRequiredStringInParameterWithNull()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredStringInParameter);
    Assert.AreEqual (string.Empty, converter.ConvertObjectToString (null));
  }

  [Test]
  public void ConvertObjectToStringStringInParameter()
  {
    string value = "Hello World!";
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_stringInParameter);
    Assert.AreEqual (value, converter.ConvertObjectToString (value));
  }

  [Test]
  public void ConvertObjectToStringStringInParameterWithNullValue()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_stringInParameter);
    Assert.AreEqual (null, converter.ConvertObjectToString (null));
  }

  [Test]
  public void ConvertObjectToStringRequiredInt32InParameter()
  {
    int value = 1;
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredInt32InParameter);
    Assert.AreEqual (value.ToString(), converter.ConvertObjectToString (value));
  }

  [Test]
  public void ConvertObjectToStringRequiredInt32InParameterWithNull()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredInt32InParameter);
    Assert.AreEqual (string.Empty, converter.ConvertObjectToString (null));
  }

  [Test]
  public void ConvertObjectToStringRequiredNaInt32InParameter()
  {
    int? value = 1;
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredNaInt32InParameter);
    Assert.AreEqual (value.ToString(), converter.ConvertObjectToString (value));
  }

  [Test]
  public void ConvertObjectToStringRequiredNaInt32InParameterWithNaInt32Null()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredNaInt32InParameter);
    Assert.AreEqual (string.Empty, converter.ConvertObjectToString (null));
  }

  [Test]
  public void ConvertObjectToStringRequiredNaInt32InParameterWithNull()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredNaInt32InParameter);
    Assert.AreEqual (string.Empty, converter.ConvertObjectToString (null));
  }

  [Test]
  public void ConvertObjectToStringInt32InParameter()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_int32InParameter);
    Assert.AreEqual (1.ToString(), converter.ConvertObjectToString (1));
  }

  [Test]
  public void ConvertObjectToStringInt32InParameterWithNullValue()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_int32InParameter);
    Assert.AreEqual (null, converter.ConvertObjectToString (null));
  }

  [Test]
  public void ConvertObjectToStringRequiredVarRefInParameter()
  {
    int value = 1;
    WxeVariableReference varRef = new WxeVariableReference (c_param);
    _callerParameters.Add (c_param, value);
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredInt32InParameter);
    Assert.AreEqual (value.ToString(), converter.ConvertVarRefToString (varRef, _callerParameters));
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void ConvertVarRefToStringRequiredVarRefInParameterWithNoCallerParameters()
  {
    WxeVariableReference varRef = new WxeVariableReference (c_param);
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredInt32InParameter);
    converter.ConvertVarRefToString (varRef, null);
  }

  [Test]
  [ExpectedException (typeof (WxeException))]
  public void ConvertVarRefToStringRequiredVarRefInParameterWithVarRef()
  {
    WxeVariableReference value = new WxeVariableReference (c_param);
    WxeVariableReference varRef = new WxeVariableReference (c_param);
    _callerParameters.Add (c_param, value);
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredInt32InParameter);
    converter.ConvertVarRefToString (varRef, _callerParameters);
  }

  [Test]
  public void ConvertObjectToStringVarRefInt32InParameter()
  {
    int value = 1;
    WxeVariableReference varRef = new WxeVariableReference (c_param);
    _callerParameters.Add (c_param, value);
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_int32InParameter);
    Assert.AreEqual (value.ToString(), converter.ConvertVarRefToString (varRef, _callerParameters));
  }

  [Test]
  public void ConvertVarRefToStringVarRefInt32InParameterWithNoCallerParameters()
  {
    WxeVariableReference varRef = new WxeVariableReference (c_param);
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_int32InParameter);
    Assert.IsNull (converter.ConvertVarRefToString (varRef, null));
  }

  [Test]
  public void ConvertVarRefToStringVarRefInParameterWithVarRef()
  {
    WxeVariableReference value = new WxeVariableReference (c_param);
    WxeVariableReference varRef = new WxeVariableReference (c_param);
    _callerParameters.Add (c_param, value);
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_int32InParameter);
    Assert.IsNull (converter.ConvertVarRefToString (varRef, _callerParameters));    
  }

  
  [Test]
  [ExpectedException (typeof (WxeException))]
  public void CheckForRequiredOutParameter()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredOutParameter);
    converter.CheckForRequiredOutParameter();
  }


  [Test]
  public void TryConvertObjectToStringRequiredStringInParameter()
  {
    string value = "Hello World!";
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredStringInParameter);
    Assert.AreEqual (value, converter.TryConvertObjectToString (value));
  }

  [Test]
  public void TryConvertObjectToStringRequiredStringInParameterWithEmptyString()
  {
    string value = string.Empty;
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredStringInParameter);
    converter.TryConvertObjectToString (value);
    Assert.AreEqual (value, converter.TryConvertObjectToString (value));
  }

  [Test]
  public void TryConvertObjectToStringStringInParameterWithEmptyString()
  {
    string value = string.Empty;
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_stringInParameter);
    Assert.AreEqual (value, converter.TryConvertObjectToString (value));
  }

  [Test]
  public void TryConvertObjectToStringRequiredInt32InParameter()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredInt32InParameter);
    Assert.AreEqual (1.ToString(), converter.TryConvertObjectToString (1));
  }

  [Test]
  public void TryConvertObjectToStringRequiredInt32InParameterWithNull()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredInt32InParameter);
    Assert.AreEqual (string.Empty, converter.ConvertObjectToString (null));
  }

  [Test]
  public void TryConvertObjectToStringInt32InParameter()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredInt32InParameter);
    Assert.AreEqual (1.ToString(), converter.TryConvertObjectToString (1));
  }

  [Test]
  public void TryConvertObjectToStringInt32InParameterWithNull()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_int32InParameter);
    Assert.AreEqual (string.Empty, converter.TryConvertObjectToString (null));
  }
  [Test]
  public void TryConvertObjectToStringRequiredNaInt32InParameter()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredNaInt32InParameter);
    Assert.AreEqual (1.ToString(), converter.TryConvertObjectToString (1));
  }

  [Test]
  public void TryConvertObjectToStringRequiredNaInt32InParameterWithNaInt32Null()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredNaInt32InParameter);
    Assert.AreEqual (string.Empty, converter.TryConvertObjectToString (null));
  }

  [Test]
  public void TryConvertObjectToStringRequiredNaInt32InParameterWithNull()
  {
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredNaInt32InParameter);
    Assert.AreEqual (string.Empty, converter.TryConvertObjectToString (null));
  }

  [Test]
  public void TryConvertObjectToStringRequiredObjectInParameter()
  {
    object value = new object();
    WxeParameterConverterMock converter = new WxeParameterConverterMock (_requiredObjectInParameter);
    Assert.AreEqual (value, converter.TryConvertObjectToString (value));
  }

}

}
