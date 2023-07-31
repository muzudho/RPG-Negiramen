﻿namespace _2D_RPG_Negiramen.Models.FileEntries
{
    using System.Text;
    using Tomlyn;
    using Tomlyn.Model;
    using TheFileEntryLocations = _2D_RPG_Negiramen.Models.FileEntries.Locations;

    /// <summary>
    ///     😁 構成
    /// </summary>
    class Configuration
    {
        // - その他

        #region その他（生成　関連）
        /// <summary>
        ///     生成
        /// </summary>
        /// <param name="negiramenStarterKitFolderPath">ネギラーメン 📂 `Starter Kit` フォルダへのパス</param>
        /// <param name="unityAssetsFolderPath">Unity の Assets フォルダへのパス</param>
        /// <param name="rememberYourCircleFolderName">（選択中の）あなたのサークル・フォルダ名</param>
        /// <param name="rememberYourWorkFolderName">（選択中の）あなたの作品フォルダ名</param>
        /// <param name="entryList">エントリー・リスト</param>
        internal Configuration(
            TheFileEntryLocations.StarterKit.ItsFolder negiramenStarterKitFolderPath,
            TheFileEntryLocations.UnityAssets.ItsFolder unityAssetsFolderPath,
            // <param name="starterKitConfigurationFilePath">ユーザー構成ファイルへのパス</param>
            // TheFileEntryLocations.StarterKit.StarterKitConfigurationFile starterKitConfigurationFilePath,
            YourCircleFolderName rememberYourCircleFolderName,
            YourWorkFolderName rememberYourWorkFolderName,
            List<ConfigurationEntry> entryList)
        {
            this.StarterKitFolder = negiramenStarterKitFolderPath;
            this.UnityAssetsFolder = unityAssetsFolderPath;
            // this.StarterKitConfigurationFile = starterKitConfigurationFilePath;
            this.RememberYourCircleFolderName = rememberYourCircleFolderName;
            this.RememberYourWorkFolderName = rememberYourWorkFolderName;
            this.EntryList = entryList;
        }
        #endregion

        // - インターナル静的プロパティー

        #region プロパティ（空オブジェクト）
        /// <summary>
        ///     空オブジェクト
        /// </summary>
        internal static Configuration Empty = new Configuration(
            negiramenStarterKitFolderPath: TheFileEntryLocations.StarterKit.ItsFolder.Empty,
            unityAssetsFolderPath: TheFileEntryLocations.UnityAssets.ItsFolder.Empty,
            // TheFileEntryLocations.StarterKit.StarterKitConfigurationFile.Empty,
            rememberYourCircleFolderName: YourCircleFolderName.Empty,
            rememberYourWorkFolderName: YourWorkFolderName.Empty,
            entryList: new List<ConfigurationEntry>());
        #endregion

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
        internal static bool TryLoadTOML(out Configuration configuration)
        {
            try
            {
                // 設定ファイルの読取
                var configurationText = System.IO.File.ReadAllText(TheFileEntryLocations.AppData.ConfigurationToml.Instance.Path.AsStr);


                TheFileEntryLocations.StarterKit.ItsFolder starterKitFolder = new TheFileEntryLocations.StarterKit.ItsFolder();

                TheFileEntryLocations.UnityAssets.ItsFolder unityAssetsFolder = new TheFileEntryLocations.UnityAssets.ItsFolder();

                // TheFileEntryLocations.StarterKit.StarterKitConfigurationFile starterKitConfiguration = TheFileEntryLocations.StarterKit.StarterKitConfigurationFile.Empty;
                List<ConfigurationEntry> entryList = new List<ConfigurationEntry>();
                YourCircleFolderName yourCircleFolderName = new YourCircleFolderName();
                YourWorkFolderName yourWorkFolderName = new YourWorkFolderName();

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
                            if (paths.TryGetValue("starter_kit_folder", out object starterKitFolderPathObj))
                            {
                                if (starterKitFolderPathObj is string starterKitFolderPathAsStr)
                                {
                                    starterKitFolder = new TheFileEntryLocations.StarterKit.ItsFolder(
                                        pathSource: FileEntryPathSource.FromString(starterKitFolderPathAsStr),
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
                    //        // ネギラーメン・ワークスペースの 📄 `starter_kit_configuration.toml` ファイルへのパス
                    //        if (paths2nd.TryGetValue("user_configuration_file", out object starterKitConfigurationFileObj))
                    //        {
                    //            if (starterKitConfigurationFileObj is string starterKitConfigurationFilePathAsStr)
                    //            {
                    //                starterKitConfiguration = new TheFileEntryLocations.StarterKit.StarterKitConfigurationFile(
                    //                    pathSource: FileEntryPathSource.FromString(starterKitConfigurationFilePathAsStr),
                    //                    convert: (pathSource) => FileEntryPath.From(pathSource,
                    //                                                                replaceSeparators: true,
                    //                                                                // 変数展開のためのもの（その１）
                    //                                                                expandVariables: new Dictionary<string, string>()
                    //                                                                {
                    //                                                                    { "{starter_kit_folder}", starterKitFolder.Path.AsStr },
                    //                                                                    { "{unity_assets_folder}", unityAssetsFolder.Path.AsStr},
                    //                                                                }));
                    //            }
                    //        }
                    //    }
                    //}

                    //
                    // [remember]
                    // ==========
                    //
                    if (document.TryGetValue("remember", out object rememberTomlObj))
                    {
                        if (rememberTomlObj != null && rememberTomlObj is TomlTable rememberTomlTable)
                        {
                            // あなたのサークル名
                            if (rememberTomlTable.TryGetValue("your_circle_folder_name", out object yourCircleFolderNameObj))
                            {
                                if (yourCircleFolderNameObj != null && yourCircleFolderNameObj is string yourCircleFolderNameAsStr)
                                {
                                    yourCircleFolderName = YourCircleFolderName.FromString(yourCircleFolderNameAsStr);
                                }
                            }

                            // あなたの作品名
                            if (rememberTomlTable.TryGetValue("your_work_folder_name", out object yourWorkFolderNameObj))
                            {
                                if (yourWorkFolderNameObj != null && yourWorkFolderNameObj is string yourWorkFolderNameAsStr)
                                {
                                    yourWorkFolderName = YourWorkFolderName.FromString(yourWorkFolderNameAsStr);
                                }
                            }
                        }
                    }

                    //
                    // [[entry]]
                    // =========
                    //
                    if (document.TryGetValue("entry", out object entryElement))
                    {
                        if (entryElement != null && entryElement is Tomlyn.Model.TomlTableArray entryTableArray)
                        {
                            foreach (var entryTable in entryTableArray)
                            {
                                YourCircleFolderName yourCircleFolderName2 = YourCircleFolderName.Empty;
                                YourWorkFolderName yourWorkFolderName2 = YourWorkFolderName.Empty;

                                // あなたのサークル名
                                if (entryTable.TryGetValue("your_circle_folder_name", out object yourCircleFolderNameObj))
                                {
                                    if (yourCircleFolderNameObj != null && yourCircleFolderNameObj is string yourCircleFolderNameAsStr)
                                    {
                                        yourCircleFolderName2 = YourCircleFolderName.FromString(yourCircleFolderNameAsStr);
                                    }
                                }

                                // あなたの作品名
                                if (entryTable.TryGetValue("your_work_folder_name", out object yourWorkFolderNameObj))
                                {
                                    if (yourWorkFolderNameObj != null && yourWorkFolderNameObj is string yourWorkFolderNameAsStr)
                                    {
                                        yourWorkFolderName2 = YourWorkFolderName.FromString(yourWorkFolderNameAsStr);
                                    }
                                }

                                entryList.Add(new ConfigurationEntry(yourCircleFolderName2, yourWorkFolderName2));
                            }
                        }
                    }
                }

                // ファイルを元に新規作成
                configuration = new Configuration(
                    starterKitFolder,
                    unityAssetsFolder,
                    // StarterKitConfiguration,
                    yourCircleFolderName,
                    yourWorkFolderName,
                    entryList);

                // 変数展開のためのもの（その２）
                configuration.Variables = new Dictionary<string, string>()
                    {
                        { "{starter_kit_folder}", configuration.StarterKitFolder.Path.AsStr },
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
            configurationBuffer.StarterKitFolder = difference.StarterKitFolder ?? current.StarterKitFolder;
            configurationBuffer.UnityAssetsFolder = difference.UnityAssetsFolder ?? current.UnityAssetsFolder;
            // configurationBuffer.StarterKitConfigurationFile = difference.StarterKitConfigurationFile ?? current.StarterKitConfigurationFile;
            configurationBuffer.RememberYourCircleFolderName = difference.RememberYourCircleFolderName?? current.RememberYourCircleFolderName;
            configurationBuffer.RememberYourWorkFolderName = difference.RememberYourWorkFolderName?? current.RememberYourWorkFolderName;
            configurationBuffer.EntryList = difference.EntryList ?? current.EntryList;

            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendLine($@"[paths]

# ネギラーメンの 📂 `Starter Kit` フォルダ―へのパス
starter_kit_folder = ""{configurationBuffer.StarterKitFolder.Path.AsStr}""

# Unity の 📂 `Assets` フォルダ―へのパス
unity_assets_folder = ""{configurationBuffer.UnityAssetsFolder.Path.AsStr}""

[remember]

# あなたのサークル・フォルダ名
your_circle_folder_name = ""{configurationBuffer.RememberYourCircleFolderName.AsStr}""

# あなたの作品フォルダ名
your_work_folder_name = ""{configurationBuffer.RememberYourWorkFolderName.AsStr}""
");
            /*
[paths_2nd]

# スターターキット構成ファイルへのパス
user_configuration_file = ""{{starter_kit_folder}}/starter_kit_configuration.toml""
             */

            foreach (var entry in configurationBuffer.EntryList)
            {
                strBuilder.AppendLine($@"[[entry]]

# あなたのサークル・フォルダ名
your_circle_folder_name = ""{entry.YourCircleFolderName.AsStr}""

# あなたの作品フォルダ名
your_work_folder_name = ""{entry.YourWorkFolderName.AsStr}""
");
            }

            // 上書き
            System.IO.File.WriteAllText(configurationFilePath, strBuilder.ToString());

            // 差分をマージして、イミュータブルに変換
            newConfiguration = new Configuration(
                configurationBuffer.StarterKitFolder,
                configurationBuffer.UnityAssetsFolder,
                // configurationBuffer.StarterKitConfigurationFile,
                configurationBuffer.RememberYourCircleFolderName,
                configurationBuffer.RememberYourWorkFolderName,
                configurationBuffer.EntryList);
            return true;
        }
        #endregion

        // - インターナル・プロパティー

        #region プロパティ（ネギラーメンに添付の 📂 `Starter Kit` フォルダの場所）
        /// <summary>
        ///     ネギラーメンに添付の 📂 `Starter Kit` フォルダの場所
        /// </summary>
        /// <example>"C:/Users/むずでょ/Documents/GitHub/2D-RPG-Negiramen/Starter Kit"</example>
        internal TheFileEntryLocations.StarterKit.ItsFolder StarterKitFolder { get; }
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
        //internal TheFileEntryLocations.StarterKit.StarterKitConfigurationFile StarterKitConfigurationFile { get; }
        //#endregion

        #region プロパティ（選択中のあなたのサークル・フォルダ名）
        /// <summary>
        ///     選択中のあなたのサークル・フォルダ・名
        /// </summary>
        internal YourCircleFolderName RememberYourCircleFolderName { get; }
        #endregion

        #region プロパティ（選択中のあなたの作品フォルダ名）
        /// <summary>
        ///     選択中のあなたの作品フォルダ名
        /// </summary>
        internal YourWorkFolderName RememberYourWorkFolderName { get; }
        #endregion

        #region プロパティ（エントリー・リスト）
        /// <summary>
        ///     エントリー・リスト
        /// </summary>
        internal List<ConfigurationEntry> EntryList { get; } = new List<ConfigurationEntry>();
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
            return this.ExistsNegiramenStarterKitFolder() && this.UnityAssetsFolder.YourCircleFolder.YourWorkFolder.AutoGeneratedFolder.IsExists();
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
            return System.IO.Directory.Exists(this.StarterKitFolder.Path.AsStr);
        }
        #endregion
    }
}
