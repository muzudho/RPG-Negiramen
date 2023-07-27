﻿namespace _2D_RPG_Negiramen.Views;

using _2D_RPG_Negiramen.Models;
using _2D_RPG_Negiramen.Models.FileEntries;
using _2D_RPG_Negiramen.ViewModels;
using SkiaSharp;
using System.Diagnostics;
using System.Threading.Tasks;

/// <summary>
///     😁 タイルセット一覧ページ
/// </summary>
public partial class TilesetListPage : ContentPage
{
    // - その他

    #region その他（生成）
    /// <summary>
    ///     生成
    /// </summary>
    public TilesetListPage()
    {
        // TODO ここで横幅を取得する方法が分からない
        //Trace.WriteLine($"[TilesetListPage.xaml.cs TilesetListPage] this.WidthRequest: {this.WidthRequest}, this.MaximumWidthRequest: {this.MaximumWidthRequest}, this.MinimumWidthRequest: {this.MinimumWidthRequest}, this.Width: {this.Width}");

        //Window window = this.GetParentWindow();

        //Trace.WriteLine($"[TilesetListPage.xaml.cs TilesetListPage] window.Width: {window.Width}, window.MaximumWidth: {window.MaximumWidth}, this.MinimumWidth: {window.MinimumWidth}");

        //// セル・サイズ（固定幅）
        double cellWidth = 128.0f;
        int cellColumns = (int)(App.WidthForCollectionView / cellWidth);
        // int cellColumns = (int)(this.Width / cellWidth);
        // int cellColumns = Random.Shared.Next(5, 8);
        // int cellColumns = 4;

        this.BindingContext = new TilesetListPageViewModel(
            itemsLayout: new GridItemsLayout(
                span: cellColumns,
                orientation: ItemsLayoutOrientation.Vertical));

        InitializeComponent();
    }
    #endregion

    // - パブリック・プロパティ

    #region プロパティ（ビューモデル）
    /// <summary>
    ///     ビューモデル
    /// </summary>
    public ITilesetListPageViewModel TilesetListPageVM => (ITilesetListPageViewModel)this.BindingContext;
    #endregion

    // - プライベート・イベントハンドラ

    #region イベントハンドラ（ページ読込完了時）
    /// <summary>
    ///     ページ読込完了時
    /// </summary>
    /// <param name="sender">このイベントを送っているコントロール</param>
    /// <param name="e">イベント</param>
    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        //
        // ビューモデルの取得
        // ==================
        //
        TilesetListPageViewModel context = (TilesetListPageViewModel)this.BindingContext;

        {
            TilesetListPage page = (TilesetListPage)sender;

            // セル・サイズ（固定幅）
            double cellWidth = 128.0f;
            int cellColumns = (int)(page.Width / cellWidth);

            context.ItemsLayout = new GridItemsLayout(cellColumns, ItemsLayoutOrientation.Vertical);
        }

        //
        // ユーザー設定の読込
        // ==================
        //
        UserConfiguration userConfiguration = App.GetOrLoadUserConfiguration();

        // タイルセット画像が入っているフォルダを取得
        var tilesetFolder = App.GetOrLoadConfiguration().UnityAssetsFolder.YourCircleNameFolder.YourWorkNameFolder.AutoGeneratedFolder.ImagesFolder.TilesetsFolder;

        List<Task> taskList = new List<Task>();

