using System.Reflection;
using LearningRoslyn1.Misc;
using Microsoft.CodeAnalysis;

namespace LearningRoslyn1;


/// <summary>
/// example2
/// </summary>
public static class Example2
{
	const string Directory = "../Example2";

	static readonly string DirDll = Path.Combine(Directory, "Dll");

	static readonly string DirClassLib = Path.Combine(Directory, "ClassLib");

	static readonly string DirMain = Path.Combine(Directory, "Main");

	public static async Task RunAsync()
	{
		using AsmMetadataReferencePoolD asmMetaRefPoolD = new();

		//
		// prepare
		//
		foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
		{
			asmMetaRefPoolD.AdddMetadataOf(asm);
		}


		//
		// load my DLL file.
		//
		var dllFile = Path.Combine(DirDll, "MyDll.dll");
		Assembly dllAsm = Assembly.LoadFrom(dllFile);

		// So that the Assembly can be referenced at compile time.
		// (Either of the following methods may work)
		#if true
			asmMetaRefPoolD.AddMetadataReference(MetadataReference.CreateFromFile(dllFile));
		#else
			asmMetaRefPoolD.AdddMetadataOf(dllAsm);
		#endif


		//
		// build the ClassLib
		//
		Assembly? classLibAsm = HowToBuild.Build(DirClassLib, asmMetaRefPoolD);
		if (classLibAsm is null)
		{
			return;
		}

		asmMetaRefPoolD.AdddMetadataOf(classLibAsm);


		//
		// Dynamically created assemblies should be handled in the event handler as follows.
		//
		AppDomain.CurrentDomain.AssemblyResolve += (_, eventArgs) =>
		{
			Console.WriteLine($"# Someone needs an assembly [{eventArgs.Name}]");

			if (eventArgs.Name == classLibAsm.FullName)
			{
				Console.WriteLine($"# OK, I know the assembly. Returning this...");
				return classLibAsm;
			}

			return null;
		};


		//
		// build the Main project
		//
		Assembly? mainAsm = HowToBuild.Build(DirMain, asmMetaRefPoolD);
		if (mainAsm is null)
		{
			return;
		}


		//
		//  run
		//
		await Utils.InvokeAsync(mainAsm, "Example2.Main", "Run");
	}
}
