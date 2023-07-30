﻿namespace _2D_RPG_Negiramen.Models.FileEntries.Locations.UnityAssets
{
    using _2D_RPG_Negiramen;
    using _2D_RPG_Negiramen.Coding;
    using _2D_RPG_Negiramen.Models;
    using TheFileEntryLocation = _2D_RPG_Negiramen.Models.FileEntries.Locations;

    /// <summary>
    ///     😁 Unityの 📂 `Assets` フォルダへのパス
    ///     
    ///     <list type="bullet">
    ///         <item>イミュータブル</item>
    ///         <item><see cref="App.Configuration"/></item>
    ///     </list>
    /// </summary>
    class ItsFolder : TheFileEntryLocation.Its
    {
        // - その他

        #region その他（生成　関連）
        /// <summary>
        ///     生成
        /// </summary>
        internal ItsFolder()
            : base()
        {
        }

        /// <summary>
        ///     生成
        /// </summary>
        internal ItsFolder(FileEntryPathSource pathSource, Lazy.Convert<FileEntryPathSource, FileEntryPath> convert)
            : base(pathSource, convert)
        {
        }
        #endregion

        // - インターナル静的プロパティ

        #region プロパティ（空オブジェクト）
        /// <summary>
        ///     空オブジェクト
        /// </summary>
        internal static ItsFolder Empty { get; } = new ItsFolder();
        #endregion

        // - インターナル・プロパティ

        #region プロパティ（Unityの 📂 `Assets/{あなたのサークル・フォルダ名}` フォルダの場所）
        /// <summary>
        ///     Unityの 📂 `Assets/{あなたのサークル・フォルダ名}` フォルダの場所
        /// </summary>
        internal YourCircleFolderNameFolder YourCircleFolderNameFolder
        {
            get
            {
                if (yourCircleFolderNameFolder == null)
                {
                    yourCircleFolderNameFolder = new YourCircleFolderNameFolder(
                        pathSource: FileEntryPathSource.FromString(
                            System.IO.Path.Combine(Path.AsStr, App.GetOrLoadConfiguration().RememberYourCircleFolderName.AsStr)),
                        convert: (pathSource) => FileEntryPath.From(pathSource,
                                                                    replaceSeparators: true));
                }

                return yourCircleFolderNameFolder;
            }
        }
        #endregion

        // - プライベート・フィールド

        YourCircleFolderNameFolder? yourCircleFolderNameFolder;
    }
}
