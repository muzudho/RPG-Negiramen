﻿namespace _2D_RPG_Negiramen.Models.FileEntries
{
    using Tomlyn;
    using Tomlyn.Model;
    using TheFileEntryLocations = _2D_RPG_Negiramen.Models.FileEntries.Locations;

    /// <summary>
    ///     😁 構成
    /// </summary>
    class Configuration
    {
        // - インターナル静的プロパティー

        internal static string AutoGenerated { get; } = "Auto Generated";

        // - インターナル静的メソッド

        #region メソッド（TOML形式ファイルの読取）
        /// <summary>
        ///     <pre>
        ///         TOML形式ファイルの読取
        ///     
        ///         📖　[Tomlyn　＞　Documentation](https://github.com/xoofx/Tomlyn/blob/main/doc/readme.md)
        ///     </pre>
        /// </summary>
        /// <param name="configuration">構成</param>
        /// <returns>TOMLテーブルまたはヌル</returns>
        internal static bool TryLoadTOML(string configurationFilePath, out Configuration configuration)
        {
            try
            {
                // 設定ファイルの読取
                var configurationText = System.IO.File.ReadAllText(configurationFilePath);


                TheFileEntryLocations.StarterKit.ItsFolder negiramenStarterKitFolder = new TheFileEntryLocations.StarterKit.ItsFolder();

                TheFileEntryLocations.UnityAssets.ItsFolder unityAssetsFolder = new TheFileEntryLocations.UnityAssets.ItsFolder();

                // TheFileEntryLocations.StarterKit.UserConfigurationFile userConfiguration = TheFileEntryLocations.StarterKit.UserConfigurationFile.Empty;
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
                            // ネギラーメンの 📂 `Starter Kit` フォルダ―へのパス
                            if (paths.TryGetValue("negiramen_starter_kit_folder", out object negiramenStarterKitFolderPathObj))
                            {
                                if (negiramenStarterKitFolderPathObj is string negiramenStarterKitFolderPathAsStr)
                                {
                                    negiramenStarterKitFolder = new TheFileEntryLocations.StarterKit.ItsFolder(
                                        pathSource: FileEntryPathSource.FromString(negiramenStarterKitFolderPathAsStr),
                                        convert: (pathSource) => FileEntryPath.From(pathSource,
                                                                                    replaceSeparators: true));
                                }
                            }

                            // Unity の Assets フォルダ―へのパス
                            if (paths.TryGetValue("unity_assets_folder", out object unityAssetsFolderPathObj))
                            {
                                if (unityAssetsFolderPathObj is string unityAssetsFolderPathAsStr)
                                {
                                    unityAssetsFolder = new TheFileEntryLocations.UnityAssets.ItsFolder(
                                        pathSource: FileEntryPathSource.FromString(unityAssetsFolderPathAsStr),
                                        convert: (pathSource) => FileEntryPath.From(pathSource,
                                                                                    replaceSeparators: true));
                                }
                            }
                        }
                    }

                    ////
                    //// [paths_2nd]
                    //// ===========
                    ////
                    //if (document.TryGetValue("paths_2nd", out object paths2ndObj))
                    //{
                    //    if (paths2ndObj != null && paths2ndObj is TomlTable paths2nd)
                    //    {
                    //        // ネギラーメン・ワークスペースの 📄 `user_configuration.toml` ファイルへのパス
                    //        if (paths2nd.TryGetValue("user_configuration_file", out object userConfigurationFileObj))
                    //        {
                    //            if (userConfigurationFileObj is string userConfigurationFilePathAsStr)
                    //            {
                    //                userConfiguration = new Locations.StarterKit.UserConfigurationFile(
                    //                    pathSource: FileEntryPathSource.FromString(userConfigurationFilePathAsStr),
                    //                    convert: (pathSource) => FileEntryPath.From(pathSource,
                    //                                                                replaceSeparators: true,
                    //                                                                // 変数展開のためのもの（その１）
                    //                                                                expandVariables: new Dictionary<string, string>()
                    //                                                                {
                    //                                                                    { "{negiramen_starter_kit_folder}", negiramenStarterKitFolder.Path.AsStr },
                    //                                                                    { "{unity_assets_folder}", unityAssetsFolder.Path.AsStr},
                    //                                                                }));
                    //            }
                    //        }
                    //    }
                    //}

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
                    negiramenStarterKitFolder,
                    unityAssetsFolder,
                    // userConfiguration,
                    yourCircleName,
                    yourWorkName);

                // 変数展開のためのもの（その２）
                configuration.Variables = new Dictionary<string, string>()
                    {
                        { "{negiramen_starter_kit_folder}", configuration.NegiramenStarterKitFolder.Path.AsStr },
                        { "{unity_assets_folder}", configuration.UnityAssetsFolder.Path.AsStr},
                    };

                return true;
            }
            catch (Exception ex)
            {
                // TODO 例外対応、何したらいい（＾～＾）？
                configuration = null;
                return false;
            }
        }
        #endregion

        #region メソッド（保存）
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

            // フォルダ名は自動的に与えられているので、これを使う
            string appDataDirAsStr = FileSystem.Current.AppDataDirectory;
            // Example: `C:/Users/むずでょ/AppData/Local/Packages/1802ca7b-559d-489e-8a13-f02ac4d27fcc_9zz4h110yvjzm/LocalState`

            // 保存したいファイルへのパス
            var configurationFilePath = System.IO.Path.Combine(appDataDirAsStr, "configuration.toml");

            var configurationBuffer = new ConfigurationBuffer();

            // 差分適用
            configurationBuffer.NegiramenStarterKitFolder = difference.NegiramenStarterKitFolder == null ? current.NegiramenStarterKitFolder : difference.NegiramenStarterKitFolder;
            configurationBuffer.UnityAssetsFolder = difference.UnityAssetsFolder == null ? current.UnityAssetsFolder : difference.UnityAssetsFolder;
            // configurationBuffer.UserConfigurationFile = difference.UserConfigurationFile == null ? current.UserConfigurationFile : difference.UserConfigurationFile;
            configurationBuffer.YourCircleName = difference.YourCircleName == null ? current.YourCircleName : difference.YourCircleName;
            configurationBuffer.YourWorkName = difference.YourWorkName == null ? current.YourWorkName : difference.YourWorkName;

            var text = $@"[paths]

