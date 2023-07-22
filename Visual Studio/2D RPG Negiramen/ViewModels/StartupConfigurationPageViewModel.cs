﻿namespace _2D_RPG_Negiramen.ViewModels
{
    using _2D_RPG_Negiramen.Models;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using System.Windows.Input;

    /// <summary>
    ///     😁 ［初期設定］ページ用のビューモデル
    /// </summary>
    class StartupConfigurationPageViewModel : ObservableObject
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
        public StartupConfigurationPageViewModel()
        {
            // 構成ファイル取得
            var configuration = App.GetOrLoadConfiguration();

            NegiramenWorkspaceFolderPathAsStr = configuration.NegiramenWorkspaceFolder.Path.AsStr;
            UnityAssetsFolderPathAsStr = configuration.UnityAssetsFolder.Path.AsStr;
            YourCircleNameAsStr = configuration.YourCircleName.AsStr;
            YourWorkNameAsStr = configuration.YourWorkName.AsStr;

            // Unity の Assets フォルダ―へ初期設定をコピーするコマンド
            PushStartupToUnityAssetsFolderCommand = new AsyncRelayCommand(PushStartupToUnityAssetsFolder);
        }
        #endregion

        // - パブリック・プロパティ

        #region プロパティ（Unity の Assets フォルダ―へ初期設定をコピーするコマンド）
        /// <summary>
        ///     Unity の Assets フォルダ―へ初期設定をコピーするコマンド
        /// </summary>
        public ICommand PushStartupToUnityAssetsFolderCommand { get; }
        #endregion

        // - パブリック変更通知プロパティ

        #region 変更通知プロパティ（ネギラーメン・ワークスペース・フォルダーへのパス。文字列形式）
        /// <summary>
        ///     ネギラーメン・ワークスペース・フォルダーへのパス。文字列形式
        /// </summary>
        public string NegiramenWorkspaceFolderPathAsStr
        {
            get => _negiramenWorkspaceFolder.Path.AsStr;
            set
            {
                if (_negiramenWorkspaceFolder.Path.AsStr != value)
                {
                    _negiramenWorkspaceFolder = new Models.FileEntries.Locations.Negiramen.WorkspaceFolder(
                        pathSource: FileEntryPathSource.FromString(value),
                        convert: (pathSource) => FileEntryPath.From(pathSource,
                                                                    replaceSeparators: true));
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region 変更通知プロパティ（Unity の Assets フォルダーへのパス。文字列形式）
        /// <summary>
        ///     Unity の Assets フォルダーへのパス。文字列形式
        /// </summary>
        /// <example>"C:/Users/むずでょ/Documents/Unity Projects/Negiramen Practice/Assets"</example>
        public string UnityAssetsFolderPathAsStr
        {
            get => _unityAssetsFolder.Path.AsStr;
            set
            {
                if (_unityAssetsFolder.Path.AsStr == value)
                    return;

                _unityAssetsFolder = new Models.FileEntries.Locations.UnityAssetsFolder(
                    pathSource: FileEntryPathSource.FromString(value),
                    convert: (pathSource) => FileEntryPath.From(pathSource,
                                                                replaceSeparators: true));
                OnPropertyChanged();
            }
        }
        #endregion

        #region 変更通知プロパティ（あなたのサークル名）
        /// <summary>
        ///     あなたのサークル名
        /// </summary>
        public string YourCircleNameAsStr
        {
            get => _yourCircleName.AsStr;
            set
            {
                if (_yourCircleName.AsStr == value)
                    return;

                _yourCircleName = Models.YourCircleName.FromString(value);
                OnPropertyChanged();
            }
        }
        #endregion

        #region 変更通知プロパティ（あなたの作品名）
        /// <summary>
        ///     あなたの作品名
        /// </summary>
        public string YourWorkNameAsStr
        {
            get => _yourWorkName.AsStr;
            set
            {
                if (_yourWorkName.AsStr == value)
                    return;

                _yourWorkName = Models.YourWorkName.FromString(value);
                OnPropertyChanged();
            }
        }
        #endregion

        // - プライベート・フィールド

        /// <summary>
        ///     ネギラーメンの 📂 `Workspace` フォルダーへのパス
        /// </summary>
        private Models.FileEntries.Locations.Negiramen.WorkspaceFolder _negiramenWorkspaceFolder = Models.FileEntries.Locations.Negiramen.WorkspaceFolder.Empty;

        /// <summary>
        ///     Unity の Assets フォルダーへのパス
        /// </summary>
        private Models.FileEntries.Locations.UnityAssetsFolder _unityAssetsFolder = Models.FileEntries.Locations.UnityAssetsFolder.Empty;

        /// <summary>
        ///     あなたのサークル名
        /// </summary>
        private YourCircleName _yourCircleName = YourCircleName.Empty;

        /// <summary>
        ///     あなたの作品名
        /// </summary>
        private YourWorkName _yourWorkName = YourWorkName.Empty;

        // - プライベート・メソッド

        #region メソッド（［Unity の Assets フォルダ―へ初期設定をコピーする］コマンドを実行）
        /// <summary>
        ///     ［Unity の Assets フォルダ―へ初期設定をコピーする］コマンドを実行
        /// </summary>
        /// <returns>なし</returns>
        async Task PushStartupToUnityAssetsFolder()
        {
            await Task.Run(() =>
            {
                // テキスト・ボックスから、Unity エディターの Assets フォルダーへのパスを取得
                var assetsFolderPathAsStr = this.UnityAssetsFolderPathAsStr;

                // 構成ファイルの更新差分
                var configurationDifference = new Models.FileEntries.ConfigurationBuffer()
                {
                    NegiramenWorkspaceFolder = this._negiramenWorkspaceFolder,
                    UnityAssetsFolder = this._unityAssetsFolder,
                    YourCircleName = _yourCircleName,
                    YourWorkName = _yourWorkName,
                };

                // 設定ファイルの保存
                if (Models.FileEntries.Configuration.SaveTOML(App.GetOrLoadConfiguration(), configurationDifference, out Models.FileEntries.Configuration newConfiguration))
                {
                    // グローバル変数を更新
                    App.SetConfiguration(newConfiguration);

                    // ネギラーメンのワークスペース・フォルダーの内容を確認
                    var isOk = Models.FileEntries.NegiramenWorkspaceDeployment.CheckForUnityAssets();
                    if (!isOk)
                    {
                        // TODO 異常時の処理
                        return;
                    }

                    // Unity の Assets フォルダ―へ初期設定をコピー
                    if (!Models.FileEntries.UnityAssetsDeployment.PushStartupMemberToUnityAssetsFolder(assetsFolderPathAsStr))
                    {
                        // TODO 異常時の処理
                        return;
                    }
                }
            });

            // 画面遷移、戻る
            await Shell.Current.GoToAsync("..");

            // 履歴は戻しておく
            var shellNavigationState = App.NextPage.Pop();

            // 全ての入力が準備できているなら、画面遷移する
            var newConfiguration = App.GetOrLoadConfiguration();
            if (newConfiguration.IsReady())
            {
                await Shell.Current.GoToAsync(shellNavigationState);
            }
        }
        #endregion
    }
}
