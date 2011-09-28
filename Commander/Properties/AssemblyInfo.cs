using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if WINDOWS && TRIAL
[assembly: AssemblyTitle( "Commander" )]
[assembly: AssemblyProduct( "Commander" )]
[assembly: AssemblyDescription("An epic and lovely tower defense game.")]
[assembly: AssemblyCompany("Ephemere Games")]

[assembly: AssemblyCopyright("Copyright © Jodi Giordano")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
#else
[assembly: AssemblyTitle("Commander")]
[assembly: AssemblyProduct("Commander")]
[assembly: AssemblyDescription("An epic and lovely tower defense game.")]
[assembly: AssemblyCompany("Ephemere Games")]

[assembly: AssemblyCopyright("Copyright © Jodi Giordano")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
#endif
// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
#if WINDOWS && TRIAL
[assembly: Guid("2dfedd60-73b0-434f-92e2-2801be716f68")]
#else
[assembly: Guid("09d33e20-6640-43fd-977d-48702051f49a")]
#endif

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("0.1.0.0")]
