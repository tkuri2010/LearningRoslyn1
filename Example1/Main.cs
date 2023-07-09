using System;
using System.Threading.Tasks;

namespace Example1;

public class Main
{
	public Task RunAsync()
	{
		Console.WriteLine("Hello Roslyn.");
		return Task.CompletedTask;
	}
}
