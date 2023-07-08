namespace LearningRoslyn1.Misc;

public class EnumFileDto
{
	public static EnumFileDto File(string filePath)
	{
		return new()
		{
			Event = EnumFileEvent.File,
			Path = filePath,
		};
	}


	public static EnumFileDto EnterDir(string dirPath)
	{
		return new()
		{
			Event = EnumFileEvent.EnterDir,
			Path = dirPath,
		};
	}


	public EnumFileEvent Event = EnumFileEvent._None;


	public string Path = string.Empty;


	public EnumFileReaction Reaction = EnumFileReaction._None;


	public EnumFileDto LeaveDir()
	{
		return new()
		{
			Event = EnumFileEvent.LeaveDir,
			Path = this.Path,
		};
	}
}


public enum EnumFileEvent
{
	_None = 0,
	File = 1,
	EnterDir = 2,
	LeaveDir = 3,
}


public enum EnumFileReaction
{
	_None = 0,

	SkipDir = 1,
}

