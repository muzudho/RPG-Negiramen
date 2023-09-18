﻿namespace _2D_RPG_Negiramen.Hierarchy.Pages.TileCrop;

using _2D_RPG_Negiramen.Models;
using System.Diagnostics;
using TheGeometric = Models.Geometric;
using TheTileCropPage = _2D_RPG_Negiramen.Hierarchy.Pages.TileCrop;

/// <summary>
///     メンバー
///     
///     <list type="bullet">
///         <item>Mutable</item>
///         <item>密結合を認めるオブジェクトのコレクション</item>
///     </list>
/// </summary>
internal class ItsMembers
{
    // - その他

    #region その他（生成）
    /// <summary>
    ///     生成
    /// </summary>
    internal ItsMembers()
    {
        this.InnerCultureInfo = new InnerCultureInfo();
        this.PointingDevice = new PointingDevice();
        this.ZoomProperties = new ZoomProperties();
        this.GridUnit = new GridUnit();
        this.SelectedTile = new Tile();
        this.DeletesButton = new Button();
        this.TilesetSourceImageSize = TheGeometric.SizeInt.Empty;
        this.HalfThicknessOfGridLine = new(1);
    }
    #endregion

    // - インターナル・プロパティ

    #region プロパティ（切抜きカーソルと、既存タイルが交差しているか？）
    /// <summary>
    ///     切抜きカーソルと、既存タイルが交差しているか？
    /// </summary>
    /// <returns>そうだ</returns>
    internal bool HasIntersectionBetweenCroppedCursorAndRegisteredTile { get; set; }
    #endregion

    #region プロパティ（切抜きカーソルと、既存タイルは合同か？）
    /// <summary>
    ///     切抜きカーソルと、既存タイルは合同か？
    /// </summary>
    /// <returns>そうだ</returns>
    internal bool IsCongruenceBetweenCroppedCursorAndRegisteredTile { get; set; }
    #endregion

    #region プロパティ（文化情報）
    /// <summary>文化情報</summary>
    internal InnerCultureInfo InnerCultureInfo { get; }
    #endregion

    #region プロパティ（ポインティング・デバイス）
    /// <summary>ポインティング・デバイス</summary>
    internal PointingDevice PointingDevice { get; }
    #endregion

    #region プロパティ（ズーム）
    /// <summary>ズーム</summary>
    internal ZoomProperties ZoomProperties { get; }
    #endregion

    #region プロパティ（グリッド単位）
    /// <summary>グリッド単位</summary>
    internal GridUnit GridUnit { get; }
    #endregion

    #region プロパティ（切抜きカーソルが指すタイル）
    /// <summary>切抜きカーソルが指すタイル</summary>
    internal Tile SelectedTile { get; }
    #endregion

    #region プロパティ（削除ボタン）
    /// <summary>削除ボタン</summary>
    internal Button DeletesButton { get; }
    #endregion

    #region プロパティ（［タイルセット元画像］　関連）
    /// <summary>
    ///     ［タイルセット元画像］のサイズ
    /// </summary>
    internal TheGeometric.SizeInt TilesetSourceImageSize { get; set; }
    #endregion

    #region プロパティ（［元画像グリッド］の線の半分の太さ）
    /// <summary>
    ///     ［元画像グリッド］の線の半分の太さ
    ///     
    ///     <list type="bullet">
    ///         <item>変更通知プロパティ <see cref="HalfThicknessOfGridLineAsInt"/> に関わる</item>
    ///     </list>
    /// </summary>
    internal ThicknessOfLine HalfThicknessOfGridLine { get; }
    #endregion

    // - インターナル・メソッド

    #region メソッド（追加ボタンの状態取得）
    /// <summary>
    ///     追加ボタンの状態取得
    /// </summary>
    internal AddsButtonState GetStateOfAddsButton()
    {
        // 切抜きカーソルが、登録済みタイルのいずれかと交差しているか？
        if (this.HasIntersectionBetweenCroppedCursorAndRegisteredTile)
        {
            // 合同のときは「交差中」とは表示しない
            if (!this.IsCongruenceBetweenCroppedCursorAndRegisteredTile)
            {
                // Trace.WriteLine("[TileCropPage.xml.cs InvalidateAddsButton] 交差中だ");
                // 「交差中」
                return AddsButtonState.Intersecting;
            }
        }

        var contents = this.SelectedTile.RecordVisually;

        if (contents.IsNone)
        {
            // ［切抜きカーソル］の指すタイル無し時

            // 「追加」
            return AddsButtonState.Adds;
        }

        // 切抜きカーソル有り時
        // Ｉｄ未設定時
        if (this.SelectedTile.IdOrEmpty == TileIdOrEmpty.Empty)
        {
            // Ｉｄが空欄
            // ［追加］（新規作成）だ

            // ［追加」
            return AddsButtonState.Adds;
        }

        // ［復元」
        return AddsButtonState.Restore;
    }
    #endregion

