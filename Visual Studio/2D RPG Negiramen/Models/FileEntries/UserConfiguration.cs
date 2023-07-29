﻿namespace _2D_RPG_Negiramen.Models.FileEntries
{
    using Tomlyn;
    using Tomlyn.Model;
    using TheLocationOfUnityAssets = _2D_RPG_Negiramen.Models.FileEntries.Locations.UnityAssets;

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
                var userConfigurationFilePath = App.GetOrLoadConfiguration().NegiramenStarterKitFolder.UserConfigurationFile.Path.AsStr;
                // ユーザー構成ファイルへのパスは構成ファイルに与えられているので、これを使う
                // var userConfigurationFilePath = App.GetOrLoadConfiguration().UserConfigurationFile.Path.AsStr;
                // Example: `"C:/Users/むずでょ/Documents/GitHub/2D-RPG-Negiramen/Starter Kit/user_configuration.toml"`

                // 設定ファイルの読取
                var userConfigurationText = System.IO.File.ReadAllText(userConfigurationFilePath);


                Locations.StarterKit.WorkingTilesetImageFile workingTilesetImageFile = new Models.FileEntries.Locations.StarterKit.WorkingTilesetImageFile();

                TheLocationOfUnityAssets.ItsFolder unityAssetsFolder = new TheLocationOfUnityAssets.ItsFolder();

                // Locations.StarterKit.UserConfigurationFile userConfiguration = Models.FileEntries.Locations.StarterKit.UserConfigurationFile.Empty;
                YourCircleName yourCircleName = new YourCircleName();
                YourWorkName yourWorkName = new YourWorkName();

                // TOML
                TomlTable document = Toml.ToModel(userConfigurationText);

                if (document != null)
                {
                    // 準備
                }

                configuration = new UserConfiguration();
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

            //
            // 注意：　変数展開後のパスではなく、変数展開前のパス文字列を保存すること
            //
            var text = $@"# 準備
";

            // 上書き
            System.IO.File.WriteAllText(
                // 保存したいファイルへのパス
                path: App.GetOrLoadConfiguration().NegiramenStarterKitFolder.UserConfigurationFile.Path.AsStr,
                contents: text);

            // イミュータブル・オブジェクトを生成
            newConfiguration = new UserConfiguration();
            return true;
        }

        // - その他

        /// <summary>
        ///     生成
        /// </summary>
        internal UserConfiguration()
        {
        }
    }
}
