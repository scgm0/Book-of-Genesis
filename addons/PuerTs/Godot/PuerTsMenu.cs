#if TOOLS

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Godot;
using Puerts.Editor.Generator;
// ReSharper disable CheckNamespace
#pragma warning disable CA1050

[Tool]
public partial class PuerTsMenu : PopupMenu {
	private string _saveTo = ProjectSettings.GlobalizePath("res://addons/PuerTs/Gen/");

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

	private async void GenV1() {
		await Task.Run(GenerateCode);
		await Task.Run(GenerateDts);
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