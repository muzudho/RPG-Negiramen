﻿namespace _2D_RPG_Negiramen.Models.FileEntries
{
    using Tomlyn;
    using Tomlyn.Model;

    /// <summary>
    ///     😁 構成
    /// </summary>
    class Configuration
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
        internal static bool LoadTOML(out Configuration configuration)
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


                Locations.Negiramen.WorkspaceFolder negiramenWorkspaceFolderPath = new Models.FileEntries.Locations.Negiramen.WorkspaceFolder();

                Locations.UnityAssetsFolder unityAssetsFolderPath = new Models.FileEntries.Locations.UnityAssetsFolder();

                Locations.Negiramen.UserConfiguration userConfigurationFilePath = new Models.FileEntries.Locations.Negiramen.UserConfiguration();
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
                            // ネギラーメンの 📂 `Workspace` フォルダ―へのパス
                            if (paths.TryGetValue("negiramen_workspace_folder", out object negiramenWorkspaceFolderPathObj))
                            {
                                if (negiramenWorkspaceFolderPathObj is string negiramenWorkspaceFolderPathAsStr)
                                {
                                    negiramenWorkspaceFolderPath = Locations.Negiramen.WorkspaceFolder.FromString(
                                        negiramenWorkspaceFolderPathAsStr,
                                        replaceSeparators: true);
                                }
                            }

                            // Unity の Assets フォルダ―へのパス
                            if (paths.TryGetValue("unity_assets_folder", out object unityAssetsFolderPathObj))
                            {
                                if (unityAssetsFolderPathObj is string unityAssetsFolderPathAsStr)
                                {

/* プロジェクト '2D RPG Negiramen (net7.0-android)' からのマージされていない変更
前:
                                    unityAssetsFolderPath = Models.FileEntriesLocations.UnityAssetsFolderPath.FromString(
後:
                                    unityAssetsFolderPath = UnityAssetsFolderPath.FromString(
*/
                                    unityAssetsFolderPath = Locations.UnityAssetsFolder.FromString(
                                        unityAssetsFolderPathAsStr,
                                        replaceSeparators: true);
                                }
                            }
                        }
                    }

                    //
                    // [paths_2nd]
                    // ===========
                    //
                    if (document.TryGetValue("paths_2nd", out object paths2ndObj))
                    {
                        if (paths2ndObj != null && paths2ndObj is TomlTable paths2nd)
                        {
                            // ネギラーメン・ワークスペースの 📄 `user_configuration.toml` ファイルへのパス
                            if (paths2nd.TryGetValue("user_configuration_file", out object userConfigurationFileObj))
                            {
                                if (userConfigurationFileObj is string userConfigurationFilePathAsStr)
                                {
                                    userConfigurationFilePath = Locations.Negiramen.UserConfiguration.FromString(
                                        userConfigurationFilePathAsStr,
                                        replaceSeparators: true);
                                }
                            }
                        }
                    }

                    //
                    // [profile]
                    // =========
                    //
                    if (document.TryGetValue("profile", out object profileObj))
                    {
                        if (profileObj != null && profileObj is TomlTable profile)
                        {
                            // あなたのサークル名
                            if (profile.TryGetValue("your_circle_name", out object yourCircleNameObj))
                            {
                                if (yourCircleNameObj != null && yourCircleNameObj is string yourCircleNameAsStr)
                                {
                                    yourCircleName = YourCircleName.FromString(yourCircleNameAsStr);
                                }
                            }

                            // あなたの作品名
                            if (profile.TryGetValue("your_work_name", out object yourWorkNameObj))
                            {
                                if (yourWorkNameObj != null && yourWorkNameObj is string yourWorkNameAsStr)
                                {
                                    yourWorkName = YourWorkName.FromString(yourWorkNameAsStr);
                                }
                            }
                        }
                    }
                }

                configuration = new Configuration(
                    negiramenWorkspaceFolderPath,
                    unityAssetsFolderPath,
                    userConfigurationFilePath,
                    yourCircleName,
                    yourWorkName);
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
        internal static bool SaveTOML(Configuration current, ConfigurationBuffer difference, out Configuration newConfiguration)
        {
            //
            // マルチプラットフォームの MAUI では、
            // パソコンだけではなく、スマホなどのサンドボックス環境などでの使用も想定されている
            // 
            // そのため、設定の保存／読込の操作は最小限のものしかない
            //
            // 📖　[Where to save .Net MAUI user settings](https://stackoverflow.com/questions/70599331/where-to-save-net-maui-user-settings)
            //
            // // getter
            // var value = Preferences.Get("nameOfSetting", "defaultValueForSetting");
            //
            // // setter
            // Preferences.Set("nameOfSetting", value);
            //
            //
            // しかし、2D RPG は　Windows PC で開発すると想定する。
            // そこで、 MAUI の範疇を外れ、Windows 固有のファイル・システムの API を使用することにする
            //
            // 📖　[File system helpers](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/file-system-helpers?tabs=windows)
            //

            // フォルダー名は自動的に与えられているので、これを使う
            string appDataDirAsStr = FileSystem.Current.AppDataDirectory;
            // Example: `C:/Users/むずでょ/AppData/Local/Packages/1802ca7b-559d-489e-8a13-f02ac4d27fcc_9zz4h110yvjzm/LocalState`

            // 保存したいファイルへのパス
            var configurationFilePath = System.IO.Path.Combine(appDataDirAsStr, "configuration.toml");

            var configurationBuffer = new ConfigurationBuffer();

            // 差分適用
            configurationBuffer.NegiramenWorkspaceFolderPath = difference.NegiramenWorkspaceFolderPath == null ? current.NegiramenWorkspaceFolderPath : difference.NegiramenWorkspaceFolderPath;
            configurationBuffer.UnityAssetsFolderPath = difference.UnityAssetsFolderPath == null ? current.UnityAssetsFolderPath : difference.UnityAssetsFolderPath;
            configurationBuffer.YourCircleName = difference.YourCircleName == null ? current.YourCircleName : difference.YourCircleName;
            configurationBuffer.YourWorkName = difference.YourWorkName == null ? current.YourWorkName : difference.YourWorkName;

            var text = $@"[paths]

# ネギラーメンの 📂 `Workspace` フォルダ―へのパス
negiramen_workspace_folder = ""{configurationBuffer.NegiramenWorkspaceFolderPath.AsStr}""

# Unity の Assets フォルダ―へのパス
unity_assets_folder = ""{configurationBuffer.UnityAssetsFolderPath.AsStr}""

[paths_2nd]

# ユーザー構成ファイルへのパス
user_configuration_file = ""{{negiramen_workspace_folder}}/user_configuration.toml""

[profile]

# あなたのサークル名
your_circle_name = ""{configurationBuffer.YourCircleName.AsStr}""

# あなたの作品名
your_work_name = ""{configurationBuffer.YourWorkName.AsStr}""
";

            // 上書き
            System.IO.File.WriteAllText(configurationFilePath, text);

            // イミュータブル・オブジェクトを生成
            newConfiguration = new Configuration(
                configurationBuffer.NegiramenWorkspaceFolderPath,
                configurationBuffer.UnityAssetsFolderPath,
                configurationBuffer.UserConfigurationFilePath,
                configurationBuffer.YourCircleName,
                configurationBuffer.YourWorkName);
            return true;
        }

        // - インターナル・プロパティー

        /// <summary>
        ///     ネギラーメン・ワークスペース・フォルダーへのパス
        /// </summary>
        /// <example>"C:/Users/むずでょ/Documents/GitHub/2D-RPG-Negiramen/Workspace"</example>
        internal Locations.Negiramen.WorkspaceFolder NegiramenWorkspaceFolderPath { get; }

        /// <summary>
        ///     Unity の Assets フォルダーへのパス
        /// </summary>
        /// <example>"C:/Users/むずでょ/Documents/Unity Projects/Negiramen Practice/Assets"</example>

