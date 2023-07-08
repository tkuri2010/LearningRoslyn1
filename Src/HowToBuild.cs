using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using LearningRoslyn1.Misc;

namespace LearningRoslyn1;


/// <summary>
/// A directory, containing many .cs files.
/// </summary>
public class HowToBuild
{
	/// <summary>
	/// build a project directory.
	/// </summary>
	/// <param name="directory"></param>
	/// <param name="asmMetaRefPool"></param>
	/// <param name="ct"></param>
	/// <returns></returns>
	public static Assembly? Build(string directory, AsmMetadataReferencePoolD asmMetaRefPool, CancellationToken ct = default)
	{
		//
		//  parse all .cs files.
		//
		List<string> syntaxErrors = new();
		List<string> syntaxWarnings = new();
		var syntaxTreeList = Utils.EnumerateCsFiles(directory).Select(csFile =>
		{
			var text = File.ReadAllText(csFile);
			var syntaxTree = CSharpSyntaxTree.ParseText(text, null, path: csFile);

			_ProcessError(syntaxTree.GetDiagnostics(ct), out var errs, out var warns);
			syntaxErrors.AddRange(errs);
			syntaxWarnings.AddRange(warns);

			return syntaxTree;
		});

		if (syntaxErrors.Count >= 1)
		{
			syntaxErrors.ForEach(Console.WriteLine);
			return null;
		}

		syntaxWarnings.ForEach(Console.WriteLine);


		//
		// compile.
		//

		var asmName = Path.GetFileName(directory);

		CSharpCompilationOptions options = new(OutputKind.DynamicallyLinkedLibrary,
				nullableContextOptions: NullableContextOptions.Enable
				// ... and there are many many options!
				);

		CSharpCompilation comp = CSharpCompilation.Create(
				asmName,
				syntaxTreeList,
				asmMetaRefPool.GetMetadataReferences(),
				options
				);

		if (_ProcessError(comp.GetDiagnostics(ct), out var compilationErrs, out var compilationWarns))
		{
			compilationErrs.ForEach(Console.WriteLine);
			return null;
		}

		compilationWarns.ForEach(Console.WriteLine);


		//
		// emit (convert to Assembly)
		//

		using (var memoryStream = new MemoryStream())
		{
			EmitResult er = comp.Emit(memoryStream);

			if (_ProcessError(er.Diagnostics, out var emitErrs, out var emitWarns))
			{
				emitErrs.ForEach(Console.WriteLine);
				return null;
			}

			emitWarns.ForEach(Console.WriteLine);

			return Assembly.Load(memoryStream.GetBuffer());
		}
	}


	static bool _ProcessError(IEnumerable<Diagnostic> dxs, out List<string> outErrors, out List<string> outWarnings)
	{
		outErrors = new();
		outWarnings = new();

		foreach (var dx in dxs)
		{
			switch (dx.Severity)
			{
			case DiagnosticSeverity.Error:
				outErrors.Add(Utils.Stringfy(dx));
				break;

			case DiagnosticSeverity.Warning:
				outWarnings.Add(Utils.Stringfy(dx));
				break;

			default:
				break;
			}
		}

		return outErrors.Count >= 1;
	}
}
