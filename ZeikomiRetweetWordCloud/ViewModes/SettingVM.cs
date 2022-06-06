using MVVMCore.BaseClass;
using MVVMCore.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeikomiRetweetWordCloud.Common;
using ZeikomiRetweetWordCloud.Models;

namespace ZeikomiRetweetWordCloud.ViewModes
{
    public class SettingVM : ViewModelBase
    {
		#region TwitterAPI 用コンフィグ[TwitterAPIConfig]プロパティ
		/// <summary>
		/// TwitterAPI 用コンフィグ[TwitterAPIConfig]プロパティ
		/// </summary>
		public ConfigManager<TwitterAPIConfigM> TwitterAPIConfig
		{
			get
			{
				return GBLValues.GetInstance().TwitterAPIConfig;
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
                // ロード処理
                this.TwitterAPIConfig.LoadXML();
            }
            catch (Exception ex)
            {
                ShowMessage.ShowErrorOK(ex.Message, "Error");
            }
        }
        #endregion

        public override void Close(object sender, EventArgs e)
        {
        }

        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            try
            {
                if (ShowMessage.ShowQuestionYesNo("保存した後に画面を閉じます。よろしいですか？", "確認") == System.Windows.MessageBoxResult.Yes)
                {
                    this.TwitterAPIConfig.SaveXML();
                    this.DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                ShowMessage.ShowErrorOK(ex.Message, "Error");
            }
        }
        #endregion

        #region キャンセル
        /// <summary>
        /// キャンセル
        /// </summary>
        public void Cancel()
        {
            try
            {
                if (ShowMessage.ShowQuestionYesNo("保存せず画面を閉じます。よろしいですか？", "確認") == System.Windows.MessageBoxResult.Yes)
                {
                    this.DialogResult = false;
                }
            }
            catch (Exception ex)
            {
                ShowMessage.ShowErrorOK(ex.Message, "Error");
            }
        }
        #endregion
    }
}
