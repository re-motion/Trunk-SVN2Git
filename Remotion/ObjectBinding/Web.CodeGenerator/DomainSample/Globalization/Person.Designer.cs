//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DomainSample.Globalization {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Person {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Person() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DomainSample.Globalization.Person", typeof(Person).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Person.
        /// </summary>
        internal static string property_DisplayName {
            get {
                return ResourceManager.GetString("property:DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to e-Mail.
        /// </summary>
        internal static string property_EMailAddress {
            get {
                return ResourceManager.GetString("property:EMailAddress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to First name.
        /// </summary>
        internal static string property_FirstName {
            get {
                return ResourceManager.GetString("property:FirstName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ID.
        /// </summary>
        internal static string property_ID {
            get {
                return ResourceManager.GetString("property:ID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Last name.
        /// </summary>
        internal static string property_LastName {
            get {
                return ResourceManager.GetString("property:LastName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Location.
        /// </summary>
        internal static string property_Location {
            get {
                return ResourceManager.GetString("property:Location", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Phone numbers.
        /// </summary>
        internal static string property_PhoneNumbers {
            get {
                return ResourceManager.GetString("property:PhoneNumbers", resourceCulture);
            }
        }
    }
}
