using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeikomiRetweetWordCloud.Models;

namespace ZeikomiRetweetWordCloud.Common
{
    public class GBLValues : ModelBase
    {
        private GBLValues()
        {

        }

        private static GBLValues _SingleInstance = new();

        #region インスタンス
        /// <summary>
        /// インスタンス
        /// </summary>
        /// <returns></returns>
        public static GBLValues GetInstance()
        {
            return _SingleInstance;
        }
        #endregion

        #region gettweet.pyのパス[GettweetPythonPath]プロパティ
        /// <summary>
        /// gettweet.pyのパス[GettweetPythonPath]プロパティ用変数
        /// </summary>
        string _GettweetPythonPath = @"Common\Python\gettweet.py";
        /// <summary>
        /// gettweet.pyのパス[GettweetPythonPath]プロパティ
        /// </summary>
        public string GettweetPythonPath
        {
            get
            {
                string path = Path.Combine(Directory.GetParent(System.Reflection.Assembly.GetEntryAssembly()!.Location)!.FullName, _GettweetPythonPath);
                return _GettweetPythonPath;
            }
            set
            {
                if (_GettweetPythonPath == null || !_GettweetPythonPath.Equals(value))
                {
                    _GettweetPythonPath = value;
                    NotifyPropertyChanged("GettweetPythonPath");
                }
            }
        }
        #endregion

        #region gettweet2.pyのパス[Gettweet2PythonPath]プロパティ
        /// <summary>
        /// gettweet.pyのパス[Gettweet2PythonPath]プロパティ用変数
        /// </summary>
        string _Gettweet2PythonPath = @"Common\Python\gettweet2.py";
        /// <summary>
        /// gettweet.pyのパス[GettweetPythonPath]プロパティ
        /// </summary>
        public string Gettweet2PythonPath
        {
            get
            {
                string path = Path.Combine(Directory.GetParent(System.Reflection.Assembly.GetEntryAssembly()!.Location)!.FullName, _Gettweet2PythonPath);
                return path;
            }
            set
            {
                if (_Gettweet2PythonPath == null || !_Gettweet2PythonPath.Equals(value))
                {
                    _Gettweet2PythonPath = value;
                    NotifyPropertyChanged("Gettweet2PythonPath");
                }
            }
        }
        #endregion

        #region wordcloud_from_tweetid.pyのパス[Gettweet3PythonPath]プロパティ
        /// <summary>
        /// wordcloud_from_tweetid.pyのパス[Gettweet3PythonPath]プロパティ用変数
        /// </summary>
        string _Gettweet3PythonPath = @"Common\Python\wordcloud_from_tweetid.py";
        /// <summary>
        /// wordcloud_from_tweetid.pyのパス[GettweetPythonPath]プロパティ
        /// </summary>
        public string Gettweet3PythonPath
        {
            get
            {
                string path = Path.Combine(Directory.GetParent(System.Reflection.Assembly.GetEntryAssembly()!.Location)!.FullName, _Gettweet3PythonPath);
                return path;
            }
            set
            {
                if (_Gettweet3PythonPath == null || !_Gettweet3PythonPath.Equals(value))
                {
                    _Gettweet3PythonPath = value;
                    NotifyPropertyChanged("Gettweet3PythonPath");
                }
            }
        }
        #endregion

        #region WordCloud用Pythonファイルパス[WordCloudPythonPath]プロパティ
        /// <summary>
        /// WordCloud用Pythonファイルパス[WordCloudPythonPath]プロパティ用変数
        /// </summary>
        string _WordCloudPythonPath = @"Common\Python\exec_wordcloud.py";
        /// <summary>
        /// WordCloud用Pythonファイルパス[WordCloudPythonPath]プロパティ
        /// </summary>
        public string WordCloudPythonPath
        {
            get
            {
                return _WordCloudPythonPath;
            }
            set
            {
                if (_WordCloudPythonPath == null || !_WordCloudPythonPath.Equals(value))
                {
                    _WordCloudPythonPath = value;
                    NotifyPropertyChanged("WordCloudPythonPath");
                }
            }
        }
        #endregion


        #region ツイッターAPIGetCommand用コンフィグ[TwitterAPIConfig]プロパティ
        /// <summary>
        /// ツイッターAPIGetCommand用コンフィグ[TwitterAPIConfig]プロパティ用変数
        /// </summary>
        ConfigManager<TwitterAPIConfigM> _TwitterAPIConfig = new("Config", "TwitterAPI.config", new TwitterAPIConfigM());
        /// <summary>
        /// ツイッターAPIGetCommand用コンフィグ[TwitterAPIConfig]プロパティ
        /// </summary>
        public ConfigManager<TwitterAPIConfigM> TwitterAPIConfig
        {
            get
            {
                return _TwitterAPIConfig;
            }
            set
            {
                if (_TwitterAPIConfig == null || !_TwitterAPIConfig.Equals(value))
                {
                    _TwitterAPIConfig = value;
                    NotifyPropertyChanged("TwitterAPIConfig");
                }
            }
        }
        #endregion
    }
}
