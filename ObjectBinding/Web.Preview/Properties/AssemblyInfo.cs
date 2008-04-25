using System;
using System.Reflection;
using System.Web.UI;

[assembly: TagPrefix ("Remotion.ObjectBinding.Web.UI.Controls", "remotion")]

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("Remotion BusinessObjectControls (Preview) Library")]
[assembly: AssemblyDescription("Web controls for 2-way databinding with BusinessObjects.")]
[assembly: AssemblyConfiguration(".NET Framework: net-2.0, build type: debug")]
[assembly: AssemblyCompany("rubicon informationstechnologie gmbh")]
[assembly: AssemblyProduct("Remotion")]
[assembly: AssemblyCopyright("(c) 2003 - 2007 rubicon informationstechnologie gmbh, www.rubicon-it.com")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		
[assembly: CLSCompliant(true)]
//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion("1.7.65.202")]

//
// In order to sign your assembly you must specify a key to use. Refer to the 
// Microsoft .NET Framework documentation for more information on assembly signing.
//
// Use the attributes below to control which key is used for signing. 
//
// Notes: 
//   (*) If no key is specified, the assembly is not signed.
//   (*) KeyName refers to a key that has been installed in the Crypto Service
//       Provider (CSP) on your machine. KeyFile refers to a file which contains
//       a key.
//   (*) If the KeyFile and the KeyName values are both specified, the 
//       following processing occurs:
//       (1) If the KeyName can be found in the CSP, that key is used.
//       (2) If the KeyName does not exist and the KeyFile does exist, the key 
//           in the KeyFile is installed into the CSP and used.
//   (*) In order to create a KeyFile, you can use the sn.exe (Strong Name) utility.
//       When specifying the KeyFile, the location of the KeyFile should be
//       relative to the project output directory which is
//       %Project Directory%\obj\<configuration>. For example, if your KeyFile is
//       located in the project directory, you would specify the AssemblyKeyFile 
//       attribute as [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//   (*) Delay Signing is an advanced option - see the Microsoft .NET Framework
//       documentation for more information on this.
//
#pragma warning disable 1699
[assembly: AssemblyDelaySign (false)]
[assembly: AssemblyKeyFile(@"\Development\global/remotion.snk")]
[assembly: AssemblyKeyName("")]
#pragma warning restore 1699
