# LearningRoslyn1
Learning how to compile C# code using Roslyn.

## What is this?
This dotnet project shows you haw to compile and run C# source codes with Roslyn.

Requires dotnet 6.0 (or later (?)).

We have 2 examples on this project.

- directory `Example1`
	- Very simple example.
- directory `Example2`
	- .dll file, class library, and main project.

To try `Example1`, go to `Src` directory, and run it.
```powershell
cd Src
dotnet run
```


To try `Example2`, modify `Src/Program.cs`.
```cs
// simple example
// await LearningRoslyn1.Example1.RunAsync();

// many classes
await LearningRoslyn1.Example2.RunAsync();
```

and run it.
```powershell
cd Src
dotnet run
```
The `Main.cs` in the `Example2/Main` referes the `ClassLib/MySomeClass.cs`, and the `MySomeClass.cs` referes the `Dll/MyDll.dll`.


### memo
https://stackoverflow.com/questions/32769630/how-to-compile-a-c-sharp-file-with-roslyn-programmatically
