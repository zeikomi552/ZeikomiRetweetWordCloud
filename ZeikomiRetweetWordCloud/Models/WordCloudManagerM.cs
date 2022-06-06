using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeikomiRetweetWordCloud.Common;

namespace ZeikomiRetweetWordCloud.Models
{
    public class WordCloudManagerM : ModelBase
    {
        #region 共通変数
        /// <summary>
        /// 共通変数
        /// </summary>
        public static GBLValues CommonValues
        {
            get
            {
                return GBLValues.GetInstance();
            }
        }
        #endregion

        static Random _Rand = new Random();

        #region ランダムにフォントを選択肢してフォントのファイルパスを返却する
        /// <summary>
        /// ランダムにフォントを選択肢してフォントのファイルパスを返却する
        /// </summary>
        /// <param name="font_dir">フォントファイルが保存されているフォルダ</param>
        /// <returns>フォントファイルパス</returns>
        private static string GetRandomFontPath(string font_dir)
        {
            // 指定ディレクトリ配下の.ttfファイルを全て取得する
            string[] files = System.IO.Directory.GetFiles(font_dir, "*.ttf", System.IO.SearchOption.TopDirectoryOnly);

            // .ttfファイルが存在する場合
            if (files.Length > 0)
            {
                // ランダムに一つ選んで返却
                int index = _Rand.Next(0, files.Length-1);
                return files[index];
            }
            else
            {
                return string.Empty;
            }

        }
        #endregion


        #region キーワード検索メイン処理
        /// <summary>
        /// キーワード検索メイン処理
        /// </summary>
        /// <param name="keyword">キーワード</param>
        public static void Search(string keyword, string since_id)
        {
            var config = CommonValues.TwitterAPIConfig.Item;

            string command = CommonValues.Gettweet2PythonPath + string.Format(" {0} {1} {2} {3} {4} {5} {6}",
                    "\"" + config.BearerToken + "\"",   // トークン
                    "\"" + @"tmp" + "\"",               // 出力先ディレクトリ
                    "\"" + keyword + "\"",   // 検索クエリ
                    "\"" + "all" + "\"",                 // 言語
                    1,                                 // 最大実行回数
                    "\"" + @"tmp\tmp.db" + "\"",
                    "\"" + since_id + "\"");

            var myProcess = new Process
            {
                StartInfo = new ProcessStartInfo(config.PythonPath)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    Arguments = command
                }
            };

            myProcess.Start();
            StreamReader myStreamReader = myProcess.StandardOutput;

            string? myString = myStreamReader.ReadLine();
            myProcess.WaitForExit();
            myProcess.Close();
        }
        #endregion

        #region 対象IDのワードクラウド作成処理
        /// <summary>
        /// 対象ツイートのワードクラウド作成処理
        /// </summary>
        /// <param name="output_dir">出力先ディレクトリ</param>
        /// <param name="lang">検索対象言語</param>
        /// <param name="id">ツイートID</param>
        /// <param name="sqlite_path">SQLiteのファイルパス</param>
        /// <param name="font_dir">フォントのディレクトリパス</param>
        /// <param name="colormap">カラーマップ</param>
        /// <returns>実行結果</returns>
        public static string? CreateWordcloud(string output_dir, string lang, string id, string sqlite_path, string font_dir, int max_count, out string font_path, out string colormap)
        {
            var config = CommonValues.TwitterAPIConfig.Item;

            // フォントをランダムに取得
            font_path = GetRandomFontPath(font_dir);

            // カラーマップをランダムに取得
            var colormap_name_list = Enum.GetNames(typeof(enumColormap));
            colormap = colormap_name_list[_Rand.Next(0, colormap_name_list.Length)];

            // カラーマップ作成実行用コマンド
            string command = CommonValues.Gettweet3PythonPath + string.Format(" {0} {1} {2} {3} {4} {5} {6} {7}",
                    "\"" + config.BearerToken + "\"",       // トークン
                    "\"" + output_dir + "\"",                   // 出力先ディレクトリ
                    "\"" + id + "\"",    // 対象ID
                    "\"" + lang + "\"",                     // 言語
                    max_count,                                      // 最大実行回数
                    "\"" + sqlite_path + "\"",            //SQLiteパス
                    "\"" + font_path + "\"",            // フォントパス
                    "\"" + colormap + "\"");          // フォントパス

            // プロセス呼び出し
            var myProcess = new Process
            {
                StartInfo = new ProcessStartInfo(config.PythonPath)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    Arguments = command
                }
            };

            // 実行
            myProcess.Start();
            StreamReader myStreamReader = myProcess.StandardOutput;
            string? myString = myStreamReader.ReadLine();
            Console.WriteLine(myString);
            myProcess.WaitForExit();
            myProcess.Close();

            // 実行結果
            return myString;
        }
        #endregion
    }
}
