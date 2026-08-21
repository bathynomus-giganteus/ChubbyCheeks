using System.Text.Json;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;

namespace CultLeaderMod.CultLeaderModCode.Patches;

[HarmonyPatch]
public static class LocInjectPatch
{
	private const string LocalizationRoot = "res://CultLeaderMod/localization";

	private static readonly string[] TableNames =
	[
		"characters",
		"cards",
		"card_keywords",
		"powers",
		"relics",
		"events",
		"ancients",
		"gameplay_ui",
	];

	[HarmonyPatch(typeof(LocManager), "SetLanguageInternal")]
	[HarmonyPostfix]
	private static void Postfix(string language, Dictionary<string, LocTable> tables)
	{
		if (tables == null)
			return;

		var normalizedLanguage = NormalizeLanguage(language);
		Log.Info($"[CultLeaderMod] Loading localization for language={language}, normalized={normalizedLanguage}");

		var loadedTables = 0;
		foreach (var tableName in TableNames)
		{
			if (!tables.TryGetValue(tableName, out var locTable))
				continue;

			var localizedEntries = LoadLocalizationTable(normalizedLanguage, tableName)
				?? LoadLocalizationTable("eng", tableName)
				?? LoadLocalizationTable("zhs", tableName);

			if (localizedEntries == null || localizedEntries.Count == 0)
			{
				Log.Warn($"[CultLeaderMod] Missing localization table: language={normalizedLanguage}, table={tableName}");
				continue;
			}

			locTable.MergeWith(localizedEntries);
			loadedTables++;
			Log.Info($"[CultLeaderMod] Localization injected: language={normalizedLanguage}, table={tableName}, entries={localizedEntries.Count}");
		}

		Log.Info($"[CultLeaderMod] Localization injection complete: language={normalizedLanguage}, tables={loadedTables}");
	}

	private static string NormalizeLanguage(string? language)
	{
		if (string.IsNullOrWhiteSpace(language))
			return "eng";

		var normalized = language.Trim().Replace('-', '_').ToLowerInvariant();
		return normalized switch
		{
			"zhs" or "zh" or "zh_cn" or "zh_hans" or "chs" or "chinese" or "simplified_chinese" => "zhs",
			"eng" or "en" or "en_us" or "en_gb" or "english" => "eng",
			"jpn" or "ja" or "jp" or "ja_jp" or "japanese" => "jpn",
			"kor" or "ko" or "kr" or "ko_kr" or "korean" => "kor",
			_ => normalized,
		};
	}

	private static Dictionary<string, string>? LoadLocalizationTable(string language, string tableName)
	{
		var resourcePath = $"{LocalizationRoot}/{language}/{tableName}.json";
		var json = ReadText(resourcePath);
		if (string.IsNullOrWhiteSpace(json))
			return null;

		try
		{
			return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
		}
		catch (Exception ex)
		{
			Log.Warn($"[CultLeaderMod] Failed to parse localization table {resourcePath}: {ex.Message}");
			return null;
		}
	}

	private static string? ReadText(string resourcePath)
	{
		try
		{
			if (Godot.FileAccess.FileExists(resourcePath))
			{
				using var file = Godot.FileAccess.Open(resourcePath, Godot.FileAccess.ModeFlags.Read);
				return file?.GetAsText();
			}
		}
		catch (Exception ex)
		{
			Log.Warn($"[CultLeaderMod] Failed to read localization resource {resourcePath}: {ex.Message}");
		}

		var relativePath = resourcePath.Replace("res://", string.Empty, StringComparison.Ordinal);
		var fileSystemPath = Path.Combine(AppContext.BaseDirectory, relativePath.Replace('/', Path.DirectorySeparatorChar));
		try
		{
			return File.Exists(fileSystemPath)
				? File.ReadAllText(fileSystemPath)
				: null;
		}
		catch (Exception ex)
		{
			Log.Warn($"[CultLeaderMod] Failed to read localization file {fileSystemPath}: {ex.Message}");
			return null;
		}
	}
}