    #region メソッド（追加ボタンのラベル算出）
    /// <summary>
    ///     追加ボタンのラベル算出
    /// </summary>
    internal string GetLabelOfAddsButton()
    {
        var addsButtonState = this.GetStateOfAddsButton();

        switch (addsButtonState)
        {
            case AddsButtonState.Intersecting:
                // 「交差中」
                return (string)LocalizationResourceManager.Instance["Intersecting"];

            case AddsButtonState.Adds:
                // 「追加」
                return (string)LocalizationResourceManager.Instance["Add"];

            case AddsButtonState.Restore:
                // ［復元」
                return (string)LocalizationResourceManager.Instance["Restore"];
        }

        // それ以外
        return string.Empty;
    }
    #endregion

    /// <summary>
    ///     <pre>
    ///         ［追加／復元］ボタンの活性性
    ///         
    ///         ※１　以下の条件を満たさないと、いずれにしても不活性
    ///     </pre>
    ///     <list type="bullet">
    ///         <item>［切抜きカーソルが指すタイル］が有る</item>
    ///     </list>
    ///     
    ///     ※２　［追加］ボタンは、３状態ある。以下の条件で活性
            // TODO 論理削除は難しいから廃止予定
    ///     <list type="bullet">
    ///         <item>Ｉｄが未設定時、かつ、論理削除フラグがＯｆｆ</item>
    ///     </list>
    ///     
    ///     ※３　［復元］ボタンは、以下の条件で活性
            // TODO 論理削除は難しいから廃止予定
    ///     <list type="bullet">
    ///         <item>Ｉｄが設定時、かつ、論理削除フラグがＯｎ</item>
    ///     </list>
    ///     
    ///     ※４　［交差中］ボタンは、常に不活性
    /// </summary>
    internal bool AddsButton_IsEnabled
    {
        get
        {
            // ※１
            if (this.SelectedTile.RecordVisually.IsNone)
            {
                return false;
            }

            var addsButtonState = this.GetStateOfAddsButton();
            bool enabled;

            switch (addsButtonState)
            {
                case TheTileCropPage.AddsButtonState.Adds:
                    {
                        // ※２
                        enabled = this.SelectedTile.RecordVisually.Id == TileIdOrEmpty.Empty && !this.SelectedTile.RecordVisually.LogicalDelete.AsBool;
                        Trace.WriteLine($"［デバッグ］　追加ボタンの活性性を {enabled} へ");
                    }
                    return enabled;

                case TheTileCropPage.AddsButtonState.Restore:
                    {
                        // ※３

                        // TODO 論理削除は難しいから廃止予定
                        // 画面にマークが見えないのに、タイルＩｄが入っていて、論理削除が False になっているケースがある？
                        enabled = this.SelectedTile.RecordVisually.Id != TileIdOrEmpty.Empty && this.SelectedTile.RecordVisually.LogicalDelete.AsBool;
                        Trace.WriteLine($"［デバッグ］　復元ボタンの活性性を {enabled} へ。 selectedTile:{this.SelectedTile.RecordVisually.Dump()}");
                    }
                    return enabled;

                case TheTileCropPage.AddsButtonState.Intersecting:
                    {
                        // ※４
                        enabled = false;
                        Trace.WriteLine($"［デバッグ］　交差中ボタンの活性性を {enabled} へ");
                    }
                    return enabled;

                default:
                    // ※４
                    Trace.WriteLine($"［デバッグ］　▲異常　追加ボタンの状態 {addsButtonState}");
                    return false;
            }
        }
    }

    #region メソッド（削除ボタンの活性性）
    /// <summary>
    ///     削除ボタンの活性性
    /// </summary>
    internal bool DeletesButton_IsEnabled
    {
        get
        {
            var contents = this.SelectedTile.RecordVisually;

            if (
                // 切抜きカーソル無し時
                contents.IsNone
                // TODO 論理削除は難しいから廃止予定
                // 論理削除時
                || contents.LogicalDelete.AsBool
                // Ｉｄ未設定時
                || contents.Id == TileIdOrEmpty.Empty)
            {
                // 不活性
                return false;
            }

            // タイル登録済み時
            return true;
        }
    }
    #endregion
}
