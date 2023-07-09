using System.Reflection;
using Microsoft.CodeAnalysis;

namespace LearningRoslyn1.Misc;

public static class Utils
{
	/// <summary>
	/// build the string from a Diagnositc object.
	/// </summary>
	/// <param name="dx"></param>
	/// <param name="projectFilePath"></param>
	/// <returns></returns>
	public static string Stringfy(Diagnostic dx, string? projectFilePath = null)
	{
		var file = dx.Location.SourceTree?.FilePath ?? "(unknown file location)";
		var line = dx.Location.SourceSpan.Start + 1;
		var ch = dx.Location.SourceSpan.Length + 1;

		var proj = string.IsNullOrEmpty(projectFilePath) ? "" : $" [{projectFilePath}]";

		return $"{file}({line},{ch}): {dx.Severity} {dx.Id}: {dx.GetMessage()}" + proj;
	}


	/// <summary>
	/// invoke specified method.
	/// </summary>
	/// <param name="asm">assembly</param>
	/// <param name="fullName">namespace and class name</param>
	/// <param name="methodName"></param>
	/// <param name="parameters">(if any)</param>
	/// <returns></returns>
	public static Task InvokeAsync(Assembly asm, string fullName, string methodName, object?[]? parameters = null)
	{
		Type? mainType = asm.GetType(fullName);
		if (mainType is null)
		{
			Console.Error.WriteLine($"There is not a class \"{fullName}\"");
			return Task.CompletedTask;
		}

		MethodInfo? method = mainType.GetMethod(methodName);
		if (method is null)
		{
			Console.Error.WriteLine($"There is not a method \"{methodName}()\".");
			return Task.CompletedTask;
		}

		object? instance = Activator.CreateInstance(mainType);
		if (instance is null)
		{
			Console.Error.WriteLine($"{mainType.FullName} - Instantiation failed.");
			return Task.CompletedTask;
		}

		object? result = method.Invoke(instance, parameters);

		return (result is Task task) ? task : Task.CompletedTask;
	}


	/// <summary>
	///  numerates *.cs files
	/// </summary>
	/// <param name="directory"></param>
	/// <returns></returns>
	public static IEnumerable<string> EnumerateCsFiles(string directory)
	{
		foreach (var info in EnumerateFiles(directory))
		{
			var name = Path.GetFileName(info.Path);
			var ext = Path.GetExtension(name);

			if (info.Event == EnumFileEvent.EnterDir)
			{
				if (name is "bin" or "obj")
				{
					info.Reaction = EnumFileReaction.SkipDir;
				}
			}
			else if (info.Event == EnumFileEvent.File && ext == ".cs")
			{
				yield return info.Path;
			}
		}
	}


	/// <summary>
	/// enumerates files and directories
	/// </summary>
	/// <param name="directory"></param>
	/// <returns></returns>
	public static IEnumerable<EnumFileDto> EnumerateFiles(string directory)
	{
		var stack = new Stack<EnumFileDto>();
		_EnumFilesInto(directory, stack);

		while (stack.TryPop(out var info))
		{
			yield return info;

			if (info.Event == EnumFileEvent.EnterDir
					&& info.Reaction != EnumFileReaction.SkipDir)
			{
				stack.Push(info.LeaveDir());
				_EnumFilesInto(info.Path, stack);
			}
		}
	}


	static void _EnumFilesInto(string directory, Stack<EnumFileDto> outStack)
	{
		var files = Directory.EnumerateFiles(directory)
				.Reverse()
				.Select(path => EnumFileDto.File(path));
		foreach (var file in files)
		{
			outStack.Push(file);
		}

		var dirs = Directory.EnumerateDirectories(directory)
				.Reverse()
				.Select(path => EnumFileDto.EnterDir(path));
		foreach (var dir in dirs)
		{
			outStack.Push(dir);
		}
	}
}
