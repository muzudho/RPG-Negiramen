﻿namespace _2D_RPG_Negiramen.Models.FileEntries.Locations.UnityAssets
{
    using _2D_RPG_Negiramen.Coding;
    using _2D_RPG_Negiramen.Models;
    using _2D_RPG_Negiramen.Models.FileEntries.Locations;

    /// <summary>
    ///     😁 Unity の 📂 `Assets/｛あなたのサークル名｝/｛あなたの作品名｝/Auto Generated` フォルダーの場所
    ///     
    ///     <list type="bullet">
    ///         <item>イミュータブル</item>
    ///         <item><see cref="_2D_RPG_Negiramen.Models.FileEntries.Locations.UnityAssets.ItsFolder"/></item>
    ///     </list>
    /// </summary>
    class AutoGeneratedFolder : Its
    {
        // - その他

        #region その他（生成　関連）
        /// <summary>
        ///     生成
        /// </summary>
        internal AutoGeneratedFolder()
            : base()
        {
        }

        /// <summary>
        ///     生成
        /// </summary>
        internal AutoGeneratedFolder(FileEntryPathSource pathSource, Lazy.Convert<FileEntryPathSource, FileEntryPath> convert)
            : base(pathSource, convert)
        {
        }
        #endregion

        // - インターナル静的プロパティ

        #region プロパティ（空オブジェクト）
        /// <summary>
        ///     空オブジェクト
        /// </summary>
        internal static AutoGeneratedFolder Empty { get; } = new AutoGeneratedFolder();
        #endregion

        // - インターナル・プロパティ

        #region プロパティ（Unity の 📂 `Assets/｛あなたのサークル名｝/｛あなたの作品名｝/Auto Generated/Images` フォルダーの場所）
        /// <summary>
        ///     Unity の 📂 `Assets/｛あなたのサークル名｝/｛あなたの作品名｝/Auto Generated/Images` フォルダーの場所
        /// </summary>
        internal ImagesFolder ImagesFolder
        {
            get
            {
                if (imagesFolder == null)
                {
                    imagesFolder = new ImagesFolder(
                        pathSource: FileEntryPathSource.FromString(
                            System.IO.Path.Combine(Path.AsStr, "Images")),
                        convert: (pathSource) => FileEntryPath.From(pathSource,
                                                                    replaceSeparators: true));
                }

                return imagesFolder;
            }
        }
        #endregion

        // - プライベート・フィールド

        ImagesFolder? imagesFolder;
    }
}
