using ClosedXML.Excel;
using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;
using ZeikomiRetweetWordCloud.Common;
using ZeikomiRetweetWordCloud.Models;
using ZeikomiRetweetWordCloud.Models.SQLite;
using ZeikomiRetweetWordCloud.Views;

namespace ZeikomiRetweetWordCloud.ViewModes
{
    public class MainWindowVM : ViewModelBase
    {
        /// <summary>
        /// 自動検索用タイマー
        /// </summary>
        private DispatcherTimer _SearchTimer;

        /// <summary>
        /// 自動ワードクラウド生成用タイマー
        /// </summary>
        private DispatcherTimer _CreateWordcloudTimer;

        #region 検索キーワード[SearchKeyword]プロパティ
        /// <summary>
        /// 検索キーワード[SearchKeyword]プロパティ用変数
        /// </summary>
        string _SearchKeyword = "#ワードクラウドでよろしく";
        /// <summary>
        /// 検索キーワード[SearchKeyword]プロパティ
        /// </summary>
        public string SearchKeyword
        {
            get
            {
                return _SearchKeyword;
            }
            set
            {
                if (_SearchKeyword == null || !_SearchKeyword.Equals(value))
                {
                    _SearchKeyword = value;
                    NotifyPropertyChanged("SearchKeyword");
                }
            }
        }
        #endregion