/* プロジェクト '2D RPG Negiramen (net7.0-android)' からのマージされていない変更
前:
        internal Models.FileEntriesLocations.UnityAssetsFolderPath UnityAssetsFolderPath { get; }
後:
        internal UnityAssetsFolderPath UnityAssetsFolderPath { get; }
*/
        internal Locations.UnityAssetsFolder UnityAssetsFolderPath { get; }

        /// <summary>
        ///     ユーザー構成ファイルへのパス
        /// </summary>
        /// <example>"C:/Users/むずでょ/Documents/GitHub/2D-RPG-Negiramen/Workspace/configuration_2nd.toml"</example>

/* プロジェクト '2D RPG Negiramen (net7.0-android)' からのマージされていない変更
前:
        internal Models.FileEntriesLocations.Negiramen.UserConfigurationFilePath UserConfigurationFilePath { get; }
後:
        internal UserConfigurationFilePath UserConfigurationFilePath { get; }
*/
        internal Locations.Negiramen.UserConfiguration UserConfigurationFilePath { get; }

        /// <summary>
        ///     あなたのサークル名
        /// </summary>
        internal YourCircleName YourCircleName { get; }

        /// <summary>
        ///     あなたの作品名
        /// </summary>
        internal YourWorkName YourWorkName { get; }

        /// <summary>
        ///     生成
        /// </summary>
        internal Configuration() : this(

/* プロジェクト '2D RPG Negiramen (net7.0-android)' からのマージされていない変更
前:
            Models.FileEntriesLocations.Negiramen.WorkspaceFolderPath.Empty,
後:
            WorkspaceFolderPath.Empty,
*/
            Locations.Negiramen.WorkspaceFolder.Empty,

/* プロジェクト '2D RPG Negiramen (net7.0-android)' からのマージされていない変更
前:
            Models.FileEntriesLocations.UnityAssetsFolderPath.Empty,
後:
            UnityAssetsFolderPath.Empty,
*/
            Locations.UnityAssetsFolder.Empty,

/* プロジェクト '2D RPG Negiramen (net7.0-android)' からのマージされていない変更
前:
            Models.FileEntriesLocations.Negiramen.UserConfigurationFilePath.Empty,
後:
            UserConfigurationFilePath.Empty,
*/
            Locations.Negiramen.UserConfiguration.Empty,
            YourCircleName.Empty,
            YourWorkName.Empty)
        {
        }

        /// <summary>
        ///     生成
        /// </summary>
        /// <param name="negiramenWorkspaceFolderPath">ネギラーメン・ワークスペース・フォルダーへのパス</param>
        /// <param name="unityAssetsFolderPath">Unity の Assets フォルダーへのパス</param>
        /// <param name="userConfigurationFilePath">ユーザー構成ファイルへのパス</param>
        /// <param name="yourCircleName">あなたのサークル名</param>
        /// <param name="yourWorkName">あなたの作品名</param>
        internal Configuration(

/* プロジェクト '2D RPG Negiramen (net7.0-android)' からのマージされていない変更
前:
            Models.FileEntriesLocations.Negiramen.WorkspaceFolderPath negiramenWorkspaceFolderPath,
後:
            WorkspaceFolderPath negiramenWorkspaceFolderPath,
*/
            Locations.Negiramen.WorkspaceFolder negiramenWorkspaceFolderPath,

/* プロジェクト '2D RPG Negiramen (net7.0-android)' からのマージされていない変更
前:
            Models.FileEntriesLocations.UnityAssetsFolderPath unityAssetsFolderPath,
後:
            UnityAssetsFolderPath unityAssetsFolderPath,
*/
            Locations.UnityAssetsFolder unityAssetsFolderPath,

/* プロジェクト '2D RPG Negiramen (net7.0-android)' からのマージされていない変更
前:
            Models.FileEntriesLocations.Negiramen.UserConfigurationFilePath userConfigurationFilePath,
後:
            UserConfigurationFilePath userConfigurationFilePath,
*/
            Locations.Negiramen.UserConfiguration userConfigurationFilePath,
            YourCircleName yourCircleName,
            YourWorkName yourWorkName)
        {
            this.NegiramenWorkspaceFolderPath = negiramenWorkspaceFolderPath;
            this.UnityAssetsFolderPath = unityAssetsFolderPath;
            this.UserConfigurationFilePath = userConfigurationFilePath;
            this.YourCircleName = yourCircleName;
            this.YourWorkName = yourWorkName;
        }

        // - メソッド

        /// <summary>
        ///     構成ファイルは有効か？
        /// </summary>
        /// <returns>そうだ</returns>
        internal bool IsReady()
        {
            return this.ExistsNegiramenWorkspaceFolder() && this.ExistsUnityAssetsAutoGeneratedFolder();
        }

        // - プライベート・メソッド

        /// <summary>
        ///     ネギラーメンの 📂 `Workspace` フォルダ―は存在するか？
        /// </summary>
        /// <returns>そうだ</returns>
        bool ExistsNegiramenWorkspaceFolder()
        {
            return System.IO.Directory.Exists(this.NegiramenWorkspaceFolderPath.AsStr);
        }

        /// <summary>
        ///     📂 `{Unity の Assets}/{Your Circle Name}/{Your Work Name}/Auto Generated` フォルダ―は存在するか？
        /// </summary>
        /// <returns>そうだ</returns>
        bool ExistsUnityAssetsAutoGeneratedFolder() => System.IO.Directory.Exists(this.GetAutoGeneratedFolderPath());

        /// <summary>
        ///     フォルダー・パスの取得
        /// </summary>
        /// <returns></returns>
        /// <example>"C:/Users/むずでょ/Documents/Unity Projects/Negiramen Practice/Assets/Doujin Circle Negiramen/Negiramen Quest/Auto Generated"</example>
        string GetAutoGeneratedFolderPath()
        {
            return System.IO.Path.Combine(
                this.UnityAssetsFolderPath.AsStr,
                this.YourCircleName.AsStr,
                this.YourWorkName.AsStr,
                AutoGenerated);
        }
    }
}