        // フォルダの中の PNG画像ファイルを一覧
        foreach (var originalPngPathAsStr in System.IO.Directory.GetFiles(tilesetFolder.Path.AsStr, "*.png"))
        {
            Trace.WriteLine($"[TilesetListPage.xaml.cs ContentPage_Loaded] path: [{originalPngPathAsStr}]");

            // 画像ファイルの名前は UUID という想定
            var uuid = System.IO.Path.GetFileNameWithoutExtension(originalPngPathAsStr);

            // TODO TOML があれば読込む。無ければ新規作成
            string tomlPathAsStr = System.IO.Path.Join(
                System.IO.Path.GetDirectoryName(originalPngPathAsStr),
                $"{uuid}.toml");
            if (System.IO.File.Exists(tomlPathAsStr))
            {
                // TODO TOML 読込
            }
            else
            {
                // TODO TOML 新規作成
            }

            //
            // TODO 画像ファイルを縮小して（サムネイル画像を作り）、キャッシュ・フォルダーへコピーしたい
            //
            var task = Task.Run(async () =>
            {
                try
                {
                    // 出力先ディレクトリーが無ければ作成する
                    var outputFolder = App.CacheFolder.YourCircleNameFolder.YourWorkNameFolder.ImagesFolder.TilesetFolder.ImagesTilesetsThumbnailsFolder;
                    outputFolder.CreateThisDirectoryIfItDoesNotExist();

                    // サムネイル画像のファイルパス
                    string thumbnailPathAsStr;

                    // サイズ
                    int originalWidth;
                    int originalHeight;
                    int thumbnailWidth;
                    int thumbnailHeight;

                    // タイルセット画像読込
                    using (Stream inputFileStream = System.IO.File.OpenRead(originalPngPathAsStr))
                    {
                        // ↓ １つのストリームが使えるのは、１回切り
                        using (MemoryStream memStream = new MemoryStream())
                        {
                            await inputFileStream.CopyToAsync(memStream);
                            memStream.Seek(0, SeekOrigin.Begin);

                            // 元画像
                            var bitmap = SkiaSharp.SKBitmap.Decode(memStream);

                            originalWidth = bitmap.Width;
                            originalHeight = bitmap.Height;
                            int longLength = Math.Max(originalWidth, originalHeight);
                            int shortLength = Math.Min(originalWidth, originalHeight);
                            // 長い方が 128 より大きければ縮める
                            if (128 < longLength)
                            {
                                float rate = (float)longLength / 128.0f;
                                thumbnailWidth = (int)(originalWidth / rate);
                                thumbnailHeight = (int)(originalHeight / rate);
                            }
                            else
                            {
                                thumbnailWidth = originalWidth;
                                thumbnailHeight = originalHeight;
                            }

                            // 作業画像のリサイズ
                            bitmap = bitmap.Resize(
                                size: new SKSizeI(
                                    width: thumbnailWidth,
                                    height: thumbnailHeight),
                                quality: SKFilterQuality.Medium);

                            //
                            // 書出先（ウィンドウズ・ローカルＰＣ）
                            //
                            // 📖 [Using SkiaSharp, how to save a SKBitmap ?](https://social.msdn.microsoft.com/Forums/en-US/25fe8438-8afb-4acf-9d68-09acc6846918/using-skiasharp-how-to-save-a-skbitmap-?forum=xamarinforms)  
                            //
                            var fileStem = System.IO.Path.GetFileNameWithoutExtension(originalPngPathAsStr);
                            thumbnailPathAsStr = outputFolder.CreateTilesetThumbnailPng(fileStem).Path.AsStr;
                            using (Stream outputFileStream = System.IO.File.Open(
                                path: thumbnailPathAsStr,
                                mode: FileMode.OpenOrCreate))
                            {
                                // 画像にする
                                SKImage skImage = SkiaSharp.SKImage.FromBitmap(bitmap);

                                // PNG画像にする
                                SKData pngImage = skImage.Encode(SKEncodedImageFormat.Png, 100);

                                // 出力
                                pngImage.SaveTo(outputFileStream);
                            }
                        };
                    }

                    context.EnqueueTilesetRecordVM(new TilesetRecordViewModel(
                            uuidAsStr: uuid,
                            filePathAsStr: originalPngPathAsStr,
                            widthAsInt: originalWidth,
                            heightAsInt: originalHeight,
                            thumbnailFilePathAsStr: thumbnailPathAsStr,
                            thumbnailWidthAsInt: thumbnailWidth,
                            thumbnailHeightAsInt: thumbnailHeight,
                            title: "たいとる１"));


                }
                catch (Exception ex)
                {
                    // TODO エラー対応どうする？
                    Trace.WriteLine(ex);
                }
            });

            taskList.Add(task);
        }

