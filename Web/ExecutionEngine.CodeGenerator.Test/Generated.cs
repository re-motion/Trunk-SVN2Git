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
namespace Test {
    using System;
    using System.Web.UI;
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
                function = new AutoPageFunction(InArg, InOutArg);
                function.ExceptionHandler.SetCatchExceptionTypes(typeof(System.Exception));
                currentPage.ExecuteFunction(function, arguments);
                throw new System.Exception("(Unreachable code)");
            }
            else {
                function = ((AutoPageFunction)(currentPage.ReturningFunction));
                if ((function.ExceptionHandler.Exception != null)) {
                    throw function.ExceptionHandler.Exception;
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
        
        public AutoPageFunction() : 
                base(new Remotion.Web.ExecutionEngine.Infrastructure.NoneTransactionMode(), new object[0]) {
        }
        
        public AutoPageFunction(String InArg, String InOutArg) : 
                base(new Remotion.Web.ExecutionEngine.Infrastructure.NoneTransactionMode(), new object[] {
                            InArg,
                            InOutArg}) {
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
    using System.Collections.Generic;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using Remotion.Web.ExecutionEngine;
    
    
    public partial class AutoUserControl {
        
        protected new AutoUserControlFunction CurrentFunction {
            get {
                return ((AutoUserControlFunction)(base.CurrentFunction));
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
        
        public String ReturnValue {
            set {
                this.Variables["ReturnValue"] = value;
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
        
        public static String Call(Remotion.Web.ExecutionEngine.IWxePage currentPage, Remotion.Web.ExecutionEngine.WxeUserControl currentUserControl, System.Web.UI.Control sender, String InArg, ref String InOutArg) {
            AutoUserControlFunction function;
            if ((currentPage.IsReturningPostBack == false)) {
                function = new AutoUserControlFunction(InArg, InOutArg);
                function.ExceptionHandler.SetCatchExceptionTypes(typeof(System.Exception));
                Remotion.Web.ExecutionEngine.WxeUserControl actualUserControl;
                actualUserControl = ((Remotion.Web.ExecutionEngine.WxeUserControl)(currentPage.FindControl(currentUserControl.PermanentUniqueID)));
                actualUserControl.ExecuteFunction(function, sender, null);
                throw new System.Exception("(Unreachable code)");
            }
            else {
                function = ((AutoUserControlFunction)(currentPage.ReturningFunction));
                if ((function.ExceptionHandler.Exception != null)) {
                    throw function.ExceptionHandler.Exception;
                }
                InOutArg = function.InOutArg;
                return function.ReturnValue;
            }
        }
    }
    
    [System.SerializableAttribute()]
    public class AutoUserControlFunction : WxeFunction {
        
        private Remotion.Web.ExecutionEngine.WxeUserControlStep Step1 = new Remotion.Web.ExecutionEngine.WxeUserControlStep("AutoUserControl.ascx");
        
        public AutoUserControlFunction() : 
                base(new Remotion.Web.ExecutionEngine.Infrastructure.NoneTransactionMode(), new object[0]) {
        }
        
        public AutoUserControlFunction(String InArg, String InOutArg) : 
                base(new Remotion.Web.ExecutionEngine.Infrastructure.NoneTransactionMode(), new object[] {
                            InArg,
                            InOutArg}) {
        }
        
        [Remotion.Web.ExecutionEngine.WxeParameterAttribute(0, true, Remotion.Web.ExecutionEngine.WxeParameterDirection.In)]
        public String InArg {
            set {
                this.Variables["InArg"] = value;
            }
        }
        
        [Remotion.Web.ExecutionEngine.WxeParameterAttribute(1, Remotion.Web.ExecutionEngine.WxeParameterDirection.InOut)]
        public String InOutArg {
            get {
                return ((String)(this.Variables["InOutArg"]));
            }
            set {
                this.Variables["InOutArg"] = value;
            }
        }
        
        [Remotion.Web.ExecutionEngine.WxeParameterAttribute(2, Remotion.Web.ExecutionEngine.WxeParameterDirection.Out)]
        public String ReturnValue {
            get {
                return ((String)(this.Variables["ReturnValue"]));
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
                function = new CalledPageFunction(input, other, bothways);
                function.ExceptionHandler.SetCatchExceptionTypes(typeof(System.Exception));
                currentPage.ExecuteFunction(function, arguments);
                throw new System.Exception("(Unreachable code)");
            }
            else {
                function = ((CalledPageFunction)(currentPage.ReturningFunction));
                if ((function.ExceptionHandler.Exception != null)) {
                    throw function.ExceptionHandler.Exception;
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
        
        public CalledPageFunction() : 
                base(new object[0]) {
        }
        
        public CalledPageFunction(string input, List<System.Int32[,][]> other, string bothways) : 
                base(new object[] {
                            input,
                            other,
                            bothways}) {
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
