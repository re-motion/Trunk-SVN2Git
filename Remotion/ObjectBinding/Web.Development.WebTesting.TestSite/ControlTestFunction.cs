using System;
using Remotion.ObjectBinding.Sample;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite
{
  public class ControlTestFunction : WxeFunction
  {
    private Person _person;

    public ControlTestFunction ()
        : base (new NoneTransactionMode())
    {
    }

    [WxeParameter (1, false, WxeParameterDirection.In)]
    public string UserControl
    {
      get { return (string) Variables["UserControl"]; }
      set
      {
        ArgumentUtility.CheckNotNullOrEmpty ("UserControl", value);
        Variables["UserControl"] = value;
      }
    }

    public Person Person
    {
      get { return _person; }
    }

    // Steps
    private void Step1 ()
    {
      if (string.IsNullOrEmpty (UserControl))
        UserControl = "Controls/BocAutoCompleteReferenceValueUserControl.ascx";

      ExceptionHandler.AppendCatchExceptionTypes (typeof (WxeUserCancelException));
    }

    private void Step2 ()
    {
      XmlReflectionBusinessObjectStorageProvider.Current.Reset();

      var personID = new Guid (0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);
      _person = Person.GetObject (personID);
    }

    private WxeStep Step3 = new WxePageStep ("ControlTestForm.aspx");

    private void Step4 ()
    {
      XmlReflectionBusinessObjectStorageProvider.Current.Reset();
    }
  }
}