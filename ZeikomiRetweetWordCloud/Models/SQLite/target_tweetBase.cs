using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeikomiRetweetWordCloud.Models.SQLite
{
	/// <summary>
	/// 対象ツイート情報
	/// target_tweetテーブルをベースに作成しています
	/// 作成日：2022/06/05 作成者gohya
	/// </summary>
	[Table("target_tweet")]
	public class target_tweetBase : INotifyPropertyChanged
	{
		#region パラメータ
		#region ツイートID[id]プロパティ
		/// <summary>
		/// ツイートID[id]プロパティ用変数
		/// </summary>
		String _id = string.Empty;
		/// <summary>
		/// ツイートID[id]プロパティ
		/// </summary>
		[Key]
		[Column("id")]
		public String id
		{
			get
			{
				return _id;
			}
			set
			{
				if (!_id.Equals(value))
				{
					_id = value;
					NotifyPropertyChanged("id");
				}
			}
		}
		#endregion

		#region ツイート内容[text]プロパティ
		/// <summary>
		/// ツイート内容[text]プロパティ用変数
		/// </summary>
		String _text = string.Empty;
		/// <summary>
		/// ツイート内容[text]プロパティ
		/// </summary>
		[Column("text")]
		public String text
		{
			get
			{
				return _text;
			}
			set
			{
				if (!_text.Equals(value))
				{
					_text = value;
					NotifyPropertyChanged("text");
				}
			}
		}
		#endregion

		#region ツイート日時(UTC)[create_at]プロパティ
		/// <summary>
		/// ツイート日時(UTC)[create_at]プロパティ用変数
		/// </summary>
		String _create_at = string.Empty;
		/// <summary>
		/// ツイート日時(UTC)[create_at]プロパティ
		/// </summary>
		[Column("create_at")]
		public String create_at
		{
			get
			{
				return _create_at;
			}
			set
			{
				if (!_create_at.Equals(value))
				{
					_create_at = value;
					NotifyPropertyChanged("create_at");
				}
			}
		}
		#endregion

		#region 挿入日時(現地時刻)[insert_tm]プロパティ
		/// <summary>
		/// 挿入日時(現地時刻)[insert_tm]プロパティ用変数
		/// </summary>
		DateTime _insert_tm = DateTime.MinValue;
		/// <summary>
		/// 挿入日時(現地時刻)[insert_tm]プロパティ
		/// </summary>
		[Column("insert_tm")]
		public DateTime insert_tm
		{
			get
			{
				return _insert_tm;
			}
			set
			{
				if (!_insert_tm.Equals(value))
				{
					_insert_tm = value;
					NotifyPropertyChanged("insert_tm");
				}
			}
		}
		#endregion

		#region WordCloud実行状態(キーワード抽出時:0 ワードクラウド送信リツイート時:99)[wordcloud_status]プロパティ
		/// <summary>
		/// WordCloud実行状態(キーワード抽出時:0 ワードクラウド送信リツイート時:99)[wordcloud_status]プロパティ用変数
		/// </summary>
		Int32 _wordcloud_status = 0;
		/// <summary>
		/// WordCloud実行状態(キーワード抽出時:0 ワードクラウド送信リツイート時:99)[wordcloud_status]プロパティ
		/// </summary>
		[Column("wordcloud_status")]
		public Int32 wordcloud_status
		{
			get
			{
				return _wordcloud_status;
			}
			set
			{
				if (!_wordcloud_status.Equals(value))
				{
					_wordcloud_status = value;
					NotifyPropertyChanged("wordcloud_status");
				}
			}
		}
		#endregion

		#region ユーザー名(スクリーン名)[username]プロパティ
		/// <summary>
		/// ユーザー名(スクリーン名)[username]プロパティ用変数
		/// </summary>
		String _username = string.Empty;
		/// <summary>
		/// ユーザー名(スクリーン名)[username]プロパティ
		/// </summary>
		[Column("username")]
		public String username
		{
			get
			{
				return _username;
			}
			set
			{
				if (!_username.Equals(value))
				{
					_username = value;
					NotifyPropertyChanged("username");
				}
			}
		}
		#endregion

		#region フォント[font]プロパティ
		/// <summary>
		/// フォント[font]プロパティ用変数
		/// </summary>
		String? _font = string.Empty;
		/// <summary>
		/// フォント[font]プロパティ
		/// </summary>
		[Column("font")]
		public String? font
		{
			get
			{
				return _font;
			}
			set
			{
				if (_font == null || !_font.Equals(value))
				{
					_font = value;
					NotifyPropertyChanged("font");
				}
			}
		}
		#endregion

		#region カラーマップ[colormap]プロパティ
		/// <summary>
		/// カラーマップ[colormap]プロパティ用変数
		/// </summary>
		String? _colormap = string.Empty;
		/// <summary>
		/// カラーマップ[colormap]プロパティ
		/// </summary>
		[Column("colormap")]
		public String? colormap
		{
			get
			{
				return _colormap;
			}
			set
			{
				if (_colormap == null || !_colormap.Equals(value))
				{
					_colormap = value;
					NotifyPropertyChanged("colormap");
				}
			}
		}
		#endregion


		#endregion

		#region 関数
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public target_tweetBase()
		{

		}
		#endregion

		#region コピーコンストラクタ
		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		/// <param name="item">コピー内容</param>
		public target_tweetBase(target_tweetBase item)
		{
			// 要素のコピー
			Copy(item);
		}
		#endregion

		#region コピー
		/// <summary>
		/// コピー
		/// </summary>
		/// <param name="item">コピー内容</param>
		public void Copy(target_tweetBase item)
		{
			this.id = item.id;

			this.text = item.text;

			this.create_at = item.create_at;

			this.insert_tm = item.insert_tm;

			this.wordcloud_status = item.wordcloud_status;

			this.username = item.username;

			this.font = item.font;

			this.colormap = item.colormap;


		}
		#endregion

		#region Insert処理
		/// <summary>
		/// Insert処理
		/// </summary>
		/// <param name="item">Insertする要素</param>
		public static void Insert(target_tweetBase item)
		{
			using (var db = new SQLiteDataContext())
			{
				Insert(db, item);
				db.SaveChanges();
			}
		}
		#endregion

		#region Insert処理
		/// <summary>
		/// Insert処理
		/// </summary>
		/// <param name="db">SQLiteDataContext</param>
		/// <param name="item">Insertする要素</param>
		public static void Insert(SQLiteDataContext db, target_tweetBase item)
		{
			// Insert
			db.Add<target_tweetBase>(item);
		}
		#endregion

		#region Update処理
		/// <summary>
		/// Update処理
		/// </summary>
		/// <param name="pk_item">更新する主キー（主キーの値のみ入っていれば良い）</param>
		/// <param name="update_item">テーブル更新後の状態</param>
		public static void Update(target_tweetBase pk_item, target_tweetBase update_item)
		{
			using (var db = new SQLiteDataContext())
			{
				Update(db, pk_item, update_item);
				db.SaveChanges();
			}
		}
		#endregion

		#region Update処理
		/// <summary>
		/// Update処理
		/// </summary>
		/// <param name="db">SQLiteDataContext</param>
		/// <param name="pk_item">更新する主キー（主キーの値のみ入っていれば良い）</param>
		/// <param name="update_item">テーブル更新後の状態</param>
		public static void Update(SQLiteDataContext db, target_tweetBase pk_item, target_tweetBase update_item)
		{
			var item = db.DbSet_target_tweet.SingleOrDefault(x => x.id.Equals(pk_item.id));

			if (item != null)
			{
				item.Copy(update_item);
			}
		}
		#endregion

		#region Delete処理
		/// <summary>
		/// Delete処理
		/// </summary>
		/// <param name="pk_item">削除する主キー（主キーの値のみ入っていれば良い）</param>
		public static void Delete(target_tweetBase pk_item)
		{
			using (var db = new SQLiteDataContext())
			{
				Delete(db, pk_item);
				db.SaveChanges();
			}
		}
		#endregion

		#region Delete処理
		/// <summary>
		/// Delete処理
		/// </summary>
		/// <param name="db">SQLiteDataContext</param>
		/// <param name="pk_item">削除する主キー（主キーの値のみ入っていれば良い）</param>
		public static void Delete(SQLiteDataContext db, target_tweetBase pk_item)
		{
			var item = db.DbSet_target_tweet.SingleOrDefault(x => x.id.Equals(pk_item.id));
			if (item != null)
			{
				db.DbSet_target_tweet.Remove(item);
			}
		}
		#endregion

		#region Select処理
		/// <summary>
		/// Select処理
		/// </summary>
		/// <returns>全件取得</returns>
		public static List<target_tweetBase> Select()
		{
			using (var db = new SQLiteDataContext())
			{
				return Select(db);
			}
		}
		#endregion

		#region Select処理
		/// <summary>
		/// Select処理
		/// </summary>
		/// <param name="db">SQLiteDataContext</param>
		/// <returns>全件取得</returns>
		public static List<target_tweetBase> Select(SQLiteDataContext db)
		{
			return db.DbSet_target_tweet.ToList<target_tweetBase>();
		}
		#endregion
		#endregion

		#region INotifyPropertyChanged 
		public event PropertyChangedEventHandler? PropertyChanged;

		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}
		#endregion
	}
}
