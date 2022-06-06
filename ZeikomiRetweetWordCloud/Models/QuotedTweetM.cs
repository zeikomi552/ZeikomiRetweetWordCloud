using CoreTweet;
using MVVMCore.BaseClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeikomiRetweetWordCloud.Common;

namespace ZeikomiRetweetWordCloud.Models
{
    public class QuotedTweetM : ModelBase
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

        #region トークンの作成
        /// <summary>
        /// トークンの作成
        /// </summary>
        /// <returns>TwitterAPIアクセス用トークン</returns>
        private static CoreTweet.Tokens Createoken()
        {
            //トークンの生成
            return CoreTweet.Tokens.Create(CommonValues.TwitterAPIConfig.Item.ConsumerKey,
                                           CommonValues.TwitterAPIConfig.Item.ConsumerSecret,
                                          CommonValues.TwitterAPIConfig.Item.AccessToken,
                                          CommonValues.TwitterAPIConfig.Item.AccessSecret);
        }
        #endregion



        #region 画像付きリツイート
        /// <summary>
        /// 画像付きリツイート
        /// </summary>
        /// <param name="username">ユーザー名(スクリーン名)</param>
        /// <param name="tweetid">ツイートID</param>
        /// <param name="text">投稿文字列</param>
        /// <param name="image_path">画像ファイルパス</param>
        public static void QuotedTweet(string username, string tweetid, string text, string image_path)
        {
            string attachment_url = string.Format(@"https://twitter.com/{0}/status/{1}", username, tweetid);
            QuotedTweet(attachment_url, text, image_path);
        }

        /// <summary>
        /// 画像付きリツイート
        /// </summary>
        /// <param name="att_url">アタッチメントURL</param>
        /// <param name="text">投稿文字列</param>
        /// <param name="image_path">画像ファイルパス</param>
        public static void QuotedTweet(string att_url, string text, string image_path)
        {
            var token = Createoken();

            // 画像をアップロード
            //media 引数には FileInfo, Stream, IEnumerable<byte> が指定できます。
            //また media_data 引数に画像を BASE64 でエンコードした文字列を指定することができます。
            MediaUploadResult upload_result = token.Media.Upload(media: new FileInfo(image_path));

            // 画像とテキストをツイート
            token.Statuses.Update(status => text, attachment_url => att_url, media_ids => upload_result.MediaId);
        }

        /// <summary>
        /// 画像なしリツイート
        /// </summary>
        /// <param name="username">ユーザー名(スクリーン名)</param>
        /// <param name="tweetid">ツイートID</param>
        /// <param name="text">投稿文字列</param>
        public static void QuotedTweetNoMedia(string username, string tweetid, string text)
        {
            string attachment_url = string.Format(@"https://twitter.com/{0}/status/{1}", username, tweetid);
            QuotedTweetNoMedia(attachment_url, text);
        }
        /// <summary>
        /// 画像なしリツイート
        /// </summary>
        /// <param name="att_url">アタッチメントURL</param>
        /// <param name="text">投稿文字列</param>
        public static void QuotedTweetNoMedia(string att_url, string text)
        {
            var token = Createoken();

            // 画像とテキストをツイート
            token.Statuses.Update(status => text, attachment_url => att_url/*, media_ids => upload_result.MediaId*/);
        }
        #endregion
    }
}
