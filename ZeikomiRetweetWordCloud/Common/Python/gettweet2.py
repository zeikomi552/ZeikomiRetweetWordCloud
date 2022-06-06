import os
import sys
import requests
import pandas as pd
from pandas import json_normalize
from janome.tokenizer import Tokenizer
import sqlite3
import datetime

def get_recent_tweet(bearer_token, query, max_count, lang, since_id):
    """最近(直近7日間)のツイートを取得

    Args:
        bearer_token (string): https://developer.twitter.com/en/portal/ で取得するBearer Token
        query (string): 検索文字列などの条件
        max_count (int): 1度に取得できるツイートは100件までのため、何回まで続けて取得するかを指定する
        lang (str): 言語を指定する(all,en,ja)
        since_id (str): 開始インデックスを指定する(0以下は全て取得)

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

    if since_id > '0':
        # 検索クエリ
        query_params = {'query': query + ' -is:retweet ' + lang_cmd, # -is:retweet → 元のツイートのみを取得する lang:ja
                        'since_id'     : since_id,
                        'expansions'   : 'author_id',
                        'tweet.fields' : 'created_at,public_metrics,author_id,lang',
                        'user.fields'  : 'created_at,description,id,name,public_metrics,username',
                        'max_results': 100
         }
    else:
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


def search_tweet(bearer_token, query, max_count, lang, since_id):
    """_summary_

    Args:
        bearer_token (string): ツイッタートークン
        query (string): ツイッター検索文字列
        max_count (int): Twitter API実行 ループ回数の最大値
        lang (str): 言語指定(ja,en,all)
        since_id (str): 開始ID位置（この値を含まない）

    Returns:
        DataFrame: df → Twitter APIのData部()
        DataFrame: df2 → Twitter APIのUser部()
        DataFrame: df3 → Twitter APIの結合()
    """

    # ツイートの取得
    data, user = get_recent_tweet(bearer_token, query, max_count, lang, since_id)

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

    df3 = df3.rename(columns={'id_x': 'id', 'created_at_x': 'created_at'})
    return df, df2, df3

def create_table(sqlite_path:str):
    # カレントディレクトリにTEST.dbがなければ、作成します。
    # すでにTEST.dbが作成されていれば、TEST.dbに接続します。
    conn = sqlite3.connect(sqlite_path)


    # 2.sqliteを操作するカーソルオブジェクトを作成
    cur = conn.cursor()

    cur.execute("CREATE TABLE IF NOT EXISTS target_tweet(id text primary key,text text, username text, query text, create_at text, insert_tm datetime, wordcloud_status integer default 0, font text, colormap text)")

    # 4.データベースの接続を切断
    cur.close()
    conn.close()

def upsert_target_tweet(df, query):

    # カレントディレクトリにsqliteファイルがなければ、作成します。
    # すでにsqliteファイルが作成されていれば、sqliteファイルに接続します。
    conn = sqlite3.connect(sqlite_path)


    # 2.sqliteを操作するカーソルオブジェクトを作成
    cur = conn.cursor()

    for index, row in df.iterrows():
        t = (row['id'], row['text'], row['username'], row['created_at'], datetime.datetime.now(), query)
        cur.execute("INSERT INTO target_tweet(id, text, username, create_at, insert_tm, query) VALUES (?,?,?,?,?,?) on conflict(id) do nothing;", t)

    conn.commit()

    # 4.データベースの接続を切断
    cur.close()
    conn.close()


if __name__ == '__main__':
    """メイン処理:ツイッターAPIでツイートを検索し取得結果をファイル出力する。

    Args:
        args[1]: ツイッターAPI BearerToken
        args[2]: 出力ディレクトリ(末尾に\をつけない)
        args[3]: クエリ
        args[4]: all, jp, en
        args[5]: 取得最大数
        args[6]: SQLiteファイルパス
        args[7]: 開始インデックス位置

    """

    args = sys.argv

    bearer_token = args[1]
    outdir = args[2] + "\\"
    query = args[3]
    lang = args[4]
    max_count = int(args[5])
    sqlite_path = args[6]
    since_id = args[7]

    # sqliteテーブルの作成
    create_table(sqlite_path)

    # ツイートデータの検索
    df, df2, df3 = search_tweet(bearer_token, query, max_count, lang, since_id)

    # キーワード検索で取得したツイート内容をUPSERT
    upsert_target_tweet(df3, query)