        #region メッセージ[Message]プロパティ
        /// <summary>
        /// メッセージ[Message]プロパティ用変数
        /// </summary>
        string _Message = string.Empty;
        /// <summary>
        /// メッセージ[Message]プロパティ
        /// </summary>
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                if (_Message == null || !_Message.Equals(value))
                {
                    _Message = value;
                    NotifyPropertyChanged("Message");
                }
            }
        }
        #endregion

        #region 共通変数
        /// <summary>
        /// 共通変数
        /// </summary>
        public GBLValues CommonValues
        {
            get
            {
                return GBLValues.GetInstance();
            }
        }
        #endregion

        #region 取得しているツイートId最大値の取得処理
        /// <summary>
        /// 取得しているツイートId最大値の取得処理
        /// </summary>
        /// <returns>ツイートId最大値</returns>
        private string GetMaxId()
        {
            if (File.Exists(SQLiteDataContext.SQLitePath))
            {
                // 最新のツイートIDを取得
                var tmp = target_tweetBase.Select()?.Max(x => x.id);
                // 起動時に最新のIDをチェック
                return string.IsNullOrEmpty(tmp) ? "0" : tmp;
            }
            else
            {
                return "0";
            }
        }
        #endregion

        #region 初期化処理
        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void Init(object sender, EventArgs e)
        {
            try
            {
                // タイマのインスタンスを生成
                _SearchTimer = new DispatcherTimer(); // 優先度はDispatcherPriority.Background

                // インターバルを設定
                _SearchTimer.Interval = new TimeSpan(0, 10, 0);

                // タイマメソッドを設定
                _SearchTimer.Tick -= new EventHandler(AutoSearchThread!);
                _SearchTimer.Tick += new EventHandler(AutoSearchThread!);


                // タイマのインスタンスを生成
                _CreateWordcloudTimer = new DispatcherTimer(); // 優先度はDispatcherPriority.Background

                // インターバルを設定
                _CreateWordcloudTimer.Interval = new TimeSpan(0, 15, 0);

                // タイマメソッドを設定
                _CreateWordcloudTimer.Tick -= new EventHandler(AutoCreateWordCloudThread!);
                _CreateWordcloudTimer.Tick += new EventHandler(AutoCreateWordCloudThread!);


                // 設定ファイルの読み込み
                this.CommonValues.TwitterAPIConfig.LoadXML();
            }
            catch (Exception ex)
            {
                ShowMessage.ShowErrorOK(ex.Message, "Error");
            }
        }
        #endregion

        #region 設定画面を開く処理
        /// <summary>
        /// 設定画面を開く処理
        /// </summary>
        public void OpenSetting()
        {
            try
            {
                var wm = new SettingV();

                if (wm.ShowDialog() == true)
                {


                }

                // 設定ファイルの読み込み
                this.CommonValues.TwitterAPIConfig.LoadXML();
            }
            catch (Exception ex)
            {
                ShowMessage.ShowErrorOK(ex.Message, "Error");
            }
        }
        #endregion

        #region 自動検索タイマーの開始
        /// <summary>
        /// 自動検索タイマーの開始
        /// </summary>
        public void StartAutoSearchTimer()
        {
            // タイマを開始
            _SearchTimer.Start();
        }
        #endregion

        #region 自動検索タイマーの停止
        /// <summary>
        /// 自動検索タイマーの停止
        /// </summary>
        public void StopAutoSearchTimer()
        {
            // タイマを停止
            _SearchTimer.Stop();
        }
        #endregion

        #region 自動ワードクラウド生成の開始
        /// <summary>
        /// 自動ワードクラウド生成の開始
        /// </summary>
        public void StartAutoCreateWordCloudTimer()
        {

            // タイマを開始
            _CreateWordcloudTimer.Start();
        }
        #endregion

        #region 自動ワードクラウド生成の停止
        /// <summary>
        /// 自動ワードクラウド生成の停止
        /// </summary>
        public void StopAutoCreateWordCloudTimer()
        {
            // タイマを停止
            _CreateWordcloudTimer.Stop();
        }
        #endregion

        #region 自動検索処理
        /// <summary>
        /// 自動検索処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoSearchThread(object sender, EventArgs e)
        {
            Search();
        }
        #endregion

        #region 自動ワードクラウド生成処理
        /// <summary>
        /// 自動検索処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoCreateWordCloudThread(object sender, EventArgs e)
        {
            CreateWordCloud();
        }
        #endregion

        #region 検索処理
        /// <summary>
        /// 検索処理
        /// </summary>
        public void Search()
        {
            try
            {
                string since_id = GetMaxId();
                WordCloudManagerM.Search(this.SearchKeyword, since_id);
            }
            catch (Exception ex)
            {
                ShowMessage.ShowErrorOK(ex.Message, "Error");
            }
        }
        #endregion

        #region WordCloudの作成処理
        /// <summary>
        /// WordCloudの作成処理
        /// </summary>
        public void CreateWordCloud()
        {
            try
            {
                // WordCloudを作成していないリストを取得
                var list = target_tweetBase.Select().Where(x => x.wordcloud_status == 0).ToList();
                string work_dir = "tmp";

                // リストの分WordCloudを作成する
                foreach (var item in list)
                {
                    string font_path = string.Empty;
                    string color_map = string.Empty;
                    // ツイートID指定してWordCloudを作成する
                    string? msg = WordCloudManagerM.CreateWordcloud(work_dir, "all", item.id, SQLiteDataContext.SQLitePath, @"Common\font\", 1, out font_path, out color_map);

                    string search_key = string.Empty;
                    if (!string.IsNullOrEmpty(msg))
                    {
                        var msg_list = msg?.Split(" ");

                        foreach (var wordcloud_msg in msg_list!)
                        {
                            if (wordcloud_msg.Contains("search_keyword"))
                            {
                                search_key = wordcloud_msg.Replace("search_keyword=", "");
                                break;
                            }
                        }
                    }

                    try
                    {
                        if(search_key.Equals("ゴメンナサイ"))
                        {
                            // 引用ツイート
                            QuotedTweetM.QuotedTweet(item.username, item.id, "ゴメンナサイ。うまく名詞を見つけられませんでした。\r\nキーワードを変えるかダブルクォートんで頂けると・・・", Path.Combine(work_dir, item.id + ".png"));
                        }
                        else
                        {
                            // 引用ツイート
                            QuotedTweetM.QuotedTweet(item.username, item.id, "ワードクラウドで作成しました。対象キーワード[" + search_key + "]", Path.Combine(work_dir, item.id + ".png"));
                        }
                        var chg_value = new target_tweetBase();
                        chg_value.Copy(item);
                        chg_value.wordcloud_status = 1;
                        chg_value.colormap = color_map;
                        chg_value.font = System.IO.Path.GetFileNameWithoutExtension(font_path);
                        // データの更新
                        target_tweetBase.Update(item, chg_value);
                    }
                    catch
                    {
                        QuotedTweetM.QuotedTweetNoMedia(item.username, item.id, "ごめんなさい。何等かの理由で失敗したみたいです。\r\n調査データに使わせてもらいます。ありがとうございます。");
                        var chg_value = new target_tweetBase();
                        chg_value.Copy(item);
                        chg_value.wordcloud_status = -1;
                        // データの更新
                        target_tweetBase.Update(item, chg_value);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage.ShowErrorOK(ex.Message, "Error");
            }
        }
        #endregion

        #region クローズ処理
        /// <summary>
        /// クローズ処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void Close(object sender, EventArgs e)
        {
            // 自動検索タイマーを停止
            StopAutoSearchTimer();

            // ワードクラウド自動生成タイマーを停止
            StopAutoCreateWordCloudTimer();
        }
        #endregion

    }
}
