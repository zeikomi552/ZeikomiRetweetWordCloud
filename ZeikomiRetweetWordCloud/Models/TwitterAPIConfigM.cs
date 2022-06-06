using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeikomiRetweetWordCloud.Models
{
    public class TwitterAPIConfigM : ModelBase
    {
		#region Python.exeのパス[PythonPath]プロパティ
		/// <summary>
		/// Python.exeのパス[PythonPath]プロパティ用変数
		/// </summary>
		string _PythonPath = @"C:\ProgramData\Anaconda3\python.exe";
		/// <summary>
		/// Python.exeのパス[PythonPath]プロパティ
		/// </summary>
		public string PythonPath
		{
			get
			{
				return _PythonPath;
			}
			set
			{
				if (_PythonPath == null || !_PythonPath.Equals(value))
				{
					_PythonPath = value;
					NotifyPropertyChanged("PythonPath");
				}
			}
		}
		#endregion

		#region アクセストークン[AccessToken]プロパティ
		/// <summary>
		/// アクセストークン[AccessToken]プロパティ用変数
		/// </summary>
		string _AccessToken = string.Empty;
		/// <summary>
		/// アクセストークン[AccessToken]プロパティ
		/// </summary>
		public string AccessToken
		{
			get
			{
				return _AccessToken;
			}
			set
			{
				if (_AccessToken == null || !_AccessToken.Equals(value))
				{
					_AccessToken = value;
					NotifyPropertyChanged("AccessToken");
				}
			}
		}
		#endregion

		#region アクセスシークレット[AccessSecret]プロパティ
		/// <summary>
		/// アクセスシークレット[AccessSecret]プロパティ用変数
		/// </summary>
		string _AccessSecret = string.Empty;
		/// <summary>
		/// アクセスシークレット[AccessSecret]プロパティ
		/// </summary>
		public string AccessSecret
		{
			get
			{
				return _AccessSecret;
			}
			set
			{
				if (_AccessSecret == null || !_AccessSecret.Equals(value))
				{
					_AccessSecret = value;
					NotifyPropertyChanged("AccessSecret");
				}
			}
		}
		#endregion

		#region コンシューマーキー[ConsumerKey]プロパティ
		/// <summary>
		/// コンシューマーキー[ConsumerKey]プロパティ用変数
		/// </summary>
		string _ConsumerKey = string.Empty;
		/// <summary>
		/// コンシューマーキー[ConsumerKey]プロパティ
		/// </summary>
		public string ConsumerKey
		{
			get
			{
				return _ConsumerKey;
			}
			set
			{
				if (_ConsumerKey == null || !_ConsumerKey.Equals(value))
				{
					_ConsumerKey = value;
					NotifyPropertyChanged("ConsumerKey");
				}
			}
		}
		#endregion

		#region コンシューマーシークレット[ConsumerSecret]プロパティ
		/// <summary>
		/// コンシューマーシークレット[ConsumerSecret]プロパティ用変数
		/// </summary>
		string _ConsumerSecret = string.Empty;
		/// <summary>
		/// コンシューマーシークレット[ConsumerSecret]プロパティ
		/// </summary>
		public string ConsumerSecret
		{
			get
			{
				return _ConsumerSecret;
			}
			set
			{
				if (_ConsumerSecret == null || !_ConsumerSecret.Equals(value))
				{
					_ConsumerSecret = value;
					NotifyPropertyChanged("ConsumerSecret");
				}
			}
		}
		#endregion

		#region BearerToken[BearerToken]プロパティ
		/// <summary>
		/// BearerToken[BearerToken]プロパティ用変数
		/// </summary>
		string _BearerToken = string.Empty;
		/// <summary>
		/// BearerToken[BearerToken]プロパティ
		/// </summary>
		public string BearerToken
		{
			get
			{
				return _BearerToken;
			}
			set
			{
				if (_BearerToken == null || !_BearerToken.Equals(value))
				{
					_BearerToken = value;
					NotifyPropertyChanged("BearerToken");
				}
			}
		}
		#endregion

		string ConfigFilePath { get; set; } = @"Config\TwitterAPIConfig.config";

		#region ファイルの保存処理
		/// <summary>
		/// ファイルの保存処理
		/// </summary>
		public void SaveXML()
		{
			XMLUtil.Seialize<TwitterAPIConfigM>(ConfigFilePath, this);
		}
		#endregion

		#region ロード処理
		/// <summary>
		/// ロード処理
		/// </summary>
		/// <returns>ファイルのロード処理</returns>
		public bool LoadXML()
		{
			if (File.Exists(this.ConfigFilePath))
			{
				var tmp = XMLUtil.Deserialize<TwitterAPIConfigM>(this.ConfigFilePath);

				this.AccessToken = tmp.AccessToken;
				this.AccessSecret = tmp.AccessSecret;
				this.ConsumerSecret = tmp.ConsumerSecret;
				this.ConsumerKey = tmp.ConsumerKey;
				this.BearerToken = tmp.BearerToken;
				this.PythonPath = tmp.PythonPath;

				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion
	}
}
