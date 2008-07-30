using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Resources;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("AssetPackager.Net")]
[assembly: AssemblyDescription("Moves all scripts to the bottom of the page, combines multiple external javascripts into a single one.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Pikaba, Inc.")]
[assembly: AssemblyProduct("AssetPackager.Net")]
[assembly: AssemblyCopyright("Copyright © Dmytro Shteflyuk 2008")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("en-US")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("7c4a6674-4d03-44ff-90d5-0f9a847b3a59")]

// Version information for an assembly
[assembly: AssemblyVersion("0.1.1.*")]
[assembly: AssemblyFileVersion("0.1.0.0")]

// Informs the ResourceManager of the neutral culture of an assembly
[assembly: NeutralResourcesLanguage("en-US")]

// Assembly is CLS compliant
[assembly: CLSCompliant(true)]

// Suppress some code analysis warnings
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "AssetPackager")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "AssetPackager.WebControls")]
