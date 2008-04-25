//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [DebuggerNonUserCode()]
    [CompilerGenerated()]
    internal class AccessControlResources {
        
        private static ResourceManager resourceMan;
        
        private static CultureInfo resourceCulture;
        
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AccessControlResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager {
            get {
                if (ReferenceEquals(resourceMan, null)) {
                    ResourceManager temp = new ResourceManager("Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl.AccessControlR" +
                            "esources", typeof(AccessControlResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Actual Priority: {0}.
        /// </summary>
        internal static string ActualPriorityLabelText {
            get {
                return ResourceManager.GetString("ActualPriorityLabelText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ACE (Access Control Entry).
        /// </summary>
        internal static string auto_AccessControlEntryTitle_InnerText {
            get {
                return ResourceManager.GetString("auto:AccessControlEntryTitle:InnerText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ACL (Access Control List).
        /// </summary>
        internal static string auto_AccessControlListTitle_InnerText {
            get {
                return ResourceManager.GetString("auto:AccessControlListTitle:InnerText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please assign at least one state..
        /// </summary>
        internal static string auto_MissingStateCombinationsValidator_ErrorMessage {
            get {
                return ResourceManager.GetString("auto:MissingStateCombinationsValidator:ErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Permissions.
        /// </summary>
        internal static string auto_PermissionsLabel_Text {
            get {
                return ResourceManager.GetString("auto:PermissionsLabel:Text", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please select a tenant..
        /// </summary>
        internal static string auto_SpecificTenantField_ErrorMessage {
            get {
                return ResourceManager.GetString("auto:SpecificTenantField:ErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Delete ACE.
        /// </summary>
        internal static string DeleteAccessControlEntryButton {
            get {
                return ResourceManager.GetString("DeleteAccessControlEntryButton", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Delete ACL.
        /// </summary>
        internal static string DeleteAccessControlListButton {
            get {
                return ResourceManager.GetString("DeleteAccessControlListButton", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Remove State.
        /// </summary>
        internal static string DeleteStateCombinationCommand {
            get {
                return ResourceManager.GetString("DeleteStateCombinationCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to States must be unqiue within the ACL..
        /// </summary>
        internal static string DuplicateStateCombinationsValidatorErrorMessage {
            get {
                return ResourceManager.GetString("DuplicateStateCombinationsValidatorErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New ACE.
        /// </summary>
        internal static string NewAccessControlEntryButton {
            get {
                return ResourceManager.GetString("NewAccessControlEntryButton", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New ACL.
        /// </summary>
        internal static string NewAccessControlListButton {
            get {
                return ResourceManager.GetString("NewAccessControlListButton", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Assign State.
        /// </summary>
        internal static string NewStateCombinationButton {
            get {
                return ResourceManager.GetString("NewStateCombinationButton", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to for.
        /// </summary>
        internal static string SpecificPositionAndGroupLinkingLabelText {
            get {
                return ResourceManager.GetString("SpecificPositionAndGroupLinkingLabelText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} - Edit Permissions.
        /// </summary>
        internal static string Title {
            get {
                return ResourceManager.GetString("Title", resourceCulture);
            }
        }
    }
}
