﻿namespace _2D_RPG_Negiramen.ViewModels
{
    using _2D_RPG_Negiramen.Coding;
    using _2D_RPG_Negiramen.Models;
    using _2D_RPG_Negiramen.Models.FileEntries.Locations.UnityAssets;
    using CommunityToolkit.Mvvm.ComponentModel;
    using SkiaSharp;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using TheGeometric = _2D_RPG_Negiramen.Models.Geometric;

    /// <summary>
    ///     😁 ［タイル切抜きページ］ビューモデル
    /// </summary>
    [QueryProperty(nameof(TilesetImageFile), queryId: "TilesetImageFile")]
    [QueryProperty(nameof(TilesetSettingsFile), queryId: "TilesetSettingsFile")]
    class TileCropPageViewModel : ObservableObject, ITileCropPageViewModel
    {
        // - その他

        #region その他（生成）
        /// <summary>
        ///     生成
        ///     
        ///     <list type="bullet">
        ///         <item>ビュー・モデルのデフォルト・コンストラクターは public 修飾にする必要がある</item>
        ///     </list>
        /// </summary>
        public TileCropPageViewModel()
        {
            // 循環参照しないように注意
            this.HalfThicknessOfTileCursorLine = new Models.ThicknessOfLine(2 * this.HalfThicknessOfGridLine.AsInt);
        }
        #endregion

        // - パブリック・プロパティ

        #region プロパティ（タイルセット設定）
        /// <summary>
        ///     タイルセット設定ファイルへのパス（文字列形式）
        /// </summary>
        public string TilesetSettingFilePathAsStr
        {
            get => _tilesetSettingsFile.Path.AsStr;
            set
            {
                if (value == null || String.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"the {nameof(TilesetSettingFilePathAsStr)} must not be null or whitespace");
                }

                if (_tilesetSettingsFile.Path.AsStr != value)
                {
                    _tilesetSettingsFile = new DataCsvTilesetCsv(
                        pathSource: FileEntryPathSource.FromString(value),
                        convert: (pathSource) => FileEntryPath.From(pathSource,
                                                                    replaceSeparators: true));
                }
            }
        }

        /// <summary>
        ///     タイルセット設定ファイルへのパス
        /// </summary>
        public DataCsvTilesetCsv TilesetSettingsFile
        {
            get => _tilesetSettingsFile;
            set
            {
                if (_tilesetSettingsFile != value)
                {
                    _tilesetSettingsFile = value;
                }
            }
        }
        #endregion

        #region プロパティ（タイルセット元画像）
        /// <summary>
        ///     タイルセット元画像ファイルへのパス
        /// </summary>
        public ImagesTilesetPng TilesetImageFile
        {
            get => tilesetImageFile;
            set
            {
                if (tilesetImageFile != value)
                {
                    tilesetImageFile = value;
                }
            }
        }

        /// <summary>
        ///     タイルセット画像ファイルへのパス（文字列形式）
        /// </summary>
        public string TilesetImageFilePathAsStr
        {
            get => tilesetImageFile.Path.AsStr;
            set
            {
                if (tilesetImageFile.Path.AsStr != value)
                {
                    tilesetImageFile = new ImagesTilesetPng(
                        pathSource: FileEntryPathSource.FromString(value),
                        convert: (pathSource) => FileEntryPath.From(pathSource,
                                                                    replaceSeparators: true));
                }
            }
        }

        // タイルセット元画像
        SKBitmap tilesetSourceBitmap = new SKBitmap();

        /// <summary>
        ///     タイルセット元画像
        /// </summary>
        public SKBitmap TilesetSourceBitmap
        {
            get => this.tilesetSourceBitmap;
        }

        /// <summary>
        ///     タイルセット元画像の設定
        /// </summary>
        /// <param name="bitmap"></param>
        public void SetTilesetSourceBitmap(SKBitmap bitmap)
        {
            if (this.tilesetSourceBitmap != bitmap)
            {
                this.tilesetSourceBitmap = bitmap;

                // タイルセット画像のサイズ設定（画像の再作成）
                this.tilesetSourceImageSize = Models.FileEntries.PNGHelper.GetImageSize(this.TilesetImageFile);
                OnPropertyChanged(nameof(TilesetSourceImageWidthAsInt));
                OnPropertyChanged(nameof(TilesetSourceImageHeightAsInt));

                // 作業画像の再作成
                this.RemakeWorkingTilesetImage();

                // グリッド・キャンバス画像の再作成
                this.RemakeGridCanvasImage();
            }
        }

        /// <summary>
        ///     タイルセット元画像のサイズ
        /// </summary>
        public Models.Geometric.SizeInt TilesetSourceImageSize => tilesetSourceImageSize;
        #endregion

        #region プロパティ（タイルセット作業画像関連）
        /// <summary>
        ///     タイルセット作業画像ファイルへのパス（文字列形式）
        /// </summary>
        public string TilesetWorkingImageFilePathAsStr
        {
            get => App.CacheFolder.YourCircleFolder.YourWorkFolder.ImagesFolder.WorkingTilesetPng.Path.AsStr;
        }

        /// <summary>
        ///     タイルセット作業画像。ビットマップ形式
        /// </summary>
        public SKBitmap TilesetWorkingBitmap { get; set; } = new SKBitmap();
        #endregion

        #region プロパティ（ズーム）
        /// <summary>
        ///     ズーム
        ///     
        ///     <list type="bullet">
        ///         <item>セッターは画像を再生成する重たい処理なので、スパムしないように注意</item>
        ///     </list>
        /// </summary>
        public Models.Geometric.Zoom Zoom
        {
            get => this.zoom;
            set
            {
                if (this.zoom != value)
                {
                    this.ZoomAsFloat = value.AsFloat;
                }
            }
        }

        /// <summary>
        ///     ズーム最大
        /// </summary>
        public float ZoomMaxAsFloat => this.zoomMax.AsFloat;

        /// <summary>
        ///     ズーム最小
        /// </summary>
        public float ZoomMinAsFloat => this.zoomMin.AsFloat;
        #endregion

        #region プロパティ（選択タイル）
        /// <summary>
        ///     選択タイル
        /// </summary>
        public Option<TileRecordViewModel> SelectedTileVMOption
        {
            get => this.selectedTileVMOption;
            set
            {
                if (this.selectedTileVMOption == value)
                {
                    // 値に変化がない
                    return;
                }

                TileRecordViewModel newTileRecordVM;

                if (value.TryGetValue(out newTileRecordVM))
                {
                    // 非ヌルの想定
                    if (newTileRecordVM == null)
                    {
                        throw new InvalidOperationException($"{nameof(newTileRecordVM)} must not be null");
                    }
                }
                else
                {
                    // クリアーの意図
                    newTileRecordVM = new TileRecordViewModel();
                }

                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel oldTileRecordVM))
                {
                    // 非ヌルの想定
                    if (oldTileRecordVM == null)
                    {
                        throw new InvalidOperationException($"{nameof(oldTileRecordVM)} must not be null");
                    }
                }
                else
                {
                    // タイル・カーソル無し時

                    // 新規作成
                    this.selectedTileVMOption = new Option<TileRecordViewModel>(new TileRecordViewModel());
                }

                // 変更通知を送りたいので、構成要素ごとに設定
                this.SelectedTileId = newTileRecordVM.Id;
                this.SourceCroppedCursorLeftAsInt = newTileRecordVM.SourceRectangle.Location.X.AsInt;
                this.SourceCroppedCursorTopAsInt = newTileRecordVM.SourceRectangle.Location.Y.AsInt;
                this.SourceCroppedCursorWidthAsInt = newTileRecordVM.SourceRectangle.Size.Width.AsInt;
                this.SourceCroppedCursorHeightAsInt = newTileRecordVM.SourceRectangle.Size.Height.AsInt;
                this.SelectedTileTitleAsStr = newTileRecordVM.Title.AsStr;

                OnPropertyChanged(nameof(AddsButtonHint));
                OnPropertyChanged(nameof(AddsButtonText));
                this.NotifyTileIdChange();
                // TODO 矩形もリフレッシュしたい
                // TODO コメントもリフレッシュしたい
            }
        }
        #endregion

        #region プロパティ（登録タイル　関連）
        /// <summary>
        ///     選択タイルＩｄ
        /// </summary>
        public Models.TileId SelectedTileId
        {
            get
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    return selectedTileVM.Id;
                }
                else
                {
                    // タイル・カーソル無し時
                    return Models.TileId.Empty;
                }
            }
            set
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    if (selectedTileVM.Id == value)
                    {
                        // 値に変化がない
                        return;
                    }

                    selectedTileVMOption = new Option<TileRecordViewModel>(TileRecordViewModel.FromModel(
                        tileRecord: new TileRecord(
                            id: value,
                            rect: selectedTileVM.SourceRectangle,
                            title: selectedTileVM.Title,
                            logicalDelete: selectedTileVM.LogicalDelete),
                        workingRect: selectedTileVM.SourceRectangle.Do(this.Zoom)));
                }
                else
                {
                    // タイル・カーソル無し時
                    selectedTileVMOption = new Option<TileRecordViewModel>(TileRecordViewModel.FromModel(
                        tileRecord: new Models.TileRecord(
                            id: value,
                            rect: Models.Geometric.RectangleInt.Empty,
                            title: Models.TileTitle.Empty,
                            logicalDelete: Models.LogicalDelete.False),
                    workingRect: Models.Geometric.RectangleFloat.Empty));
                }

                this.InvalidateLocale();

                // ［追加／上書き］ボタン再描画
                this.InvalidateAddsButton();

                // ［削除］ボタン再描画
                this.InvalidateDeletesButton();

                NotifyTileIdChange();
            }
        }
        #endregion

        // - パブリック変更通知プロパティ

        #region 変更通知プロパティ（ロケール　関連）
        /// <summary>
        ///     現在選択中の文化情報。文字列形式
        /// </summary>
        public string CultureInfoAsStr
        {
            get => LocalizationResourceManager.Instance.CultureInfo.Name;
            set
            {
                if (LocalizationResourceManager.Instance.CultureInfo.Name != value)
                {
                    LocalizationResourceManager.Instance.SetCulture(new CultureInfo(value));
                    OnPropertyChanged(nameof(CultureInfoAsStr));
                }
            }
        }

        /// <summary>
        ///     ロケールＩｄのリスト
        /// </summary>
        public ObservableCollection<string> LocaleIdCollection => App.LocaleIdCollection;
        #endregion

        #region 変更通知プロパティ（タイルセット設定ビューモデル）
        /// <summary>
        ///     タイルセット設定ビューモデル
        /// </summary>
        public TilesetSettingsViewModel TilesetSettingsVM
        {
            get => this._tilesetSettingsVM;
            set
            {
                if (this._tilesetSettingsVM != value)
                {
                    this._tilesetSettingsVM = value;
                    OnPropertyChanged(nameof(TilesetSettingsVM));

                    // TODO これ要るか？ 再描画
                    NotifyTileIdChange();
                }
            }
        }
        #endregion

        #region 変更通知プロパティ（タイルセット元画像関連）
        /// <summary>
        ///     タイルセット元画像の横幅。読取専用
        /// </summary>
        public int TilesetSourceImageWidthAsInt
        {
            get => tilesetSourceImageSize.Width.AsInt;
        }

        /// <summary>
        ///     タイルセット元画像の縦幅。読取専用
        /// </summary>
        public int TilesetSourceImageHeightAsInt
        {
            get => tilesetSourceImageSize.Height.AsInt;
        }
        #endregion

        #region 変更通知プロパティ（タイルセット作業画像関連）
        /// <summary>
        ///     タイルセット作業画像の横幅。読取専用
        /// </summary>
        public int TilesetWorkingImageWidthAsInt
        {
            get => workingImageSize.Width.AsInt;
        }

        /// <summary>
        ///     タイルセット作業画像の縦幅。読取専用
        /// </summary>
        public int TilesetWorkingImageHeightAsInt
        {
            get => workingImageSize.Height.AsInt;
        }
        #endregion

        #region 変更通知プロパティ（グリッド　関連）
        /// <summary>
        ///     グリッド・キャンバスの画像サイズ
        ///         
        ///     <list type="bullet">
        ///         <item>グリッドの線の太さを 2px と想定しているので、グリッドの線が画像の端っこで切れないように、グリッドの内部的なキャンバス・サイズを 2px 広げる</item>
        ///     </list>
        /// </summary>
        public Models.Geometric.SizeInt GridCanvasImageSize
        {
            get => this.gridCanvasImageSize;
            set
            {
                if (this.gridCanvasImageSize != value)
                {
                    this.GridCanvasImageWidthAsInt = value.Width.AsInt;
                    this.GridCanvasImageHeightAsInt = value.Height.AsInt;
                }
            }
        }

        /// <summary>
        ///     <pre>
        ///         グリッドのキャンバスの横幅
        ///         
        ///         グリッドの線の太さを 2px と想定しているので、グリッドの線が画像の端っこで切れないように、グリッドのキャンバス・サイズを 2px 広げる
        ///     </pre>
        /// </summary>
        public int GridCanvasImageWidthAsInt
        {
            get => this.GridCanvasImageSize.Width.AsInt;
            set
            {
                if (this.gridCanvasImageSize.Width.AsInt != value)
                {
                    this.gridCanvasImageSize = new Models.Geometric.SizeInt(new Models.Geometric.WidthInt(value), this.gridCanvasImageSize.Height);
                    OnPropertyChanged(nameof(GridCanvasImageWidthAsInt));
                    OnPropertyChanged(nameof(GridCanvasImageSize));
                }
            }
        }

        /// <summary>
        ///     <pre>
        ///         グリッドのキャンバスの縦幅
        ///         
        ///         グリッドの線の太さを 2px と想定しているので、グリッドの線が画像の端っこで切れないように、グリッドのキャンバス・サイズを 2px 広げる
        ///     </pre>
        /// </summary>
        public int GridCanvasImageHeightAsInt
        {
            get => this.GridCanvasImageSize.Height.AsInt;
            set
            {
                if (this.gridCanvasImageSize.Height.AsInt != value)
                {
                    this.gridCanvasImageSize = new Models.Geometric.SizeInt(this.gridCanvasImageSize.Width, new Models.Geometric.HeightInt(value));
                    OnPropertyChanged(nameof(GridCanvasImageHeightAsInt));
                    OnPropertyChanged(nameof(GridCanvasImageSize));
                }
            }
        }

        /// <summary>
        ///     グリッドの線の太さの半分
        /// </summary>
        public int HalfThicknessOfGridLineAsInt => this.HalfThicknessOfGridLine.AsInt;

        ThicknessOfLine halfThicknessOfGridLine = new Models.ThicknessOfLine(1);

        /// <summary>
        ///     グリッド線の半分の太さ
        /// </summary>
        internal ThicknessOfLine HalfThicknessOfGridLine
        {
            get => this.halfThicknessOfGridLine;
            set
            {
                if (this.halfThicknessOfGridLine != value)
                {
                    this.halfThicknessOfGridLine = value;
                    OnPropertyChanged(nameof(HalfThicknessOfGridLineAsInt));
                    OnPropertyChanged(nameof(HalfThicknessOfGridLine));
                }
            }
        }

        /// <summary>
        ///     グリッド位相の左上表示位置。元画像ベース
        /// </summary>
        Models.Geometric.PointInt sourceGridPhase = Models.Geometric.PointInt.Empty;

        /// <summary>
        ///     グリッド位相の左上表示位置。元画像ベース
        /// </summary>
        public Models.Geometric.PointInt SourceGridPhase
        {
            get => this.sourceGridPhase;
            set
            {
                if (this.sourceGridPhase != value)
                {
                    this.SourceGridPhaseLeftAsInt = value.X.AsInt;
                    this.SourceGridPhaseTopAsInt = value.Y.AsInt;
                }
            }
        }

        /// <summary>
        ///     グリッド位相の左上表示位置ｘ。元画像ベース
        /// </summary>
        public int SourceGridPhaseLeftAsInt
        {
            get => this.sourceGridPhase.X.AsInt;
            set
            {
                if (this.sourceGridPhase.X.AsInt != value)
                {
                    this.sourceGridPhase = new Models.Geometric.PointInt(new Models.Geometric.XInt(value), this.sourceGridPhase.Y);
                    this.WorkingGridPhaseLeftAsFloat = this.ZoomAsFloat * this.sourceGridPhase.X.AsInt;

                    // キャンバスを再描画
                    InvalidateCanvasOfGrid();

                    // キャンバスを再描画後に変更通知
                    OnPropertyChanged(nameof(SourceGridPhaseLeftAsInt));
                    OnPropertyChanged(nameof(SourceGridPhase));

                    OnPropertyChanged(nameof(WorkingGridPhaseLeftAsFloat));
                    OnPropertyChanged(nameof(WorkingGridPhase));
                }
            }
        }

        /// <summary>
        ///     グリッド位相の左上表示位置ｙ。元画像ベース
        /// </summary>
        public int SourceGridPhaseTopAsInt
        {
            get => this.sourceGridPhase.Y.AsInt;
            set
            {
                if (this.sourceGridPhase.Y.AsInt != value)
                {
                    this.sourceGridPhase = new Models.Geometric.PointInt(this.sourceGridPhase.X, new Models.Geometric.YInt(value));
                    this.WorkingGridPhaseTopAsFloat = (float)(this.ZoomAsFloat * this.sourceGridPhase.Y.AsInt);

                    // キャンバスを再描画
                    InvalidateCanvasOfGrid();

                    // キャンバスを再描画後に変更通知
                    OnPropertyChanged(nameof(SourceGridPhaseTopAsInt));
                    OnPropertyChanged(nameof(SourceGridPhase));

                    OnPropertyChanged(nameof(WorkingGridPhaseTopAsFloat));
                    OnPropertyChanged(nameof(WorkingGridPhase));
                }
            }
        }

        /// <summary>
        ///     グリッド位相の左上表示位置。ズーム後
        /// </summary>
        public Models.Geometric.PointFloat WorkingGridPhase
        {
            get => this.workingGridPhase;
            set
            {
                if (this.workingGridPhase != value)
                {
                    this.WorkingGridPhaseLeftAsFloat = value.X.AsFloat;
                    this.WorkingGridPhaseTopAsFloat = value.Y.AsFloat;
                }
            }
        }

        /// <summary>
        ///     グリッド位相の左上表示位置ｘ。ズーム後（読取専用）
        /// </summary>
        public float WorkingGridPhaseLeftAsFloat
        {
            get => this.workingGridPhase.X.AsFloat;
            set
            {
                if (this.workingGridPhase.X.AsFloat != value)
                {
                    this.workingGridPhase = new Models.Geometric.PointFloat(
                        x: new Models.Geometric.XFloat(value),
                        y: this.workingGridPhase.Y);

                    OnPropertyChanged(nameof(WorkingGridPhaseLeftAsFloat));
                    OnPropertyChanged(nameof(WorkingGridPhase));
                }
            }
        }

        /// <summary>
        ///     グリッド位相の左上表示位置ｙ。ズーム後（読取専用）
        /// </summary>
        public float WorkingGridPhaseTopAsFloat
        {
            get => this.workingGridPhase.Y.AsFloat;
            set
            {
                if (this.workingGridPhase.Y.AsFloat != value)
                {
                    this.workingGridPhase = new Models.Geometric.PointFloat(
                        x: this.workingGridPhase.X,
                        y: new Models.Geometric.YFloat(value));

                    OnPropertyChanged(nameof(WorkingGridPhaseTopAsFloat));
                    OnPropertyChanged(nameof(WorkingGridPhase));
                }
            }
        }

        /// <summary>
        ///     グリッド・タイルのサイズ。元画像ベース
        /// </summary>
        public Models.Geometric.SizeInt SourceGridUnit
        {
            get => this.sourceGridUnit;
            set
            {
                if (this.sourceGridUnit != value)
                {
                    this.SourceGridTileWidthAsInt = value.Width.AsInt;
                    this.SourceGridTileHeightAsInt = value.Height.AsInt;
                }
            }
        }

        /// <summary>
        ///     グリッド・タイルの横幅。元画像ベース
        /// </summary>
        public int SourceGridTileWidthAsInt
        {
            get => this.sourceGridUnit.Width.AsInt;
            set
            {
                if (this.sourceGridUnit.Width.AsInt != value &&
                    // バリデーション
                    0 < value && value <= this.TileMaxWidthAsInt)
                {
                    this.sourceGridUnit = new Models.Geometric.SizeInt(new Models.Geometric.WidthInt(value), this.sourceGridUnit.Height);

                    // 作業グリッド・タイル横幅の再計算
                    RefreshWorkingGridTileWidth();

                    // カーソルの線の幅を含まない
                    this.WorkingCroppedCursorWidthAsFloat = this.ZoomAsFloat * this.sourceGridUnit.Width.AsInt;

                    // キャンバスを再描画
                    InvalidateCanvasOfGrid();
                    // RefreshCanvasOfTileCursor(codePlace: "[TileCropPageViewModel SourceGridTileWidthAsInt set]");

                    // キャンバスを再描画後に変更通知
                    OnPropertyChanged(nameof(SourceGridTileWidthAsInt));
                    OnPropertyChanged(nameof(SourceGridUnit));
                }
            }
        }

        /// <summary>
        ///     グリッド・タイルの縦幅。元画像ベース
        /// </summary>
        public int SourceGridTileHeightAsInt
        {
            get => this.sourceGridUnit.Height.AsInt;
            set
            {
                if (this.sourceGridUnit.Height.AsInt != value &&
                    // バリデーション
                    0 < value && value <= this.TileMaxHeightAsInt)
                {
                    this.sourceGridUnit = new Models.Geometric.SizeInt(this.sourceGridUnit.Width, new Models.Geometric.HeightInt(value));

                    // 作業グリッド・タイル横幅の再計算
                    RefreshWorkingGridTileHeight();

                    // カーソルの線の幅を含まない
                    this.WorkingCroppedCursorHeightAsFloat = this.ZoomAsFloat * this.sourceGridUnit.Height.AsInt;

                    // キャンバスを再描画
                    InvalidateCanvasOfGrid();
                    // RefreshCanvasOfTileCursor(codePlace: "[TileCropPageViewModel SourceGridTileHeightAsInt set]");

                    // キャンバスを再描画後に変更通知
                    OnPropertyChanged(nameof(SourceGridTileHeightAsInt));
                    OnPropertyChanged(nameof(SourceGridUnit));
                }
            }
        }

        /// <summary>
        ///     グリッド・タイルのサイズ。ズーム後（読取専用）
        /// </summary>
        public Models.Geometric.SizeFloat WorkingGridUnit
        {
            get => this.workingGridUnit;
            set
            {
                if (this.workingGridUnit != value)
                {
                    this.WorkingGridTileWidthAsFloat = value.Width.AsFloat;
                    this.WorkingGridTileHeightAsFloat = value.Height.AsFloat;
                }
            }
        }

        /// <summary>
        ///     グリッド・タイルの横幅。ズーム後（読取専用）
        /// </summary>
        public float WorkingGridTileWidthAsFloat
        {
            get => this.workingGridUnit.Width.AsFloat;
            set
            {
                if (this.workingGridUnit.Width.AsFloat != value)
                {
                    this.workingGridUnit = new Models.Geometric.SizeFloat(
                        width: new Models.Geometric.WidthFloat(value),
                        height: this.WorkingGridUnit.Height);

                    OnPropertyChanged(nameof(WorkingGridTileWidthAsFloat));
                    OnPropertyChanged(nameof(WorkingGridUnit));
                }
            }
        }

        /// <summary>
        ///     グリッド・タイルの縦幅。ズーム後（読取専用）
        /// </summary>
        public float WorkingGridTileHeightAsFloat
        {
            get => this.workingGridUnit.Height.AsFloat;
            set
            {
                if (this.workingGridUnit.Height.AsFloat != value)
                {
                    this.workingGridUnit = new Models.Geometric.SizeFloat(
                        width: this.WorkingGridUnit.Width,
                        height: new Models.Geometric.HeightFloat(value));

                    OnPropertyChanged(nameof(WorkingGridTileHeightAsFloat));
                    OnPropertyChanged(nameof(WorkingGridUnit));
                }
            }
        }

        /// <summary>
        ///     グリッド・タイルの最大横幅
        /// </summary>
        public int TileMaxWidthAsInt
        {
            get => App.GetOrLoadSettings().TileMaxSize.Width.AsInt;
        }

        /// <summary>
        ///     グリッド・タイルの最大縦幅
        /// </summary>
        public int TileMaxHeightAsInt
        {
            get => App.GetOrLoadSettings().TileMaxSize.Height.AsInt;
        }
        #endregion

        #region 変更通知プロパティ（ズーム）
        /// <summary>
        ///     ズーム。整数形式
        ///     
        ///     <list type="bullet">
        ///         <item>セッターは画像を再生成する重たい処理なので、スパムしないように注意</item>
        ///     </list>
        /// </summary>
        public float ZoomAsFloat
        {
            get => this.zoom.AsFloat;
            set
            {
                if (this.zoom.AsFloat != value)
                {
                    if (this.ZoomMinAsFloat <= value && value <= this.ZoomMaxAsFloat)
                    {
                        this.zoom = new Models.Geometric.Zoom(value);

                        // 作業画像を再作成
                        this.RemakeWorkingTilesetImage();

                        // 作業グリッド・タイル横幅の再計算
                        RefreshWorkingGridTileWidth();

                        // 作業グリッド・タイル縦幅の再計算
                        RefreshWorkingGridTileHeight();

                        // グリッド・キャンバス画像の再作成
                        this.RemakeGridCanvasImage();

                        // 全ての登録タイルのズーム時の位置とサイズを更新
                        foreach (var registeredTileVM in this.TilesetSettingsVM.RecordViewModelList)
                        {
                            registeredTileVM.WorkingRectangle = registeredTileVM.SourceRectangle.Do(this.Zoom);
                        }

                        // 切抜きカーソルの位置とサイズを更新
                        this.WorkingCroppedCursorPoint = new TheGeometric.PointFloat(
                            x: new TheGeometric.XFloat(this.ZoomAsFloat * this.SourceCroppedCursorRect.Location.X.AsInt),
                            y: new TheGeometric.YFloat(this.ZoomAsFloat * this.SourceCroppedCursorRect.Location.Y.AsInt));
                        this.WorkingCroppedCursorSize = new TheGeometric.SizeFloat(
                            width: new TheGeometric.WidthFloat(this.ZoomAsFloat * this.SourceCroppedCursorRect.Size.Width.AsInt),
                            height: new TheGeometric.HeightFloat(this.ZoomAsFloat * this.SourceCroppedCursorRect.Size.Height.AsInt));

                        TrickRefreshCanvasOfTileCursor("[TileCropPageViewModel.cs ZoomAsFloat]");

                        OnPropertyChanged(nameof(ZoomAsFloat));
                        OnPropertyChanged(nameof(WorkingGridPhaseLeftAsFloat));
                        OnPropertyChanged(nameof(WorkingGridPhaseTopAsFloat));
                        OnPropertyChanged(nameof(WorkingGridPhase));

                        OnPropertyChanged(nameof(WorkingGridTileWidthAsFloat));
                        OnPropertyChanged(nameof(WorkingGridTileHeightAsFloat));
                        OnPropertyChanged(nameof(WorkingGridUnit));

                        // 切抜きカーソル。ズーム後
                        OnPropertyChanged(nameof(WorkingCroppedCursorPointAsMargin));
                        OnPropertyChanged(nameof(WorkingCroppedCursorCanvasWidthAsFloat));
                        OnPropertyChanged(nameof(WorkingCroppedCursorCanvasHeightAsFloat));
                        OnPropertyChanged(nameof(WorkingCroppedCursorSize));
                        OnPropertyChanged(nameof(WorkingCroppedCursorLeftAsPresentableText));   // TODO これは要るか？
                        OnPropertyChanged(nameof(WorkingCroppedCursorTopAsPresentableText));   // TODO これは要るか？
                        OnPropertyChanged(nameof(WorkingCroppedCursorWidthAsPresentableText));   // TODO これは要るか？
                        OnPropertyChanged(nameof(WorkingCroppedCursorHeightAsPresentableText));   // TODO これは要るか？
                    }
                }
            }
        }
        #endregion

        #region 変更通知プロパティ（追加／上書きボタンのラベル）
        string addsButtonText = string.Empty;

        /// <summary>
        ///     追加／上書きボタンのラベル
        /// </summary>
        public string AddsButtonText
        {
            get
            {
                return this.addsButtonText;
            }
            set
            {
                if (this.addsButtonText != value)
                {
                    this.addsButtonText = value;
                    OnPropertyChanged(nameof(AddsButtonText));
                }
            }
        }
        #endregion

        #region 変更通知プロパティ（追加／上書きボタンのツールチップ・ヒント）
        /// <summary>
        ///     追加／上書きボタンのツールチップ・ヒント
        /// </summary>
        public string AddsButtonHint
        {
            get
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    if (selectedTileVM.Id == Models.TileId.Empty)
                    {
                        // 未選択時
                        return "選択タイルを、タイル一覧画面へ追加";
                    }
                    else
                    {
                        return "選択タイルを、タイル一覧画面へ上書";
                    }
                }
                else
                {
                    // タイル・カーソル無し時
                    return "選択タイルを、タイル一覧画面へ追加";
                }
            }
        }
        #endregion

        #region 変更通知プロパティ（追加／上書ボタンの活性性）
        bool addsButtonIsEnabled;

        /// <summary>
        ///     追加／上書ボタンの活性性
        /// </summary>
        public bool AddsButtonIsEnabled
        {
            get
            {
                return this.addsButtonIsEnabled;
            }
            set
            {
                if (this.addsButtonIsEnabled != value)
                {
                    this.addsButtonIsEnabled = value;
                    OnPropertyChanged(nameof(AddsButtonIsEnabled));
                }
            }
        }
        #endregion

        #region 変更通知プロパティ（削除ボタンの活性性）
        bool deletesButtonIsEnabled;

        /// <summary>
        ///     削除ボタンの活性性
        /// </summary>
        public bool DeletesButtonIsEnabled
        {
            get
            {
                return this.deletesButtonIsEnabled;
            }
            set
            {
                if (this.deletesButtonIsEnabled != value)
                {
                    this.deletesButtonIsEnabled = value;
                    OnPropertyChanged(nameof(DeletesButtonIsEnabled));
                }
            }
        }
        #endregion

        #region 変更通知プロパティ（タイル・カーソルの線の半分の太さ）
        ThicknessOfLine halfThicknessOfTileCursorLine;

        /// <summary>
        ///     タイル・カーソルの線の半分の太さ
        /// </summary>
        public ThicknessOfLine HalfThicknessOfTileCursorLine
        {
            get
            {
                return this.halfThicknessOfTileCursorLine;
            }
            set
            {
                if (this.halfThicknessOfTileCursorLine != value)
                {
                    this.halfThicknessOfTileCursorLine = value;
                    OnPropertyChanged(nameof(HalfThicknessOfTileCursorLine));
                }
            }
        }
        #endregion

        #region 変更通知プロパティ（ポインティング・デバイス押下中か？）
        bool selectingOnPointingDevice;

        /// <summary>
        ///     ポインティング・デバイス押下中か？
        /// 
        ///     <list type="bullet">
        ///         <item>タイルを選択開始していて、まだ未確定だ</item>
        ///         <item>マウスじゃないと思うけど</item>
        ///     </list>
        /// </summary>
        public bool IsMouseDragging
        {
            get => this.selectingOnPointingDevice;
            set
            {
                if (this.selectingOnPointingDevice != value)
                {
                    this.selectingOnPointingDevice = value;
                    OnPropertyChanged(nameof(IsMouseDragging));
                }
            }
        }
        #endregion

        #region 変更通知プロパティ（選択タイルＩｄ。BASE64表現）
        /// <summary>
        ///     選択タイルＩｄ。BASE64表現
        ///     
        ///     <see cref="SelectedTileId"/>
        /// </summary>
        public string SelectedTileIdAsBASE64
        {
            get
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    return selectedTileVM.Id.AsBASE64;
                }
                else
                {
                    // タイル・カーソル無し時
                    return string.Empty;
                }
            }
        }
        #endregion

        #region 変更通知プロパティ（選択タイルＩｄ。フォネティックコード表現）
        /// <summary>
        ///     選択タイルＩｄ。フォネティックコード表現
        ///     
        ///     <see cref="SelectedTileId"/>
        /// </summary>
        public string SelectedTileIdAsPhoneticCode
        {
            get
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    return selectedTileVM.Id.AsPhoneticCode;
                }
                else
                {
                    // タイル・カーソル無し時
                    return string.Empty;
                }
            }
        }
        #endregion

        #region 変更通知プロパティ（切抜きカーソル。元画像ベース　関連）
        /// <summary>
        ///     切抜きカーソル。元画像ベースの矩形
        ///     
        ///     <list type="bullet">
        ///         <item>カーソルが無いとき、大きさの無いカーソルを返す</item>
        ///     </list>
        /// </summary>
        public Models.Geometric.RectangleInt SourceCroppedCursorRect
        {
            get
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    return selectedTileVM.SourceRectangle;
                }
                else
                {
                    // タイル・カーソル無し時
                    return Models.Geometric.RectangleInt.Empty;
                }
            }
            set
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    if (selectedTileVM.SourceRectangle == value)
                    {
                        // 値に変化がない
                        return;
                    }
                }
                else
                {
                    // タイル・カーソル無し時
                }

                this.SourceCroppedCursorLeftAsInt = value.Location.X.AsInt;
                this.SourceCroppedCursorTopAsInt = value.Location.Y.AsInt;
                this.SourceCroppedCursorSize = value.Size;

                // 切抜きカーソル。ズーム済み
                this.WorkingCroppedCursorLeftAsFloat = this.ZoomAsFloat * this.SourceCroppedCursorLeftAsInt;
                this.WorkingCroppedCursorTopAsFloat = this.ZoomAsFloat * this.SourceCroppedCursorTopAsInt;
                this.WorkingCroppedCursorSize = new Models.Geometric.SizeFloat(
                    width: new Models.Geometric.WidthFloat(this.ZoomAsFloat * value.Size.Width.AsInt),
                    height: new Models.Geometric.HeightFloat(this.ZoomAsFloat * value.Size.Height.AsInt));
            }
        }

        /// <summary>
        ///     矩形カーソル。元画像ベースの位置ｘ
        /// </summary>
        public int SourceCroppedCursorLeftAsInt
        {
            get
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    return selectedTileVM.SourceRectangle.Location.X.AsInt;
                }
                else
                {
                    // タイル・カーソル無し時
                    return 0;
                }
            }
            set
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    if (selectedTileVM.SourceRectangle.Location.X.AsInt == value)
                    {
                        // 値に変化がない
                        return;
                    }

                    // 元画像ベース
                    var rect1 = new Models.Geometric.RectangleInt(
                                location: new Models.Geometric.PointInt(new Models.Geometric.XInt(value), selectedTileVM.SourceRectangle.Location.Y),
                                size: selectedTileVM.SourceRectangle.Size);
                    this.selectedTileVMOption = new Option<TileRecordViewModel>(TileRecordViewModel.FromModel(
                        tileRecord: new Models.TileRecord(
                            id: selectedTileVM.Id,
                            rect: rect1,
                            title: selectedTileVM.Title,
                            logicalDelete: selectedTileVM.LogicalDelete),
                        workingRect: rect1.Do(this.Zoom)));
                }
                else
                {
                    // タイル・カーソル無し時

                    // 元画像ベース
                    var rect1 = new Models.Geometric.RectangleInt(
                                location: new Models.Geometric.PointInt(new Models.Geometric.XInt(value), Models.Geometric.YInt.Empty),
                                size: Models.Geometric.SizeInt.Empty);
                    this.selectedTileVMOption = new Option<TileRecordViewModel>(TileRecordViewModel.FromModel(
                        tileRecord: new Models.TileRecord(
                            id: Models.TileId.Empty,
                            rect: rect1,
                            title: Models.TileTitle.Empty,
                            logicalDelete: Models.LogicalDelete.False),
                        workingRect: rect1.Do(this.Zoom)));
                }

                // 切抜きカーソル。ズーム済み
                this.WorkingCroppedCursorLeftAsFloat = this.ZoomAsFloat * this.SourceCroppedCursorLeftAsInt;
                this.WorkingCroppedCursorTopAsFloat = this.ZoomAsFloat * this.SourceCroppedCursorTopAsInt;
                // TODO サイズは変化無しか？

                OnPropertyChanged(nameof(SourceCroppedCursorLeftAsInt));
                OnPropertyChanged(nameof(SourceCroppedCursorRect));
            }
        }

        /// <summary>
        ///     切抜きカーソル。元画像ベースの位置ｙ
        /// </summary>
        public int SourceCroppedCursorTopAsInt
        {
            get
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    return selectedTileVM.SourceRectangle.Location.Y.AsInt;
                }
                else
                {
                    // タイル・カーソル無し時
                    return 0;
                }
            }
            set
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    if (selectedTileVM.SourceRectangle.Location.Y.AsInt == value)
                    {
                        // 値に変化がない
                        return;
                    }

                    // 元画像ベース
                    var rect1 = new Models.Geometric.RectangleInt(
                            location: new Models.Geometric.PointInt(selectedTileVM.SourceRectangle.Location.X, new Models.Geometric.YInt(value)),
                            size: selectedTileVM.SourceRectangle.Size);
                    selectedTileVMOption = new Option<TileRecordViewModel>(TileRecordViewModel.FromModel(
                        tileRecord: new Models.TileRecord(
                            id: selectedTileVM.Id,
                            rect: rect1,
                            title: selectedTileVM.Title,
                            logicalDelete: selectedTileVM.LogicalDelete),
                        workingRect: rect1.Do(this.Zoom)));
                }
                else
                {
                    // タイル・カーソル無し時

                    // 元画像ベース
                    var rect1 = new Models.Geometric.RectangleInt(
                            location: new Models.Geometric.PointInt(Models.Geometric.XInt.Empty, new Models.Geometric.YInt(value)),
                            size: Models.Geometric.SizeInt.Empty);
                    selectedTileVMOption = new Option<TileRecordViewModel>(TileRecordViewModel.FromModel(
                        tileRecord: new Models.TileRecord(
                            id: Models.TileId.Empty,
                            rect: rect1,
                            title: Models.TileTitle.Empty,
                            logicalDelete: Models.LogicalDelete.False),
                        workingRect: rect1.Do(this.Zoom)));
                }

                // 切抜きカーソル。ズーム済み
                this.WorkingCroppedCursorLeftAsFloat = this.ZoomAsFloat * this.SourceCroppedCursorLeftAsInt;
                this.WorkingCroppedCursorTopAsFloat = this.ZoomAsFloat * this.SourceCroppedCursorTopAsInt;
                // TODO サイズは変化無しか？

                OnPropertyChanged(nameof(SourceCroppedCursorTopAsInt));
                OnPropertyChanged(nameof(SourceCroppedCursorRect));
            }
        }

        /// <summary>
        ///     矩形カーソル。元画像ベースのサイズ
        ///     
        ///     <list type="bullet">
        ///         <item>線の太さを含まない</item>
        ///     </list>
        /// </summary>
        public Models.Geometric.SizeInt SourceCroppedCursorSize
        {
            get
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    return selectedTileVM.SourceRectangle.Size;
                }
                else
                {
                    // タイル・カーソル無し時
                    return Models.Geometric.SizeInt.Empty;
                }
            }
            set
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    if (selectedTileVM.SourceRectangle.Size == value)
                    {
                        // 値に変化がない
                        return;
                    }
                }
                else
                {
                    // タイル・カーソル無し時
                }

                //
                // 選択タイルの横幅と縦幅
                // ======================
                //
                this.SourceCroppedCursorWidthAsInt = value.Width.AsInt;
                this.SourceCroppedCursorHeightAsInt = value.Height.AsInt;

                OnPropertyChanged(nameof(SourceCroppedCursorRect));
            }
        }

        /// <summary>
        ///     切抜きカーソル。元画像ベースの横幅
        /// </summary>
        public int SourceCroppedCursorWidthAsInt
        {
            get
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    return selectedTileVM.SourceRectangle.Size.Width.AsInt;
                }
                else
                {
                    // タイル・カーソル無し時
                    return 0;
                }
            }
            set
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    if (selectedTileVM.SourceRectangle.Size.Width.AsInt == value)
                    {
                        // 値に変化がない
                        return;
                    }

                    var rect1 = new Models.Geometric.RectangleInt(selectedTileVM.SourceRectangle.Location, new Models.Geometric.SizeInt(new Models.Geometric.WidthInt(value), selectedTileVM.SourceRectangle.Size.Height));
                    selectedTileVMOption = new Option<TileRecordViewModel>(TileRecordViewModel.FromModel(
                        tileRecord: new Models.TileRecord(
                            id: selectedTileVM.Id,
                            rect: rect1,
                            title: selectedTileVM.Title,
                            logicalDelete: selectedTileVM.LogicalDelete),
                        workingRect: rect1.Do(this.Zoom)));
                }
                else
                {
                    // タイル・カーソル無し時
                    var rect1 = new Models.Geometric.RectangleInt(Models.Geometric.PointInt.Empty, new Models.Geometric.SizeInt(new Models.Geometric.WidthInt(value), Models.Geometric.HeightInt.Empty));
                    selectedTileVMOption = new Option<TileRecordViewModel>(TileRecordViewModel.FromModel(
                        tileRecord: new Models.TileRecord(
                            id: Models.TileId.Empty,
                            rect: rect1,
                            title: Models.TileTitle.Empty,
                            logicalDelete: Models.LogicalDelete.False),
                        workingRect: rect1.Do(this.Zoom)));
                }

                // 矩形カーソル。ズーム済み（カーソルの線の幅を含まない）
                WorkingCroppedCursorWidthAsFloat = this.ZoomAsFloat * value;

                OnPropertyChanged(nameof(SourceCroppedCursorWidthAsInt));
                OnPropertyChanged(nameof(SourceCroppedCursorSize));
                OnPropertyChanged(nameof(SourceCroppedCursorRect));
            }
        }

        /// <summary>
        ///     切抜きカーソル。元画像ベースの縦幅
        /// </summary>
        public int SourceCroppedCursorHeightAsInt
        {
            get
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    return selectedTileVM.SourceRectangle.Size.Height.AsInt;
                }
                else
                {
                    // タイル・カーソル無し時
                    return 0;
                }
            }
            set
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    if (selectedTileVM.SourceRectangle.Size.Height.AsInt == value)
                    {
                        // 値に変化がない
                        return;
                    }

                    var rect1 = new Models.Geometric.RectangleInt(selectedTileVM.SourceRectangle.Location, new Models.Geometric.SizeInt(selectedTileVM.SourceRectangle.Size.Width, new Models.Geometric.HeightInt(value)));
                    selectedTileVMOption = new Option<TileRecordViewModel>(TileRecordViewModel.FromModel(
                        tileRecord: new Models.TileRecord(
                            id: selectedTileVM.Id,
                            rect: rect1,
                            title: selectedTileVM.Title,
                            logicalDelete: selectedTileVM.LogicalDelete),
                        workingRect: rect1.Do(this.Zoom)));
                }
                else
                {
                    // タイル・カーソル無し時
                    var rect1 = new Models.Geometric.RectangleInt(Models.Geometric.PointInt.Empty, new Models.Geometric.SizeInt(Models.Geometric.WidthInt.Empty, new Models.Geometric.HeightInt(value)));
                    selectedTileVMOption = new Option<TileRecordViewModel>(TileRecordViewModel.FromModel(
                        tileRecord: new Models.TileRecord(
                            id: TileId.Empty,
                            rect: rect1,
                            title: Models.TileTitle.Empty,
                            logicalDelete: Models.LogicalDelete.False),
                        workingRect: rect1.Do(this.Zoom)));
                }

                // 切抜きカーソル。ズーム済みの縦幅（カーソルの線の幅を含まない）
                WorkingCroppedCursorHeightAsFloat = this.ZoomAsFloat * value;

                OnPropertyChanged(nameof(SourceCroppedCursorHeightAsInt));
                OnPropertyChanged(nameof(SourceCroppedCursorSize));
                OnPropertyChanged(nameof(SourceCroppedCursorRect));
            }
        }
        #endregion

        #region 変更通知プロパティ（矩形カーソル。ズーム済み　関連）
        /// <summary>
        ///     矩形カーソル。ズーム済みの位置（マージンとして）
        /// </summary>
        public Thickness WorkingCroppedCursorPointAsMargin
        {
            get => new Thickness(left: this.WorkingCroppedCursorLeftAsFloat,
                                 top: this.WorkingCroppedCursorTopAsFloat,
                                 right: 0,
                                 bottom: 0);
        }

        /// <summary>
        ///     矩形カーソル。ズーム済みの位置
        ///         
        ///     <list type="bullet">
        ///         <item>カーソルの線の幅を含まない</item>
        ///     </list>
        /// </summary>
        public Models.Geometric.PointFloat WorkingCroppedCursorPoint
        {
            get => this.workingCroppedCursorPoint;
            set
            {
                if (this.workingCroppedCursorPoint != value)
                {
                    this.WorkingCroppedCursorLeftAsFloat = value.X.AsFloat;
                    this.WorkingCroppedCursorTopAsFloat = value.Y.AsFloat;
                }
            }
        }

        /// <summary>
        ///     矩形カーソル。ズーム済みの位置ｘ
        ///         
        ///     <list type="bullet">
        ///         <item>カーソルの線の幅を含まない</item>
        ///     </list>
        /// </summary>
        public float WorkingCroppedCursorLeftAsFloat
        {
            get => this.workingCroppedCursorPoint.X.AsFloat;
            set
            {
                if (this.workingCroppedCursorPoint.X.AsFloat != value)
                {
                    this.workingCroppedCursorPoint = new Models.Geometric.PointFloat(
                        x: new Models.Geometric.XFloat(value),
                        y: this.workingCroppedCursorPoint.Y);

                    OnPropertyChanged(nameof(WorkingCroppedCursorLeftAsFloat));
                    OnPropertyChanged(nameof(WorkingCroppedCursorPoint));
                    OnPropertyChanged(nameof(WorkingCroppedCursorPointAsMargin));

                    OnPropertyChanged(nameof(WorkingCroppedCursorLeftAsPresentableText));
                }
            }
        }

        /// <summary>
        ///     矩形カーソル。ズーム済みの位置ｙ
        ///         
        ///     <list type="bullet">
        ///         <item>カーソルの線の幅を含まない</item>
        ///     </list>
        /// </summary>
        public float WorkingCroppedCursorTopAsFloat
        {
            get => this.workingCroppedCursorPoint.Y.AsFloat;
            set
            {
                if (this.workingCroppedCursorPoint.Y.AsFloat != value)
                {
                    this.workingCroppedCursorPoint = new Models.Geometric.PointFloat(
                        x: this.workingCroppedCursorPoint.X,
                        y: new Models.Geometric.YFloat(value));

                    OnPropertyChanged(nameof(WorkingCroppedCursorTopAsFloat));
                    OnPropertyChanged(nameof(WorkingCroppedCursorPoint));
                    OnPropertyChanged(nameof(WorkingCroppedCursorPointAsMargin));

                    OnPropertyChanged(nameof(WorkingCroppedCursorTopAsPresentableText));
                }
            }
        }

        /// <summary>
        ///     矩形カーソル。ズーム済みのサイズ
        ///         
        ///     <list type="bullet">
        ///         <item>カーソルの線の幅を含まない</item>
        ///     </list>
        /// </summary>
        public Models.Geometric.SizeFloat WorkingCroppedCursorSize
        {
            get => this.workingCroppedCursorSize;
            set
            {
                if (this.workingCroppedCursorSize != value)
                {
                    this.WorkingCroppedCursorWidthAsFloat = value.Width.AsFloat;
                    this.WorkingCroppedCursorHeightAsFloat = value.Height.AsFloat;
                }
            }
        }

        /// <summary>
        ///     矩形カーソル。ズーム済みの横幅
        ///         
        ///     <list type="bullet">
        ///         <item>カーソルの線の幅を含まない</item>
        ///     </list>
        /// </summary>
        public float WorkingCroppedCursorWidthAsFloat
        {
            get => this.workingCroppedCursorSize.Width.AsFloat;
            set
            {
                if (this.workingCroppedCursorSize.Width.AsFloat != value)
                {
                    this.workingCroppedCursorSize = new Models.Geometric.SizeFloat(new Models.Geometric.WidthFloat(value), workingCroppedCursorSize.Height);
                    // Trace.WriteLine($"[TileCropPageViewModel.cs WorkingCroppedCursorWidthAsFloat] this.workingCroppedCursorSize: {this.workingCroppedCursorSize.Dump()}");

                    // キャンバスを再描画
                    // RefreshCanvasOfTileCursor(codePlace: "[TileCropPageViewModel WorkingCroppedCursorWidthAsFloat set]");

                    // キャンバスを再描画後に変更通知
                    OnPropertyChanged(nameof(WorkingCroppedCursorWidthAsFloat));
                    OnPropertyChanged(nameof(WorkingCroppedCursorSize));

                    OnPropertyChanged(nameof(WorkingCroppedCursorCanvasWidthAsFloat));
                    OnPropertyChanged(nameof(WorkingCroppedCursorWidthAsPresentableText));
                }
            }
        }

        /// <summary>
        ///     矩形カーソル。ズーム済みの縦幅
        ///         
        ///     <list type="bullet">
        ///         <item>カーソルの線の幅を含まない</item>
        ///     </list>
        /// </summary>
        public float WorkingCroppedCursorHeightAsFloat
        {
            get => this.workingCroppedCursorSize.Height.AsFloat;
            set
            {
                if (this.workingCroppedCursorSize.Height.AsFloat != value)
                {
                    this.workingCroppedCursorSize = new Models.Geometric.SizeFloat(this.workingCroppedCursorSize.Width, new Models.Geometric.HeightFloat(value));
                    // Trace.WriteLine($"[TileCropPageViewModel.cs WorkingCroppedCursorHeightAsFloat] this.workingCroppedCursorSize: {this.workingCroppedCursorSize.Dump()}");

                    // キャンバスを再描画
                    // RefreshCanvasOfTileCursor("[TileCropPageViewModel WorkingCroppedCursorHeightAsFloat set]");

                    // キャンバスを再描画後に変更通知
                    OnPropertyChanged(nameof(WorkingCroppedCursorHeightAsFloat));
                    OnPropertyChanged(nameof(WorkingCroppedCursorSize));

                    OnPropertyChanged(nameof(WorkingCroppedCursorCanvasHeightAsFloat));
                    OnPropertyChanged(nameof(WorkingCroppedCursorHeightAsPresentableText));
                }
            }
        }

        /// <summary>
        ///     切抜きカーソル。ズーム済みの横幅
        ///         
        ///     <list type="bullet">
        ///         <item>カーソルの線の幅を含む</item>
        ///         <item>切抜きカーソルは、対象範囲に外接する</item>
        ///     </list>
        /// </summary>
        public float WorkingCroppedCursorCanvasWidthAsFloat
        {
            get => this.workingCroppedCursorSize.Width.AsFloat + (4 * this.HalfThicknessOfTileCursorLine.AsInt);
        }

        /// <summary>
        ///     切抜きカーソル。ズーム済みの縦幅
        ///         
        ///     <list type="bullet">
        ///         <item>カーソルの線の幅を含む</item>
        ///         <item>切抜きカーソルは、対象範囲に外接する</item>
        ///     </list>
        /// </summary>
        public float WorkingCroppedCursorCanvasHeightAsFloat
        {
            get => this.workingCroppedCursorSize.Height.AsFloat + (4 * this.HalfThicknessOfTileCursorLine.AsInt);
        }

        /// <summary>
        ///     矩形カーソル。ズーム済みの位置ｘ
        ///         
        ///     <list type="bullet">
        ///         <item>カーソルの線の幅を含まない</item>
        ///         <item>表示用テキスト</item>
        ///         <item>📖 [Microsoft　＞　Standard numeric format strings](https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings?redirectedfrom=MSDN)  </item>
        ///     </list>
        /// </summary>
        public string WorkingCroppedCursorLeftAsPresentableText => this.workingCroppedCursorPoint.X.AsFloat.ToString("F1");

        /// <summary>
        ///     矩形カーソル。ズーム済みの位置ｙ
        ///         
        ///     <list type="bullet">
        ///         <item>カーソルの線の幅を含まない</item>
        ///         <item>表示用テキスト</item>
        ///     </list>
        /// </summary>
        public string WorkingCroppedCursorTopAsPresentableText => this.workingCroppedCursorPoint.Y.AsFloat.ToString("F1");

        /// <summary>
        ///     矩形カーソル。ズーム済みの横幅
        ///         
        ///     <list type="bullet">
        ///         <item>カーソルの線の幅を含まない</item>
        ///         <item>表示用テキスト</item>
        ///     </list>
        /// </summary>
        public string WorkingCroppedCursorWidthAsPresentableText => this.workingCroppedCursorSize.Width.AsFloat.ToString("F1");

        /// <summary>
        ///     矩形カーソル。ズーム済みの縦幅
        ///         
        ///     <list type="bullet">
        ///         <item>カーソルの線の幅を含まない</item>
        ///         <item>表示用テキスト</item>
        ///     </list>
        /// </summary>
        public string WorkingCroppedCursorHeightAsPresentableText => this.workingCroppedCursorSize.Height.AsFloat.ToString("F1");
        #endregion

        #region 変更通知プロパティ（登録タイル　関連）
        /// <summary>
        ///     登録タイルのタイトル
        /// </summary>
        public string SelectedTileTitleAsStr
        {
            get
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    return selectedTileVM.Title.AsStr;
                }
                else
                {
                    // タイル・カーソル無し時
                    return string.Empty;
                }
            }
            set
            {
                if (this.selectedTileVMOption.TryGetValue(out TileRecordViewModel selectedTileVM))
                {
                    if (selectedTileVM.Title.AsStr == value)
                    {
                        // 値に変化がない
                        return;
                    }

                    var rect1 = selectedTileVM.SourceRectangle;
                    selectedTileVMOption = new Option<TileRecordViewModel>(TileRecordViewModel.FromModel(
                        tileRecord: new Models.TileRecord(
                            id: selectedTileVM.Id,
                            rect: rect1,
                            title: new Models.TileTitle(value),
                            logicalDelete: selectedTileVM.LogicalDelete),
                        workingRect: rect1.Do(this.Zoom)));
                }
                else
                {
                    // タイル・カーソル無し時
                    var rect1 = Models.Geometric.RectangleInt.Empty;
                    selectedTileVMOption = new Option<TileRecordViewModel>(TileRecordViewModel.FromModel(
                        tileRecord: new Models.TileRecord(
                            id: TileId.Empty,
                            rect: rect1,
                            title: new Models.TileTitle(value),
                            logicalDelete: Models.LogicalDelete.False),
                       workingRect: rect1.Do(this.Zoom)));
                }

                OnPropertyChanged(nameof(SelectedTileTitleAsStr));
            }
        }
        #endregion

        // - パブリック・メソッド

        #region メソッド（ロケール変更による再描画）
        /// <summary>
        ///     ロケール変更による再描画
        ///     
        ///     <list type="bullet">
        ///         <item>動的にテキストを変えている部分に対応するため</item>
        ///     </list>
        /// </summary>
        public void InvalidateLocale()
        {
            this.InvalidateAddsButton();
        }
        #endregion

        #region メソッド（画面遷移でこの画面に戻ってきた時）
        /// <summary>
        ///     画面遷移でこの画面に戻ってきた時
        /// </summary>
        public void ReactOnVisited()
        {
            // ロケールが変わってるかもしれないので反映
            OnPropertyChanged(nameof(CultureInfoAsStr));

            // グリッド・キャンバス
            {
                // グリッドの左上位置（初期値）
                this.SourceGridPhase = new Models.Geometric.PointInt(new Models.Geometric.XInt(0), new Models.Geometric.YInt(0));

                // グリッドのタイルサイズ（初期値）
                this.SourceGridUnit = new Models.Geometric.SizeInt(new Models.Geometric.WidthInt(32), new Models.Geometric.HeightInt(32));

                // グリッド・キャンバス画像の再作成
                this.RemakeGridCanvasImage();
            }
        }
        #endregion

        // - インターナル・プロパティ

        #region プロパティ（切抜きカーソルと、既存タイルが交差しているか？）
        /// <summary>
        ///     切抜きカーソルと、既存タイルが交差しているか？
        /// </summary>
        /// <returns>そうだ</returns>
        internal bool HasIntersectionBetweenCroppedCursorAndRegisteredTile { get; private set; }
        #endregion

        #region プロパティ（切抜きカーソルと、既存タイルは合同か？）
        /// <summary>
        ///     切抜きカーソルと、既存タイルは合同か？
        /// </summary>
        /// <returns>そうだ</returns>
        internal bool IsCongruenceBetweenCroppedCursorAndRegisteredTile { get; private set; }
        #endregion

        // - インターナル・メソッド

        #region メソッド（切抜きカーソル。ズーム済み　関連）
        /// <summary>
        ///     <pre>
        ///         切抜きカーソル。ズーム済みのキャンバスの再描画
        /// 
        ///         TRICK:  GraphicsView を再描画させたいが、ビューモデルから要求する方法が分からない。
        ///                 そこで、内部的なグリッド画像の横幅が偶数のときは +1、奇数のときは -1 して
        ///                 振動させることで、再描画を呼び起こすことにする
        ///     </pre>
        /// </summary>
        internal void TrickRefreshCanvasOfTileCursor(string codePlace = "[TileCropPageViewModel RefreshCanvasOfTileCursor]")
        {
            int offset;

            if (((int)this.workingCroppedCursorSize.Width.AsFloat) % 2 == 1) // FIXME 浮動小数点型の剰余は無理がある
            {
                // Trace.WriteLine($"{codePlace} 幅 {this._tileCursorCanvasSize.Width.AsInt} から 1 引く");
                offset = -1;
            }
            else
            {
                // Trace.WriteLine($"{codePlace} 幅 {this._tileCursorCanvasSize.Width.AsInt} へ 1 足す");
                offset = 1;
            }

            // TODO 循環参照を避けるために、直接フィールドを変更
            this.WorkingCroppedCursorSize = new Models.Geometric.SizeFloat(
                width: new Models.Geometric.WidthFloat(this.workingCroppedCursorSize.Width.AsFloat + offset),
                height: new Models.Geometric.HeightFloat(this.workingCroppedCursorSize.Height.AsFloat));

            // TRICK CODE:
            OnPropertyChanged(nameof(WorkingCroppedCursorWidthAsFloat));
        }
        #endregion

        #region メソッド（タイルＩｄの再描画）
        /// <summary>
        ///     タイルＩｄの再描画
        /// </summary>
        internal void NotifyTileIdChange()
        {
            OnPropertyChanged(nameof(SelectedTileIdAsBASE64));
            OnPropertyChanged(nameof(SelectedTileIdAsPhoneticCode));
        }
        #endregion

        #region メソッド（作業中タイルセット画像の再描画）
        /// <summary>
        ///     作業中タイルセット画像の再描画
        /// </summary>
        internal void RefreshWorkingTilesetImage()
        {
            OnPropertyChanged(nameof(TilesetWorkingImageFilePathAsStr));
        }
        #endregion

        #region メソッド　＞　再描画　＞　切抜きカーソル
        /// <summary>
        ///     切抜きカーソルの再描画
        /// </summary>
        internal void InvalidateCroppedCursor()
        {
            if (this.TilesetSettingsVM.TryGetByRectangle(
    sourceRect: this.SourceCroppedCursorRect,
    out TileRecordViewModel? recordVMOrNull))
            {
                TileRecordViewModel recordVM = recordVMOrNull ?? throw new NullReferenceException(nameof(recordVMOrNull));
                // Trace.WriteLine($"[TileCropPage.xml.cs TapGestureRecognizer_Tapped] タイルは登録済みだ。 Id:{recordVM.Id.AsInt}, X:{recordVM.SourceRectangle.Location.X.AsInt}, Y:{recordVM.SourceRectangle.Location.Y.AsInt}, Width:{recordVM.SourceRectangle.Size.Width.AsInt}, Height:{recordVM.SourceRectangle.Size.Height.AsInt}, Title:{recordVM.Title.AsStr}");

                //
                // データ表示
                // ==========
                //

                // 選択中のタイルを設定
                this.SelectedTileVMOption = new Option<TileRecordViewModel>(recordVM);
            }
            else
            {
                // Trace.WriteLine("[TileCropPage.xml.cs TapGestureRecognizer_Tapped] 未登録のタイルだ");

                //
                // 空欄にする
                // ==========
                //

                // 選択中のタイルの矩形だけ維持し、タイル・コードと、コメントを空欄にする
                this.SelectedTileVMOption = new Option<TileRecordViewModel>(TileRecordViewModel.FromModel(
                    tileRecord: new Models.TileRecord(
                        id: Models.TileId.Empty,
                        rect: this.SourceCroppedCursorRect,
                        title: Models.TileTitle.Empty,
                        logicalDelete: Models.LogicalDelete.False),
                    workingRect: this.SourceCroppedCursorRect.Do(this.Zoom)));
            }
        }
        #endregion

        #region メソッド　＞　変更あり　＞　タイルセット設定ビューモデル
        /// <summary>
        ///     タイルセット設定ビューモデルに変更あり
        /// </summary>
        internal void InvalidateTilesetSettingsVM()
        {
            OnPropertyChanged(nameof(TilesetSettingsVM));
        }
        #endregion

        #region メソッド　＞　再描画　＞　［追加／上書き］ボタン
        /// <summary>
        ///     ［追加／上書き］ボタンの再描画
        /// </summary>
        internal void InvalidateAddsButton()
        {
            // マウスドラッグ中で、かつ、
            // this.IsMouseDragging && 

            // 切抜きカーソルが、登録済みタイルのいずれかと交差しているか？
            if (this.HasIntersectionBetweenCroppedCursorAndRegisteredTile)
            {
                // 合同のときは「交差中」とは表示しない
                if (!this.IsCongruenceBetweenCroppedCursorAndRegisteredTile)
                {
                    // 「交差中」
                    // Trace.WriteLine("[TileCropPage.xml.cs InvalidateAddsButton] 交差中だ");

                    this.AddsButtonText = (string)LocalizationResourceManager.Instance["Intersecting"];
                    this.AddsButtonIsEnabled = false;
                    return;
                }
            }

            if (this.selectedTileVMOption.TryGetValue(out var recordVM))
            {
                // 切抜きカーソル有り時

                if (recordVM.Id == TileId.Empty)
                {
                    // Ｉｄ未設定時

                    // ［追加」
                    this.AddsButtonText = (string)LocalizationResourceManager.Instance["Add"];
                }
                else
                {
                    // 「上書」
                    this.AddsButtonText = (string)LocalizationResourceManager.Instance["Overwrite"];
                }

                this.AddsButtonIsEnabled = true;
            }
            else
            {
                // 切抜きカーソル無し時

                // 「追加」
                this.AddsButtonText = (string)LocalizationResourceManager.Instance["Add"];
                this.AddsButtonIsEnabled = false;
            }
        }
        #endregion

        #region メソッド　＞　再描画　＞　［削除］ボタン
        /// <summary>
        ///     ［削除］ボタンの再描画
        /// </summary>
        internal void InvalidateDeletesButton()
        {
            if (this.selectedTileVMOption.TryGetValue(out var recordVM))
            {
                // 切抜きカーソル有り時

                if (recordVM.Id == TileId.Empty)
                {
                    // Ｉｄ未設定時
                    this.DeletesButtonIsEnabled = false;
                }
                else
                {
                    // タイル登録済み時
                    this.DeletesButtonIsEnabled = true;
                }
            }
            else
            {
                // 切抜きカーソル無し時
                this.DeletesButtonIsEnabled = false;
            }
        }
        #endregion

        #region メソッド（切抜きカーソルと、既存タイルが交差しているか？合同か？　を再計算）
        /// <summary>
        ///     切抜きカーソルと、既存タイルが交差しているか？合同か？　を再計算
        ///     
        ///     <list type="bullet">
        ///         <item>軽くはない処理</item>
        ///     </list>
        /// </summary>
        internal void RecalculateBetweenCroppedCursorAndRegisteredTile()
        {
            if (this.SourceCroppedCursorRect == TheGeometric.RectangleInt.Empty)
            {
                // カーソルが無ければ、交差も無い。合同ともしない
                this.HasIntersectionBetweenCroppedCursorAndRegisteredTile = false;
                this.IsCongruenceBetweenCroppedCursorAndRegisteredTile = false;
                return;
            }

            // 軽くはない処理
            this.HasIntersectionBetweenCroppedCursorAndRegisteredTile = this.TilesetSettingsVM.HasIntersection(this.SourceCroppedCursorRect);
            this.IsCongruenceBetweenCroppedCursorAndRegisteredTile = this.TilesetSettingsVM.IsCongruence(this.SourceCroppedCursorRect);
        }
        #endregion

        // - プライベート・フィールド

        #region フィールド（タイルセット設定　関連）
        /// <summary>
        ///     タイルセット設定のCSVファイル
        /// </summary>
        DataCsvTilesetCsv _tilesetSettingsFile = DataCsvTilesetCsv.Empty;

        /// <summary>
        ///     タイルセット設定ビューモデル
        /// </summary>
        TilesetSettingsViewModel _tilesetSettingsVM = new TilesetSettingsViewModel();
        #endregion

        #region フィールド（タイルセット元画像　関連）
        /// <summary>
        ///     タイルセット元画像ファイルへのパス
        /// </summary>
        ImagesTilesetPng tilesetImageFile = ImagesTilesetPng.Empty;

        /// <summary>
        ///     タイルセット元画像サイズ
        /// </summary>

        /* プロジェクト '2D RPG Negiramen (net7.0-windows10.0.19041.0)' からのマージされていない変更
        前:
                Models.SizeInt tilesetSourceImageSize = Models.SizeInt.Empty;
        後:
                SizeInt tilesetSourceImageSize = SizeInt.Empty;
        */
        Models.Geometric.SizeInt tilesetSourceImageSize = Models.Geometric.SizeInt.Empty;
        #endregion

        #region フィールド（タイルセット作業画像　関連）
        /// <summary>
        ///     タイルセット作業画像サイズ
        /// </summary>

        /* プロジェクト '2D RPG Negiramen (net7.0-windows10.0.19041.0)' からのマージされていない変更
        前:
                Models.SizeInt workingImageSize = Models.SizeInt.Empty;
        後:
                SizeInt workingImageSize = SizeInt.Empty;
        */
        Models.Geometric.SizeInt workingImageSize = Models.Geometric.SizeInt.Empty;
        #endregion

        #region フィールド（グリッド　関連）
        /// <summary>
        ///     グリッド・キャンバス画像サイズ
        /// </summary>
        Models.Geometric.SizeInt gridCanvasImageSize = Models.Geometric.SizeInt.Empty;

        /// <summary>
        ///     グリッド単位。元画像ベース
        /// </summary>
        Models.Geometric.SizeInt sourceGridUnit = new Models.Geometric.SizeInt(new Models.Geometric.WidthInt(32), new Models.Geometric.HeightInt(32));

        /// <summary>
        ///     グリッド位相の左上表示位置。ズーム後
        /// </summary>
        Models.Geometric.PointFloat workingGridPhase = Models.Geometric.PointFloat.Empty;

        /// <summary>
        ///     グリッド単位。ズーム後
        /// </summary>
        Models.Geometric.SizeFloat workingGridUnit = new Models.Geometric.SizeFloat(new Models.Geometric.WidthFloat(32.0f), new Models.Geometric.HeightFloat(32.0f));
        #endregion

        #region フィールド（矩形カーソル。元画像ベース　関連）
        /// <summary>
        ///     矩形カーソル。元画像ベース
        ///     
        ///     <list type="bullet">
        ///         <item>タイル・カーソルが有るときと、無いときを分ける</item>
        ///     </list>
        /// </summary>
        Option<TileRecordViewModel> selectedTileVMOption = new Option<TileRecordViewModel>(new TileRecordViewModel());
        #endregion

        #region フィールド（矩形カーソル。ズーム済み　関連）
        /// <summary>
        ///     矩形カーソル。ズーム済みの位置
        ///         
        ///     <list type="bullet">
        ///         <item>カーソルの線の幅を含まない</item>
        ///     </list>
        /// </summary>
        Models.Geometric.PointFloat workingCroppedCursorPoint = Models.Geometric.PointFloat.Empty;

        /// <summary>
        ///     矩形カーソル。ズーム済みのサイズ
        ///         
        ///     <list type="bullet">
        ///         <item>カーソルの線の幅は含まない</item>
        ///     </list>
        /// </summary>
        Models.Geometric.SizeFloat workingCroppedCursorSize = Models.Geometric.SizeFloat.Empty;
        #endregion

        #region フィールド（ズーム　関連）
        /// <summary>
        ///     ズーム最大
        /// </summary>
        Models.Geometric.Zoom zoomMax = new Models.Geometric.Zoom(4.0f);

        /// <summary>
        ///     ズーム最小
        /// </summary>
        Models.Geometric.Zoom zoomMin = new Models.Geometric.Zoom(0.5f);

        /// <summary>
        ///     ズーム
        /// </summary>
        Models.Geometric.Zoom zoom = Models.Geometric.Zoom.IdentityElement;
        #endregion

        // - プライベート・メソッド

        #region メソッド（作業タイルセット画像の再作成）
        /// <summary>
        ///     作業タイルセット画像の再作成
        /// </summary>
        void RemakeWorkingTilesetImage()
        {
            // 元画像をベースに、作業画像を複製
            var temporaryBitmap = SkiaSharp.SKBitmap.FromImage(SkiaSharp.SKImage.FromBitmap(this.TilesetSourceBitmap));

            // 画像処理（明度を下げる）
            FeatSkia.ReduceBrightness.DoItInPlace(temporaryBitmap);

            // 作業画像のサイズ計算
            this.workingImageSize = new Models.Geometric.SizeInt(
                width: new Models.Geometric.WidthInt((int)(this.ZoomAsFloat * this.TilesetSourceImageSize.Width.AsInt)),
                height: new Models.Geometric.HeightInt((int)(this.ZoomAsFloat * this.TilesetSourceImageSize.Height.AsInt)));

            // 作業画像のリサイズ
            this.TilesetWorkingBitmap = temporaryBitmap.Resize(
                size: new SKSizeI(
                    width: this.workingImageSize.Width.AsInt,
                    height: this.workingImageSize.Height.AsInt),
                quality: SKFilterQuality.Medium);

            OnPropertyChanged(nameof(TilesetWorkingImageWidthAsInt));
            OnPropertyChanged(nameof(TilesetWorkingImageHeightAsInt));
        }
        #endregion

        #region メソッド（グリッド・キャンバス　関連）
        /// <summary>
        ///     <pre>
        ///         グリッドのキャンバスの再描画
        /// 
        ///         TRICK:  GraphicsView を再描画させたいが、ビューモデルから要求する方法が分からない。
        ///                 そこで、内部的なグリッド画像の横幅が偶数のときは +1、奇数のときは -1 して
        ///                 振動させることで、再描画を呼び起こすことにする
        ///     </pre>
        /// </summary>
        void InvalidateCanvasOfGrid()
        {
            if (this.GridCanvasImageWidthAsInt % 2 == 1)
            {
                this.GridCanvasImageWidthAsInt--;
            }
            else
            {
                this.GridCanvasImageWidthAsInt++;
            }
        }

        /// <summary>
        ///     グリッド・キャンバス画像の再作成
        ///     
        ///     <list type="bullet">
        ///         <item>グリッドの線の太さを 2px と想定しているので、グリッドの線が画像の端っこで切れないように、グリッドの内部的キャンバス・サイズを 2px 広げる</item>
        ///     </list>
        /// </summary>
        void RemakeGridCanvasImage()
        {
            this.GridCanvasImageSize = new Models.Geometric.SizeInt(
                width: new Models.Geometric.WidthInt((int)(this.ZoomAsFloat * this.TilesetSourceImageSize.Width.AsInt) + (2 * this.HalfThicknessOfGridLineAsInt)),
                height: new Models.Geometric.HeightInt((int)(this.ZoomAsFloat * this.TilesetSourceImageSize.Height.AsInt) + (2 * this.HalfThicknessOfGridLineAsInt)));
        }
        #endregion

        #region メソッド（ズーム）
        void DoZoom()
        {
            // 拡大率
            double zoomNum = this.ZoomAsFloat;

            // 元画像の複製
            var copySourceMap = new SKBitmap();
            this.TilesetSourceBitmap.CopyTo(copySourceMap);

            // TODO 出力先画像（ズーム）
        }
        #endregion

        #region メソッド（作業グリッド・タイル横幅の再計算）
        /// <summary>
        ///     作業グリッド・タイル横幅の再計算
        /// </summary>
        void RefreshWorkingGridTileWidth()
        {
            this.WorkingGridTileWidthAsFloat = this.ZoomAsFloat * this.sourceGridUnit.Width.AsInt;

            OnPropertyChanged(nameof(WorkingGridTileWidthAsFloat));
            OnPropertyChanged(nameof(WorkingGridUnit));
        }
        #endregion

        #region メソッド（作業グリッド・タイル縦幅の再計算）
        /// <summary>
        ///     作業グリッド・タイル縦幅の再計算
        /// </summary>
        void RefreshWorkingGridTileHeight()
        {
            this.WorkingGridTileHeightAsFloat = this.ZoomAsFloat * this.sourceGridUnit.Height.AsInt;

            OnPropertyChanged(nameof(WorkingGridTileHeightAsFloat));
            OnPropertyChanged(nameof(WorkingGridUnit));
        }
        #endregion
    }
}
