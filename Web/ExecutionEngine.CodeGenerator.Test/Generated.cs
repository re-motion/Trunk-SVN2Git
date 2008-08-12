//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1434
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Test {
    using System;
    using Remotion.Web.ExecutionEngine;
    
    
    public partial class AutoPage {
        
        protected new AutoPageFunction CurrentFunction {
            get {
                return ((AutoPageFunction)(base.CurrentFunction));
            }
        }
        
        public String InArg {
            get {
                return ((String)(this.Variables["InArg"]));
            }
            set {
                this.Variables["InArg"] = value;
            }
        }
        
        public String InOutArg {
            get {
                return ((String)(this.Variables["InOutArg"]));
            }
            set {
                this.Variables["InOutArg"] = value;
            }
        }
        
        public String OutArg {
            set {
                this.Variables["OutArg"] = value;
            }
        }
        
        public String Suffix {
            get {
                return ((String)(this.Variables["Suffix"]));
            }
            set {
                this.Variables["Suffix"] = value;
            }
        }
        
        protected void Return() {
            this.ExecuteNextStep();
        }
        
        public static String Call(Remotion.Web.ExecutionEngine.IWxePage currentPage, Remotion.Web.ExecutionEngine.IWxeCallArguments arguments, String InArg, ref String InOutArg) {
            AutoPageFunction function;
            if ((currentPage.IsReturningPostBack == false)) {
                function = new AutoPageFunction();
                function.InArg = InArg;
                function.InOutArg = InOutArg;
                function.SetCatchExceptionTypes(typeof(System.Exception));
                arguments.Dispatch(currentPage, function);
                throw new System.Exception("(Unreachable code)");
            }
            else {
                function = ((AutoPageFunction)(currentPage.ReturningFunction));
                if ((function.Exception != null)) {
                    throw function.Exception;
                }
                InOutArg = function.InOutArg;
                return function.OutArg;
            }
        }
        
        public static String Call(Remotion.Web.ExecutionEngine.IWxePage currentPage, String InArg, ref String InOutArg) {
            return AutoPage.Call(currentPage, Remotion.Web.ExecutionEngine.WxeCallArguments.Default, InArg, ref InOutArg);
        }
    }
    
    [System.SerializableAttribute()]
    public class AutoPageFunction : WxeFunction {
        
        private Remotion.Web.ExecutionEngine.WxePageStep Step1 = new Remotion.Web.ExecutionEngine.WxePageStep("AutoPage.aspx");
        
        public AutoPageFunction() {
        }
        
        public AutoPageFunction(String InArg, String InOutArg) : 
                base(InArg, InOutArg) {
        }
        
        [Remotion.Web.ExecutionEngine.WxeParameterAttribute(0, true, Remotion.Web.ExecutionEngine.WxeParameterDirection.In)]
        public String InArg {
            set {
                this.Variables["InArg"] = value;
            }
        }
        
        [Remotion.Web.ExecutionEngine.WxeParameterAttribute(1, true, Remotion.Web.ExecutionEngine.WxeParameterDirection.InOut)]
        public String InOutArg {
            get {
                return ((String)(this.Variables["InOutArg"]));
            }
            set {
                this.Variables["InOutArg"] = value;
            }
        }
        
        [Remotion.Web.ExecutionEngine.WxeParameterAttribute(2, Remotion.Web.ExecutionEngine.WxeParameterDirection.Out)]
        public String OutArg {
            get {
                return ((String)(this.Variables["OutArg"]));
            }
        }
    }
}
namespace Test {
    using System;
    using Remotion.Web.ExecutionEngine;
    using System.Collections.Generic;
    
    
    public partial class CalledPage {
        
        protected new CalledPageFunction CurrentFunction {
            get {
                return ((CalledPageFunction)(base.CurrentFunction));
            }
        }
        
        public string input {
            get {
                return ((string)(this.Variables["input"]));
            }
            set {
                this.Variables["input"] = value;
            }
        }
        
        public List<System.Int32[,][]> other {
            get {
                return ((List<System.Int32[,][]>)(this.Variables["other"]));
            }
            set {
                this.Variables["other"] = value;
            }
        }
        
        public string output {
            set {
                this.Variables["output"] = value;
            }
        }
        
        public string bothways {
            get {
                return ((string)(this.Variables["bothways"]));
            }
            set {
                this.Variables["bothways"] = value;
            }
        }
        
        public string ReturnValue {
            set {
                this.Variables["ReturnValue"] = value;
            }
        }
        
        protected void Return() {
            this.ExecuteNextStep();
        }
        
        public static string Call(Remotion.Web.ExecutionEngine.IWxePage currentPage, Remotion.Web.ExecutionEngine.IWxeCallArguments arguments, string input, List<System.Int32[,][]> other, out string output, ref string bothways) {
            CalledPageFunction function;
            if ((currentPage.IsReturningPostBack == false)) {
                function = new CalledPageFunction();
                function.input = input;
                function.other = other;
                function.bothways = bothways;
                function.SetCatchExceptionTypes(typeof(System.Exception));
                arguments.Dispatch(currentPage, function);
                throw new System.Exception("(Unreachable code)");
            }
            else {
                function = ((CalledPageFunction)(currentPage.ReturningFunction));
                if ((function.Exception != null)) {
                    throw function.Exception;
                }
                output = function.output;
                bothways = function.bothways;
                return function.ReturnValue;
            }
        }
        
        public static string Call(Remotion.Web.ExecutionEngine.IWxePage currentPage, string input, List<System.Int32[,][]> other, out string output, ref string bothways) {
            return CalledPage.Call(currentPage, Remotion.Web.ExecutionEngine.WxeCallArguments.Default, input, other, out output, ref bothways);
        }
    }
    
    [System.SerializableAttribute()]
    public class CalledPageFunction : Test.BaseFunction {
        
        private Remotion.Web.ExecutionEngine.WxePageStep Step1 = new Remotion.Web.ExecutionEngine.WxePageStep("CalledPage.aspx");
        
        public CalledPageFunction() {
        }
        
        public CalledPageFunction(string input, List<System.Int32[,][]> other) : 
                base(input, other) {
        }
        
        [Remotion.Web.ExecutionEngine.WxeParameterAttribute(0, Remotion.Web.ExecutionEngine.WxeParameterDirection.In)]
        public string input {
            set {
                this.Variables["input"] = value;
            }
        }
        
        [Remotion.Web.ExecutionEngine.WxeParameterAttribute(1, Remotion.Web.ExecutionEngine.WxeParameterDirection.In)]
        public List<System.Int32[,][]> other {
            set {
                this.Variables["other"] = value;
            }
        }
        
        [Remotion.Web.ExecutionEngine.WxeParameterAttribute(2, Remotion.Web.ExecutionEngine.WxeParameterDirection.Out)]
        public string output {
            get {
                return ((string)(this.Variables["output"]));
            }
        }
        
        [Remotion.Web.ExecutionEngine.WxeParameterAttribute(3, Remotion.Web.ExecutionEngine.WxeParameterDirection.InOut)]
        public string bothways {
            get {
                return ((string)(this.Variables["bothways"]));
            }
            set {
                this.Variables["bothways"] = value;
            }
        }
        
        [Remotion.Web.ExecutionEngine.WxeParameterAttribute(4, Remotion.Web.ExecutionEngine.WxeParameterDirection.Out)]
        public string ReturnValue {
            get {
                return ((string)(this.Variables["ReturnValue"]));
            }
        }
    }
}