        Task.WaitAll(taskList.ToArray());
    }
    #endregion

    #region イベントハンドラ（［ホーム］ボタン・クリック時）
    /// <summary>
    ///     ［ホーム］ボタン・クリック時
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    async void HomeBtn_Clicked(object sender, EventArgs e)
    {
        await PolicyOfView.ReactOnPushed((Button)sender);

        await Shell.Current.GoToAsync("//MainPage");
    }
    #endregion

    #region イベントハンドラ（ボタンにマウスカーソル進入時）
    /// <summary>
    ///     ボタンにマウスカーソル進入時
    /// </summary>
    /// <param name="sender">このイベントを呼び出したコントロール</param>
    /// <param name="e">この発生イベントの制御変数</param>
    void Button_PointerGestureRecognizer_PointerEntered(object sender, PointerEventArgs e)
    {
        PolicyOfView.ReactOnMouseEntered((Button)sender);
    }
    #endregion

    #region イベントハンドラ（ボタンからマウスカーソル退出時）
    /// <summary>
    ///     ボタンからマウスカーソル退出時
    /// </summary>
    /// <param name="sender">このイベントを呼び出したコントロール</param>
    /// <param name="e">この発生イベントの制御変数</param>
    void Button_PointerGestureRecognizer_PointerExited(object sender, PointerEventArgs e)
    {
        PolicyOfView.ReactOnMouseLeaved((Button)sender);
    }
    #endregion

    #region イベントハンドラ（ロケール変更時）
    /// <summary>
    ///     ロケール変更時
    /// </summary>
    /// <param name="sender">このイベントを呼び出したコントロール</param>
    /// <param name="e">この発生イベントの制御変数</param>
    void LocalePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        // ＸＡＭＬではなく、Ｃ＃で動的に翻訳を行っている場合のための変更通知
        var context = this.TilesetListPageVM;
        context.InvalidateLocale();
    }
    #endregion

    private void ContentPage_SizeChanged(object sender, EventArgs e)
    {
        //ITilesetListPageViewModel context = (ITilesetListPageViewModel)this.BindingContext;

        //TilesetListPage page = (TilesetListPage)sender;


        //// セル・サイズ（固定幅）
        //double cellWidth = 128.0f;
        //// double cellHeight = 148.0f;

        //int cellColumns = (int)(page.Width / cellWidth);
        //// int cellRows = (int)(page.Height / cellHeight);

        //// Trace.WriteLine($"コンテント・ページ・サイズ変更 sender: {sender.GetType().FullName} cellColumns: {cellColumns}, Width: {page.Width}, Height: {page.Height}, WidthRequest: {page.WidthRequest}, HeightRequest: {page.HeightRequest}");

        //// this.TilesetListPageVM.ItemsLayout = new GridItemsLayout(cellColumns, ItemsLayoutOrientation.Vertical);
    }

    private void CollectionView_SizeChanged(object sender, EventArgs e)
    {
        ITilesetListPageViewModel context = (ITilesetListPageViewModel)this.BindingContext;

        CollectionView view = (CollectionView)sender;

        // セル・サイズ（固定幅）
        double cellWidth = 128.0f;
        int cellColumns = (int)(view.Width / cellWidth);

        context.ItemsLayout = new GridItemsLayout(cellColumns, ItemsLayoutOrientation.Vertical);

        //GridItemsLayout layout = (GridItemsLayout)view.ItemsLayout;
        //layout.Span = cellColumns;
        //view.ItemsLayout = null;
        //view.ItemsLayout = layout;
        //// view.ItemsLayout = new GridItemsLayout(cellColumns, ItemsLayoutOrientation.Vertical);
        //// view.Opacity = view.Opacity - 0.01d;
        //view.RotateTo(5.0, length:10000);

        //Trace.WriteLine($"コレクション・ビュー・サイズ変更 sender: {sender.GetType().FullName}, cellColumns: {cellColumns}, Width: {view.Width}, Height: {view.Height} layout.Span: {layout.Span}");
    }
}