using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;

namespace LearningRoslyn1;


/// <summary>
/// Holding Assemblies' MetadataReferences
/// </summary>
public class AsmMetadataReferencePoolD : IDisposable
{
	List<AsmAndMetadataD> mLoadedsD = new();


	List<MetadataReference> mMetaRefList = new();


	/// <summary>
	/// We need an Assembly's MetadataReference at compile time.
	/// https://github.com/dotnet/runtime/issues/36590#issuecomment-689883856
	/// </summary>
	/// <param name="asm"></param>
	unsafe public void AdddMetadataOf(Assembly asm)
	{
		if (! asm.TryGetRawMetadata(out var blob, out var len))
		{
			// TODO: report an error.
			return;
		}

		var ok = false;
		var dto = new AsmAndMetadataD();
		try
		{
			// I think these objects may need to be Dispose() later.
			dto.ModuleMetadata = ModuleMetadata.CreateFromMetadata((IntPtr)blob, len);
			dto.AssemblyMetadata = AssemblyMetadata.Create(dto.ModuleMetadata);
			dto.MetadataReference = dto.AssemblyMetadata.GetReference();
			mLoadedsD.Add(dto);
			ok = true;
		}
		finally
		{
			if (!ok) dto.Dispose();
		}
	}


	public void AddMetadataReference(MetadataReference mr)
	{
		mMetaRefList.Add(mr);
	}


	/// <summary>
	/// Use at compoile time.
	/// </summary>
	/// <returns></returns>
	public IEnumerable<MetadataReference> GetMetadataReferences()
	{
		return mLoadedsD.Select(it => it.MetadataReference!)
				.Concat(mMetaRefList);
	}


	#region implements IDisposable

	private bool mDisposed = false;


	public void Dispose()
	{
		Dispose(true);
	}


	~AsmMetadataReferencePoolD()
	{
		Dispose(false);
	}


	protected virtual void Dispose(bool disposing)
	{
		if (mDisposed) return;
		mDisposed = true;

		foreach (var obj in mLoadedsD)
		{
			obj.Dispose();
		}

		if (disposing)
		{
			GC.SuppressFinalize(this);
		}
	}

	#endregion
}


internal class AsmAndMetadataD : IDisposable
{
	internal ModuleMetadata? ModuleMetadata { get; set; } = null;


	internal AssemblyMetadata? AssemblyMetadata { get; set; } = null;


	internal MetadataReference? MetadataReference { get; set; } = null;


	#region implements IDisposable

	private bool mDisposed = false;


	public void Dispose()
	{
		Dispose(true);
	}


	~AsmAndMetadataD()
	{
		Dispose(false);
	}


	protected virtual void Dispose(bool disposing)
	{
		if (mDisposed) return;
		mDisposed = true;

		ModuleMetadata?.Dispose();
		AssemblyMetadata?.Dispose();

		if (disposing)
		{
			GC.SuppressFinalize(this);
		}
	}

	#endregion
}

