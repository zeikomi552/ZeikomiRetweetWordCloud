using ClosedXML.Excel;
using MVVMCore.BaseClass;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeikomiRetweetWordCloud.Models
{
    public class TweetDataM : ModelBase
    {
		public TweetDataM(IXLRow row)
        {
			// 行データのセット
			SetCellData(row);

		}
		#region ツイートID[Id]プロパティ
		/// <summary>
		/// ツイートID[Id]プロパティ用変数
		/// </summary>
		string _Id = string.Empty;
		/// <summary>
		/// ツイートID[Id]プロパティ
		/// </summary>
		public string Id
		{
			get
			{
				return _Id;
			}
			set
			{
				if (_Id == null || !_Id.Equals(value))
				{
					_Id = value;
					NotifyPropertyChanged("Id");
				}
			}
		}
		#endregion

		#region テキスト[Text]プロパティ
		/// <summary>
		/// テキスト[Text]プロパティ用変数
		/// </summary>
		string _Text = string.Empty;
		/// <summary>
		/// テキスト[Text]プロパティ
		/// </summary>
		public string Text
		{
			get
			{
				return _Text;
			}
			set
			{
				if (_Text == null || !_Text.Equals(value))
				{
					_Text = value;
					NotifyPropertyChanged("Text");
				}
			}
		}
		#endregion

		/// <summary>
		/// 値の調整
		/// </summary>
		/// <param name="cell_value">セル値</param>
		/// <param name="type">型タイプ 0:string 1:int 2:DateTimeOffset</param>
		/// <returns></returns>
		private object AdjustValue(object cell_value, int type)
		{
			switch (type)
			{
				case 0: // string
					{
						if (cell_value == null)
						{
							return string.Empty;
						}
						else
						{
							return cell_value.ToString()!;
						}
					}
				case 1: // int
					{
						if (cell_value == null)
						{
							return (int)0;
						}
						else
						{
							int num = 0;
							int.TryParse(cell_value.ToString()!, out num);
							return num;
						}
					}
				case 2: // DateTimeOffset
					{
						if (cell_value == null)
						{
							return DateTimeOffset.MinValue;
						}
						else
						{
							try
							{
								DateTime datetime = DateTime.MinValue;
								string date_txt = cell_value.ToString()!;
								DateTime.TryParseExact(date_txt, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out datetime);
								DateTimeOffset dt_ofset = datetime;
								return dt_ofset;
							}
							catch
							{
								return DateTimeOffset.MinValue;
							}
						}
					}
				default:
					{
						return cell_value;
					}
			}

		}

		/// <summary>
		/// セルデータのセット
		/// </summary>
		/// <param name="row">行</param>
		public void SetCellData(IXLRow row)
		{
			this.Id = (string)AdjustValue(row.Cell("B").Value, 0);
			this.Text = (string)AdjustValue(row.Cell("D").Value, 0);
		}

	}
}
