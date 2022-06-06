import os
import sys
import requests
import pandas as pd
from pandas import json_normalize

def get_recent_tweet(bearer_token, query, max_count, lang):
    """最近(直近7日間)のツイートを取得

    Args:
        bearer_token (string): https://developer.twitter.com/en/portal/ で取得するBearer Token
        query (string): 検索文字列などの条件
        max_count (int): 1度に取得できるツイートは100件までのため、何回まで続けて取得するかを指定する

    Raises:
        Exception: リクエストに対し失敗が返ってきた場合

    Returns:
        DataFrame: ツイートデータ一覧、ユーザー情報一覧
    """

    # Twitter APIのURL
    search_url = "https://api.twitter.com/2/tweets/search/recent"

    lang_cmd = ""
    
    # langに何か指定があれば
    if lang != "all" and len(lang) > 0:
        lang_cmd = "lang:" + lang

    # 検索クエリ
    query_params = {'query': query + ' -is:retweet ' + lang_cmd, # -is:retweet → 元のツイートのみを取得する lang:ja
                    'expansions'   : 'author_id',
                    'tweet.fields' : 'created_at,public_metrics,author_id,lang',
                    'user.fields'  : 'created_at,description,id,name,public_metrics,username',
                    'max_results': 100
     }

    # headerにbearer tokenを設定
    headers = {"Authorization": "Bearer {}".format(bearer_token)}

    has_next = True
    c = 0
    result = []
    users = []

    while has_next:
        response = requests.request("GET", search_url, headers=headers, params=query_params)
        if response.status_code != 200:
            raise Exception(response.status_code, response.text)

        response_body = response.json()

        # データの取得
        result += response_body['data']

        # ユーザー情報の取得
        users += response_body['includes']['users']

        # API制限残り回数の表示
        rate_limit = response.headers['x-rate-limit-remaining']
        print('Rate limit remaining: ' + rate_limit)

        c = c + 1
        has_next = ('next_token' in response_body['meta'].keys() and c < max_count)

        # next_tokenがある場合は検索クエリに追加
        if has_next:
            query_params['next_token'] = response_body['meta']['next_token']

    return result, users


def get_planetext(text_list):
    """プレーンテキストの取得処理

    Args:
        text_list (list): テキストデータの配列

    Returns:
        string: プレーンテキスト（URLおよび絵文字の削除）
    """
    texts = ""

    for index, row in text_list.items():
        texts = texts + row + '\n'

    return texts

def output_tweet_data(dir, query, df, df2, df3, plane_text):
    """ツイートデータの外部ファイル出力

    Args:
        dir (string): 出力先ディレクトリ
        query (string): 検索文字列(ファイル名の接頭辞になる)
        df (DataFrame): データ部
        df2 (DataFrame): ユーザー部
        df3 (DataFrame): データ部およびユーザー部をマージ
        div_text (string): プレーンのテキスト
    """
    
    tweet_data_dir=dir + 'tweetdata\\'
    # ディレクトリの作成
    os.makedirs(tweet_data_dir, exist_ok=True)
    plane_text_dir=dir + 'planetext\\'
    # ディレクトリの作成
    os.makedirs(plane_text_dir, exist_ok=True)

    # ツイートデータの出力
    df.to_excel(tweet_data_dir + '{}_data.xlsx'.format(query),sheet_name='data')   # データ部の出力
    df2.to_excel(tweet_data_dir + '{}_user.xlsx'.format(query),sheet_name='data')  # ユーザー部の出力
    df3.to_excel(tweet_data_dir + '{}_merge.xlsx'.format(query),sheet_name='data') # データ部およびユーザー部をマージしたものの出力

    # 出力先ファイル
    f_out = open(plane_text_dir + '{}_tweet.txt'.format(query), 'w', encoding='utf-8')
    f_out.write(plane_text)
    f_out.close()

def search_tweet(bearer_token, query, max_count, lang):
    """_summary_

    Args:
        bearer_token (string): ツイッタートークン
        query (string): ツイッター検索文字列
        max_count (int): Twitter API実行 ループ回数の最大値

    Returns:
        DataFrame: df → Twitter APIのData部()
        DataFrame: df2 → Twitter APIのUser部()
        DataFrame: df3 → Twitter APIの結合()
    """

    # ツイートの取得
    data, user = get_recent_tweet(bearer_token, query, max_count, lang)

    # JSONファイルの正規化
    df = json_normalize(data)
    df2 = json_normalize(user)

    # 冗長項目の削除
    df = df.drop_duplicates(subset=['text'])
    df3 = pd.merge(df, df2, left_on='author_id', right_on='id')
    df3 = df3.drop_duplicates(subset=['text'])


    df3 = df3.loc[:,['id_x','created_at_x','text','lang',\
        'author_id','public_metrics.retweet_count','public_metrics.reply_count',\
        'public_metrics.like_count','public_metrics.quote_count','id_y','username',\
        'name','created_at_y','description','public_metrics.followers_count',\
        'public_metrics.following_count','public_metrics.tweet_count','public_metrics.listed_count']]

    return df, df2, df3


if __name__ == '__main__':
    """メイン処理:ツイッターAPIでツイートを検索し取得結果をファイル出力する。

    Args:
        args[1]: ツイッターAPI BearerToken
        args[2]: 出力ディレクトリ(末尾に\をつけない)
        args[3]: クエリ
        args[4]: all, jp, en
        args[5]: 取得最大数

    """

    args = sys.argv

    bearer_token = args[1]
    outdir = args[2] + "\\"
    query = args[3]
    lang = args[4]
    max_count = int(args[5])

    # ログ表示
    print(query)

    # ツイートデータの検索
    df, df2, df3 = search_tweet(bearer_token, query, max_count, lang)

    # テキストデータの取得
    texts = get_planetext(df['text'])

    # 各種データの出力
    output_tweet_data(outdir, query, df, df2, df3, texts)
