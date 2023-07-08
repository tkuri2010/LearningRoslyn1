namespace Lib;

public class MyLibClass
{
	public MyLibClass()
	{
	}


	public void SayHello()
	{
		var dllClass = new Dll.MyDllClass();
		dllClass.Say("Hello many classes!");
	}
}
