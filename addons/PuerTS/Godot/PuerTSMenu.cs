#if TOOLS

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;
using Puerts.Editor.Generator;
using Environment = System.Environment;

[Tool]
// ReSharper disable once CheckNamespace
// ReSharper disable once InconsistentNaming
// ReSharper disable once IdentifierTypo
public partial class PuerTSMenu : PopupMenu {
	static PuerTSMenu() {
		NativeLibrary.Load(GodotUtils.GetLibraryPath(GodotUtils.PlatformFeatureMap[OS.GetName()],
			Engine.GetArchitectureName()));
		// OS.SetEnvironment("LD_LIBRARY_PATH", AppDomain.CurrentDomain.BaseDirectory);
		// GD.Print(Environment.GetEnvironmentVariable("LD_LIBRARY_PATH"));
		// Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", AppDomain.CurrentDomain.BaseDirectory);
		// GD.PrintS(Environment.CurrentDirectory, AppDomain.CurrentDomain.BaseDirectory);
	}

	private string _saveTo = ProjectSettings.GlobalizePath("res://addons/PuerTS/Gen/");

	public override void _Ready() {
		if (!Engine.IsEditorHint()) {
			QueueFree();
		}
		IdPressed += id => {
			switch (id) {
				case 0:
					GenV1();
					break;
				case 1:
					Task.Run(GenerateDts);
					break;
				case 2:
					ClearAll();
					break;
			}
		};
	}

	private void GenV1() {
		Task.Run(GenerateCode)
			.ContinueWith( _=> GenerateDts());
	}

	private void GenerateCode() {
		var start = DateTime.Now;
		Directory.CreateDirectory(_saveTo);
		FileExporter.ExportWrapper(_saveTo, new GodotDefaultLoader());
		GenRegisterInfo();
		Debug.WriteLine("finished! use " + (DateTime.Now - start).TotalMilliseconds + " ms");
		Utils.SetFilters(null);
	}

	private void GenerateDts() {
		var start = DateTime.Now;
		Directory.CreateDirectory(_saveTo);
		Directory.CreateDirectory(Path.Combine(_saveTo, "Typing/csharp"));
		FileExporter.ExportDTS(_saveTo, new GodotDefaultLoader());
		Debug.WriteLine("finished! use " + (DateTime.Now - start).TotalMilliseconds + " ms");
		Utils.SetFilters(null);
	}

	private void ClearAll() {
		if (Directory.Exists(_saveTo)) {
			Directory.Delete(_saveTo, true);
		}
	}

	private void GenRegisterInfo() {
		var start = DateTime.Now;
		Directory.CreateDirectory(_saveTo);
		FileExporter.GenRegisterInfo(_saveTo, new GodotDefaultLoader());
		Debug.WriteLine("finished! use " + (DateTime.Now - start).TotalMilliseconds + " ms Outputed to " + _saveTo);
	}
}
#endif