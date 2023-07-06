﻿namespace _2D_RPG_Negiramen.Models.FileEntries
{
    using Tomlyn;
    using Tomlyn.Model;

    /// <summary>
    ///     😁 構成
    /// </summary>
    class UserConfiguration
    {
        // - 静的プロパティー

        internal static string AutoGenerated { get; } = "Auto Generated";

        // - 静的メソッド

        /// <summary>
        ///     <pre>
        ///         TOML形式ファイルの読取
        ///     
        ///         📖　[Tomlyn　＞　Documentation](https://github.com/xoofx/Tomlyn/blob/main/doc/readme.md)
        ///     </pre>
        /// </summary>
        /// <param name="configuration">構成</param>
        /// <returns>TOMLテーブルまたはヌル</returns>
        internal static bool LoadTOML(out UserConfiguration configuration)
        {
            try
            {
                // フォルダー名は自動的に与えられているので、これを使う
                string appDataDirAsStr = FileSystem.Current.AppDataDirectory;
                // Example: `C:/Users/むずでょ/AppData/Local/Packages/1802ca7b-559d-489e-8a13-f02ac4d27fcc_9zz4h110yvjzm/LocalState`

                // 読取たいファイルへのパス
                var configurationFilePath = System.IO.Path.Combine(appDataDirAsStr, "configuration.toml");

                // 設定ファイルの読取
                var configurationText = System.IO.File.ReadAllText(configurationFilePath);


                Locations.Negiramen.WorkingTileSetCanvasImageFile workingTileSetCanvasImageFile = new Models.FileEntries.Locations.Negiramen.WorkingTileSetCanvasImageFile();

                Locations.UnityAssetsFolder unityAssetsFolder = new Models.FileEntries.Locations.UnityAssetsFolder();

                Locations.Negiramen.UserConfigurationFile userConfiguration = new Models.FileEntries.Locations.Negiramen.UserConfigurationFile();
                YourCircleName yourCircleName = new YourCircleName();
                YourWorkName yourWorkName = new YourWorkName();

                // TOML
                TomlTable document = Toml.ToModel(configurationText);

                if (document != null)
                {
                    //
                    // [paths]
                    // =======
                    //
                    if (document.TryGetValue("paths", out object pathsObj))
                    {
                        if (pathsObj != null && pathsObj is TomlTable paths)
                        {
                            // ネギラーメン・ワークスペースの作業中のタイル・セット・キャンバスPNG画像ファイルへのパス
                            if (paths.TryGetValue("working_tile_set_canvas", out object workingTileSetCanvasFilePathObj))
                            {
                                if (workingTileSetCanvasFilePathObj is string workingTileSetCanvasFilePathAsStr)
                                {
                                    workingTileSetCanvasImageFile = new Locations.Negiramen.WorkingTileSetCanvasImageFile(
                                        pathSource: FileEntryPathSource.FromString(workingTileSetCanvasFilePathAsStr),
                                        convert: (pathSource) => FileEntryPath.From(pathSource,
                                                                                    replaceSeparators: true));
                                }
                            }
                        }
                    }
                }

                configuration = new UserConfiguration(
                    workingTileSetCanvasImageFile);
                return true;
            }
            catch (Exception ex)
            {
                // TODO 例外対応、何したらいい（＾～＾）？
                configuration = null;
                return false;
            }
        }

        /// <summary>
        ///     保存
        /// </summary>
        /// <param name="current">現在の構成</param>
        /// <param name="difference">現在の構成から更新した差分</param>
        /// <param name="newConfiguration">差分を反映した構成</param>
        /// <returns>完了した</returns>
        internal static bool SaveTOML(UserConfiguration current, UserConfigurationBuffer difference, out UserConfiguration newConfiguration)
        {
            var configurationBuffer = new UserConfigurationBuffer();

            // 差分適用
            configurationBuffer.WorkingTileSetCanvasImageFile = difference.WorkingTileSetCanvasImageFile == null ? current.WorkingTileSetCanvasImageFile : difference.WorkingTileSetCanvasImageFile;

            // TODO ★ 変数展開後のパスではなく、変数展開前のパス文字列を保存したい
            var text = $@"[paths]

# ネギラーメン・ワークスペースの作業中のタイル・セット・キャンバスPNG画像ファイルへのパス
working_tile_set_canvas = ""{configurationBuffer.WorkingTileSetCanvasImageFile.Path.AsStr}""
";

            // 上書き
            System.IO.File.WriteAllText(
                // 保存したいファイルへのパス
                path: App.GetOrLoadConfiguration().UserConfigurationFile.Path.AsStr,
                contents: text);

            // イミュータブル・オブジェクトを生成
            newConfiguration = new UserConfiguration(
                configurationBuffer.WorkingTileSetCanvasImageFile);
            return true;
        }

        // - インターナル・プロパティー

        /// <summary>
        ///     ネギラーメン・ワークスペースの作業中のタイル・セット・キャンバスPNG画像ファイルへのパス
        /// </summary>
        /// <example>"C:/Users/むずでょ/Documents/GitHub/2D-RPG-Negiramen/Workspace"</example>
        internal Locations.Negiramen.WorkingTileSetCanvasImageFile WorkingTileSetCanvasImageFile { get; }

        // - その他

        /// <summary>
        ///     生成
        /// </summary>
        internal UserConfiguration() : this(
            Locations.Negiramen.WorkingTileSetCanvasImageFile.Empty)
        {
        }

        /// <summary>
        ///     生成
        /// </summary>
        /// <param name="negiramenWorkspaceFolderPath">ネギラーメン・ワークスペースの作業中のタイル・セット・キャンバスPNG画像ファイルへのパス</param>
        internal UserConfiguration(
            Locations.Negiramen.WorkingTileSetCanvasImageFile workingTileSetCanvasImageFile)
        {
            this.WorkingTileSetCanvasImageFile = workingTileSetCanvasImageFile;
        }
    }
}
