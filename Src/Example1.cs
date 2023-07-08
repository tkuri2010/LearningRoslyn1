using System.Reflection;
using LearningRoslyn1.Misc;

namespace LearningRoslyn1;


/// <summary>
/// example1
/// </summary>
public static class Example1
{
	const string Directory = "../Example1";


	/// <summary>
	/// run
	/// </summary>
	/// <returns></returns>
	public static async Task RunAsync()
	{
		using AsmMetadataReferencePoolD asmMetaRefPoolD = new();


		//
		//  prepare
		//
		foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
		{
			asmMetaRefPoolD.AdddMetadataOf(asm);
		}


		//
		//  build
		//
		Assembly? theAssembly = HowToBuild.Build(Directory, asmMetaRefPoolD);
		if (theAssembly is null)
		{
			Console.Error.WriteLine("There are some compilation error.");
			return;
		}


		//
		//  run
		//
		await Utils.InvokeAsync(theAssembly, "Example1.Main", "RunAsync");
	}
}
