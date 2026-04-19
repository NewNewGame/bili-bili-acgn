@tool
extends EditorExportPlugin

## Output folder of the last export (directory containing the .exe).
var _export_dir: String = ""

func _get_name() -> String:
	return "SpineDllCleanupExportPlugin"


func _supports_platform(platform: EditorExportPlatform) -> bool:
	return platform is EditorExportPlatformWindows


func _export_begin(features: PackedStringArray, is_debug: bool, path: String, flags: int) -> void:
	_export_dir = ""
	if not features.has("windows"):
		return
	if path.is_empty():
		return
	_export_dir = path.get_base_dir()


func _export_end() -> void:
	if _export_dir.is_empty():
		return
	_erase_spine_artifacts_in_dir(_export_dir.path_join("windows"))
	_erase_spine_artifacts_in_dir(_export_dir)
	_export_dir = ""


func _erase_spine_artifacts_in_dir(dir_path: String) -> void:
	var da := DirAccess.open(dir_path)
	if da == null:
		return
	da.list_dir_begin()
	var entry: String = da.get_next()
	while entry != "":
		if not da.current_is_dir() and _is_spine_windows_artifact(entry):
			var full: String = dir_path.path_join(entry)
			var err: Error = DirAccess.remove_absolute(full)
			if err != OK and FileAccess.file_exists(full):
				push_warning("Post export cleanup: could not remove %s (error %s)" % [full, err])
		entry = da.get_next()
	da.list_dir_end()


func _is_spine_windows_artifact(file_name: String) -> bool:
	if not (file_name.begins_with("libspine_godot") or file_name.begins_with("~libspine_godot")):
		return false
	return (
		file_name.ends_with(".dll")
		or file_name.ends_with(".lib")
		or file_name.ends_with(".exp")
	)
