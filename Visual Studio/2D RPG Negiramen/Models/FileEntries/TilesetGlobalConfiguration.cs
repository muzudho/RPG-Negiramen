﻿namespace _2D_RPG_Negiramen.Models.FileEntries;

using Tomlyn.Model;
using Tomlyn;
using TheFileEntryLocations = _2D_RPG_Negiramen.Models.FileEntries.Locations;

/// <summary>
///     😁 タイルセット・グローバル構成
///     
///     <list type="bullet">
///         <item>イミュータブル</item>
///     </list>
/// </summary>
internal class TilesetGlobalConfiguration
{
    // - その他

    #region その他（生成　関連）
    /// <summary>
    ///     読込。なければ作成
    ///     
    ///     <list type="bullet">
    ///         <item>ファイル・ステムが UUID ではないとき、タイルセット・グローバル構成ファイルは存在しないし、作成できない</item>
    ///     </list>
    /// </summary>
    /// <param name="tilesetPngLocation">タイルセットPNG画像ファイルの場所</param>
    /// <param name="newConfiguration">新ファイル</param>
    internal static bool LoadOrAdd(
        TheFileEntryLocations.UnityAssets.Images.TilesetPng tilesetPngLocation,
        out TilesetGlobalConfiguration? newConfiguration)
    {
        // 画像ファイルの名前は UUID という想定（UUIDじゃないかもしれない）
        var fileStem = tilesetPngLocation.GetStem();

        // タイルセット・グローバル構成ファイルの場所（TOML）
        var tilesetGlobalConfigurationLocation = App.GetOrLoadProjectConfiguration().UnityAssetsFolderLocation.YourCircleFolder.YourWorkFolder.AutoGeneratedFolder.ImagesFolder.TilesetsFolder.CreateTilesetGlobalToml(fileStem);

        // ファイルの存在確認
        if (tilesetGlobalConfigurationLocation.IsExists())
        {
            // あれば読込

            // 設定ファイルのテキスト読取
            var configurationText = System.IO.File.ReadAllText(tilesetGlobalConfigurationLocation.Path.AsStr);

            UUID? globalUuid = null;
            FileExtension? globalExtension = null;

            // TOML
            TomlTable document = Toml.ToModel(configurationText);

            if (document != null)
            {
                //
                // [global]
                // ========
                //
                if (document.TryGetValue("global", out object globalTomlObj))
                {
                    if (globalTomlObj != null && globalTomlObj is TomlTable globalTomlTable)
                    {
                        // UUID文字列
                        if (globalTomlTable.TryGetValue("uuid", out object uuidStringObj))
                        {
                            if (uuidStringObj is string uuidAsStr)
                            {
                                globalUuid = UUID.FromString(uuidAsStr);
                            }
                        }

                        // 拡張子文字列
                        if (globalTomlTable.TryGetValue("extension", out object extensionStringObj))
                        {
                            if (extensionStringObj is string extensionAsStr)
                            {
                                globalExtension = FileExtension.FromString(extensionAsStr);
                            }
                        }
                    }
                }
            }

            // ファイルを元に新規作成
            newConfiguration = new TilesetGlobalConfiguration(
                uuid: globalUuid ?? throw new Exception(),
                extension: globalExtension ?? throw new Exception());

            return true;
        }
        else
        {
            // ファイル・ステムが UUID かどうか確認
            if (UUIDHelper.IsMatch(tilesetPngLocation.GetStem().AsStr))
            {
                var uuid = UUID.FromString(tilesetPngLocation.GetStem().AsStr);

                // なければ新規作成
                newConfiguration = new TilesetGlobalConfiguration(
                    uuid: uuid,
                    extension: tilesetPngLocation.GetExtension());

                // ファイル書出し
                WriteTOML(
                    tilesetGlobalConfigurationLocation: tilesetGlobalConfigurationLocation,
                    configuration: newConfiguration);

                return true;
            }
            else
            {
                newConfiguration = null;
                return false;
            }
        }
    }

    /// <summary>
    ///     生成
    /// </summary>
    /// <param name="location">タイルセット・グローバル構成ファイルの場所</param>
    TilesetGlobalConfiguration(
        UUID uuid,
        FileExtension extension)
    {
        this.Uuid = uuid;
        this.Extension = extension;
    }
    #endregion

    // - インターナル静的メソッド

    #region メソッド（保存）
    /// <summary>
    ///     保存
    /// </summary>
    /// <param name="current">現在の構成</param>
    /// <param name="difference">現在の構成から更新した差分</param>
    /// <param name="newConfiguration">差分を反映した構成</param>
    /// <returns>完了した</returns>
    internal static bool SaveTOML(TheFileEntryLocations.UnityAssets.Images.TilesetGlobalToml tilesetGlobalConfigurationLocation, TilesetGlobalConfiguration current, TilesetGlobalConfigurationBuffer difference, out TilesetGlobalConfiguration newConfiguration)
    {
        var configurationBuffer = new TilesetGlobalConfigurationBuffer();

        // 差分適用
        configurationBuffer.Uuid = difference.Uuid ?? current.Uuid;
        configurationBuffer.Extension = difference.Extension ?? current.Extension;

        // 差分をマージして、イミュータブルに変換
        newConfiguration = new TilesetGlobalConfiguration(
            uuid: configurationBuffer.Uuid,
            extension: configurationBuffer.Extension);

        WriteTOML(
            tilesetGlobalConfigurationLocation: tilesetGlobalConfigurationLocation,
            configuration: newConfiguration);

        return true;
    }

    /// <summary>
    ///     テキストファイル書出し
    /// </summary>
    /// <param name="configuration"></param>
    internal static void WriteTOML(TheFileEntryLocations.UnityAssets.Images.TilesetGlobalToml tilesetGlobalConfigurationLocation, TilesetGlobalConfiguration configuration)
    {
        //
        // 注意：　変数展開後のパスではなく、変数展開前のパス文字列を保存すること
        //

        // タイトルは、ローカル構成ファイルの方へ入れる
        var text = $@"[global]

uuid = ""{configuration.Uuid}""
extension = ""{configuration.Extension}""

[user_defined]
";

        // 上書き
        System.IO.File.WriteAllText(
            path: tilesetGlobalConfigurationLocation.Path.AsStr,
            contents: text);
    }
    #endregion

    // - インターナル・プロパティ

    #region プロパティ（UUID）
    /// <summary>
    ///     UUID
    /// </summary>
    internal UUID Uuid { get; }
    #endregion

    #region プロパティ（拡張子）
    /// <summary>
    ///     拡張子
    /// </summary>
    internal FileExtension Extension { get; set; } = FileExtension.Empty;
    #endregion

    #region プロパティ（ファイル・ステム）
    /// <summary>
    ///     ファイル・ステム
    ///     
    ///     <list type="bullet">
    ///         <item><see cref="UuidObj"/>が分かっているときは、ファイル・ステムは使わない</item>
    ///     </list>
    /// </summary>
    internal FileStem FileStemObj { get; set; } = FileStem.Empty;
    #endregion

    #region プロパティ（公開日）
    /// <summary>
    ///     公開日
    /// </summary>
    internal DateTime PublishDate { get; set; } = DateTime.MinValue;
    #endregion

    #region プロパティ（UUID）
    /// <summary>
    ///     UUID
    /// </summary>
    internal UUID UuidObj { get; set; } = UUID.Empty;
    #endregion
}
