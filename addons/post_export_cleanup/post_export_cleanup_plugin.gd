@tool
extends EditorPlugin

var _cleanup_export_plugin: EditorExportPlugin

func _enter_tree() -> void:
	_cleanup_export_plugin = preload("res://addons/post_export_cleanup/spine_dll_cleanup_export_plugin.gd").new()
	add_export_plugin(_cleanup_export_plugin)


func _exit_tree() -> void:
	if _cleanup_export_plugin != null:
		remove_export_plugin(_cleanup_export_plugin)
		_cleanup_export_plugin = null
