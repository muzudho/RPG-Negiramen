﻿namespace _2D_RPG_Negiramen.Specifications.TileCropPage;

using _2D_RPG_Negiramen.Models;
using _2D_RPG_Negiramen.ViewHistory.TileCropPage;

/// <summary>
///     削除ボタン
/// </summary>
internal class DeletesButton
{
    // - その他

    #region その他（生成）
    /// <summary>
    ///     生成
    /// </summary>
    /// <param name="specObj"></param>
    internal DeletesButton(
        ItsGardensideDoor gardensideDoor,
        ItsRoomsideDoors roomsideDoors)
    {
        this.GardensideDoor = gardensideDoor;
        this.RoomsideDoors = roomsideDoors;
    }
    #endregion

    // - インターナル・プロパティ

    #region プロパティ（活性性）
    /// <summary>
    ///     活性性
    /// </summary>
    public bool IsEnabled
    {
        get => this.isEnabled;
        set
        {
            if (this.isEnabled == value)
                return;

            this.isEnabled = value;
            this.GardensideDoor.PageVM.InvalidateDeletesButton();
        }
    }
    #endregion

    // - インターナル・メソッド

    #region メソッド（再描画）
    /// <summary>
    ///     再描画
    /// </summary>
    internal void Refresh()
    {
        var contents = this.RoomsideDoors.CropTile.SavesRecordVisually;

        if (contents.IsNone)
        {
            // 切抜きカーソル無し時
            this.IsEnabled = false;
            return;
        }

        // 切抜きカーソル有り時
        if (contents.Id == TileIdOrEmpty.Empty)
        {
            // Ｉｄ未設定時
            this.IsEnabled = false;
        }
        else
        {
            // タイル登録済み時
            this.IsEnabled = true;
        }
    }
    #endregion

    #region メソッド（タイル削除）
    /// <summary>
    ///     タイル削除
    /// </summary>
    public void RemoveTile()
    {
        App.History.Do(new RemoveRegisteredTileProcessing(
            gardensideDoor: this.GardensideDoor,
            tileIdOrEmpty: this.RoomsideDoors.CropTile.IdOrEmpty));

        this.GardensideDoor.PageVM.InvalidateForHistory();
    }
    #endregion

    // - プライベート・フィールド

    #region フィールド（活性性）
    /// <summary>
    ///     活性性
    /// </summary>
    bool isEnabled;
    #endregion

    // - プライベート・プロパティ

    ItsGardensideDoor GardensideDoor { get; }
    ItsRoomsideDoors RoomsideDoors { get; }
}