# ネギラーメンの 📂 `Starter Kit` フォルダ―へのパス
negiramen_starter_kit_folder = ""{configurationBuffer.NegiramenStarterKitFolder.Path.AsStr}""

# Unity の Assets フォルダ―へのパス
unity_assets_folder = ""{configurationBuffer.UnityAssetsFolder.Path.AsStr}""

[profile]

# あなたのサークル名
your_circle_name = ""{configurationBuffer.YourCircleName.AsStr}""

# あなたの作品名
your_work_name = ""{configurationBuffer.YourWorkName.AsStr}""
";
            /*
[paths_2nd]

# ユーザー構成ファイルへのパス
user_configuration_file = ""{{negiramen_starter_kit_folder}}/user_configuration.toml""
             */

            // 上書き
            System.IO.File.WriteAllText(configurationFilePath, text);

            // イミュータブル・オブジェクトを生成
            newConfiguration = new Configuration(
                configurationBuffer.NegiramenStarterKitFolder,
                configurationBuffer.UnityAssetsFolder,
                // configurationBuffer.UserConfigurationFile,
                configurationBuffer.YourCircleName,
                configurationBuffer.YourWorkName);
            return true;
        }
        #endregion

        // - インターナル・プロパティー

        #region プロパティ（ネギラーメン 📂 `Starter Kit` フォルダの場所）
        /// <summary>
        ///     ネギラーメン 📂 `Starter Kit` フォルダの場所
        /// </summary>
        /// <example>"C:/Users/むずでょ/Documents/GitHub/2D-RPG-Negiramen/Starter Kit"</example>
        internal Locations.StarterKit.ItsFolder NegiramenStarterKitFolder { get; }
        #endregion

        #region プロパティ（Unity の 📂 `Assets` フォルダの場所）
        /// <summary>
        ///     Unity の 📂 `Assets` フォルダの場所
        /// </summary>
        /// <example>"C:/Users/むずでょ/Documents/Unity Projects/Negiramen Practice/Assets"</example>
        internal TheFileEntryLocations.UnityAssets.ItsFolder UnityAssetsFolder { get; }
        #endregion

        //#region プロパティ（ユーザー構成ファイルの場所）
        ///// <summary>
        /////     ユーザー構成ファイルの場所
        ///// </summary>
        ///// <example>"C:/Users/むずでょ/Documents/GitHub/2D-RPG-Negiramen/Starter Kit/configuration_2nd.toml"</example>
        //internal Locations.StarterKit.UserConfigurationFile UserConfigurationFile { get; }
        //#endregion

        #region プロパティ（あなたのサークル名）
        /// <summary>
        ///     あなたのサークル名
        /// </summary>
        internal YourCircleName YourCircleName { get; }
        #endregion

        #region プロパティ（あなたの作品名）
        /// <summary>
        ///     あなたの作品名
        /// </summary>
        internal YourWorkName YourWorkName { get; }
        #endregion

        // - その他

        #region その他（生成　関連）
        /// <summary>
        ///     生成
        /// </summary>
        internal Configuration() : this(
            Locations.StarterKit.ItsFolder.Empty,
            TheFileEntryLocations.UnityAssets.ItsFolder.Empty,
            // Locations.StarterKit.UserConfigurationFile.Empty,
            YourCircleName.Empty,
            YourWorkName.Empty)
        {
        }

        /// <summary>
        ///     生成
        /// </summary>
        /// <param name="negiramenStarterKitFolderPath">ネギラーメン 📂 `Starter Kit` フォルダへのパス</param>
        /// <param name="unityAssetsFolderPath">Unity の Assets フォルダへのパス</param>
        /// <param name="yourCircleName">あなたのサークル名</param>
        /// <param name="yourWorkName">あなたの作品名</param>
        internal Configuration(
            Locations.StarterKit.ItsFolder negiramenStarterKitFolderPath,
            TheFileEntryLocations.UnityAssets.ItsFolder unityAssetsFolderPath,
            // <param name="userConfigurationFilePath">ユーザー構成ファイルへのパス</param>
            // Locations.StarterKit.UserConfigurationFile userConfigurationFilePath,
            YourCircleName yourCircleName,
            YourWorkName yourWorkName)
        {
            this.NegiramenStarterKitFolder = negiramenStarterKitFolderPath;
            this.UnityAssetsFolder = unityAssetsFolderPath;
            // this.UserConfigurationFile = userConfigurationFilePath;
            this.YourCircleName = yourCircleName;
            this.YourWorkName = yourWorkName;
        }
        #endregion

        // - インターナル・プロパティ

        #region プロパティ（変数）
        /// <summary>
        ///     変数展開のためのもの
        /// </summary>
        internal Dictionary<string, string> Variables { get; private set; }
        #endregion

        // - インターナル・メソッド

        #region メソッド（構成ファイルは有効か？）
        /// <summary>
        ///     構成ファイルは有効か？
        /// </summary>
        /// <returns>そうだ</returns>
        internal bool IsReady()
        {
            return this.ExistsNegiramenStarterKitFolder() && this.UnityAssetsFolder.YourCircleNameFolder.YourWorkNameFolder.AutoGeneratedFolder.IsExists();
        }
        #endregion

        // - プライベート・メソッド

        #region メソッド（ネギラーメンの 📂 `Starter Kit` フォルダ―は存在するか？）
        /// <summary>
        ///     ネギラーメンの 📂 `Starter Kit` フォルダ―は存在するか？
        /// </summary>
        /// <returns>そうだ</returns>
        bool ExistsNegiramenStarterKitFolder()
        {
            return System.IO.Directory.Exists(this.NegiramenStarterKitFolder.Path.AsStr);
        }
        #endregion
    }
}
