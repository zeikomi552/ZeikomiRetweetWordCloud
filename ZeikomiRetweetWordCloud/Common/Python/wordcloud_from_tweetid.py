import os
import sys
from tkinter import font
import requests
import pandas as pd
from pandas import json_normalize
from janome.tokenizer import Tokenizer
import sqlite3
import demoji
from wordcloud import WordCloud, STOPWORDS
import re


_rate_limit = 0
_ret_code = 0
_search_key = ""
_memo = ""

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

        # グローバル変数に保存
        _rate_limit = rate_limit

        c = c + 1
        has_next = ('next_token' in response_body['meta'].keys() and c < max_count)

        # next_tokenがある場合は検索クエリに追加
        if has_next:
            query_params['next_token'] = response_body['meta']['next_token']

    return result, users


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

def create_table(sqlite_path:str):
    # カレントディレクトリにTEST.dbがなければ、作成します。
    # すでにTEST.dbが作成されていれば、TEST.dbに接続します。
    conn = sqlite3.connect(sqlite_path)


    # 2.sqliteを操作するカーソルオブジェクトを作成
    cur = conn.cursor()

    cur.execute("CREATE TABLE IF NOT EXISTS target_tweet(id text primary key,text text, create_at text, wordcloud_f integer)")

    # 4.データベースの接続を切断
    cur.close()
    conn.close()

def upsert_target_tweet(df):

    # カレントディレクトリにTEST.dbがなければ、作成します。
    # すでにTEST.dbが作成されていれば、TEST.dbに接続します。
    conn = sqlite3.connect(sqlite_path)


    # 2.sqliteを操作するカーソルオブジェクトを作成
    cur = conn.cursor()

    for index, row in df.iterrows():
        id = row['id']
        text = row['text']
        created_at = row['created_at']
        t = (id, text, created_at)
        cur.execute("REPLACE INTO target_tweet(id, text, create_at, wordcloud_f) VALUES (?,?,?, 0);", t)

    conn.commit()

    # 4.データベースの接続を切断
    cur.close()
    conn.close()

def get_ids(sqlite_path:str, base_id:str):
    
    # カレントディレクトリにTEST.dbがなければ、作成します。
    # すでにTEST.dbが作成されていれば、TEST.dbに接続します。
    conn = sqlite3.connect(sqlite_path)


    # 2.sqliteを操作するカーソルオブジェクトを作成
    cur = conn.cursor()

    df=pd.read_sql_query('SELECT * FROM target_tweet where id = \'{}\''.format(base_id), conn)

    # 4.データベースの接続を切断
    cur.close()
    conn.close()

    return df

def remove_emoji(src_str):
    """絵文字の削除処理

    Args:
        src_str (string): 元の文字列

    Returns:
        string: 絵文字を取り除いた文字列
    """
    return demoji.replace(string=src_str, repl="")

def get_word_str(text):
 
    t = Tokenizer()
    token = t.tokenize(text)
    word_list = []
 
    for line in token:
        tmp = re.split('\t|,', str(line))
        # 名詞のみ対象
        if tmp[1] in ["名詞"]:
            # さらに絞り込み
            if tmp[2] in ["一般", "固有名詞"]:
                word_list.append(tmp[0])

    return " " . join(word_list)

def output_wordcloud(plane_text:str, font_path:str, filepath:str, bgcolor:str, colormap:str):
    """プレーンのテキストからワードクラウドを作成する

    Args:
        plane_text (str): プレーンテキスト
        font_path (str): フォントファイルのパス
        filepath (str): 出力先ファイルパス
        bgcolor(str) : 背景色
        colormap(str):カラーマップ
    """
    _memo = filepath

    # 文字列の取得
    text = get_word_str(plane_text)

    # 除外するワード
    stopwords = set(STOPWORDS)
    stopwords.add("https")
    stopwords.add("tco")
    stopwords.add("t")
    stopwords.add("co")
    stopwords.add("amp")


    # WordCloudの作成
    wordcloud = WordCloud(background_color=bgcolor, font_path=font_path, stopwords=stopwords,\
                         width=900, height=500, collocations=False, colormap=colormap).generate(text)

    # ファイル出力
    wordcloud.to_file(filepath)

    return text



def get_keyword_doublequate(text:str):
    """ダブルクォートで囲まれたキーワードを探す

    Args:
        text (str): ツイート内容
    """

    start_index = text.find("\"")

    if start_index >= 0:
        end_index = text.find("\"", start_index + 1)
    else:
        return ""
    
    if end_index > 0:
        keyword = text[start_index:end_index].replace("\"","")
    else:
        return ""

    return keyword


def get_planetext(text_list):
    """プレーンテキストの取得処理

    Args:
        text_list (list): テキストデータの配列

    Returns:
        string: プレーンテキスト（URLおよび絵文字の削除）
    """
    texts = ""

    for index, row in text_list.items():
        try:
            texts = texts + remove_emoji(row) + '\n'
        except Exception as e:
            _ret_code = -1  # 文字列から絵文字を取り除くのに失敗
            #print('message:' + e.message)
            #print("error:{}行目\r\n内容:{}".format(index, row))

    texts = re.sub(r"(https?|ftp)(:\/\/[-_\.!~*\'()a-zA-Z0-9;\/?:\@&=\+\$,%#]+)", "" ,texts)
    texts = texts.replace("tco","")
    texts = texts.replace("RT","")
    texts = texts.replace("amp","")
    return texts

def get_row_plane_text(text, query):
    """プレーンテキストの取得処理

    Args:
        text (str): テキストデータ
        query (str): 除外するデータ

    Returns:
        string: プレーンテキスト（URLおよび絵文字の削除）
    """

    # 検索キーワードに使用した文字列は削除
    texts = text.replace(query,"")

    # 細々とした微妙な文字は削除
    texts = re.sub(r"(https?|ftp)(:\/\/[-_\.!~*\'()a-zA-Z0-9;\/?:\@&=\+\$,%#]+)", "" ,texts)
    texts = texts.replace("tco","")
    texts = texts.replace("RT","")
    texts = texts.replace("amp","")
    return texts


if __name__ == '__main__':
    """ツイートIDに対しキーワードを抜き出してワードクラウドを作成するスクリプト

    Args:
        args[1]: ツイッターAPI BearerToken
        args[2]: 出力ディレクトリ(末尾に\をつけない)
        args[3]: 対象ツイートID
        args[4]: all, jp, en
        args[5]: 取得最大数
        args[6]: SQLiteファイルパス
        args[7]: フォントパス
        args[8]: カラーマップ

    """

    try:
        args = sys.argv

        bearer_token = args[1]
        out_dir = args[2]
        base_id = args[3]
        lang = args[4]
        max_count = int(args[5])
        sqlite_path = args[6]
        font_path = args[7]
        colormap = args[8]

        # sqliteテーブルの作成
        create_table(sqlite_path)

        _ret_code = 1

        df = get_ids(sqlite_path, base_id)
        
        _ret_code = 2

        keyword = ""
        for index, row in df.iterrows():

            # ダブルクォートで括られたキーワードを探す
            keyword = get_keyword_doublequate(row['text'])

            # 無ければ名詞を含まれる使用する
            if len(keyword) <= 0:
                plane_text = get_row_plane_text(row['text'], row['query'])
                plane_text = get_word_str(plane_text)
                noun_list = plane_text.split(" ")
                if len(noun_list) > 0:
                    keyword = noun_list[0]

            # デバッグ用にキーワードを保持する
            _search_key = keyword

            break

        _ret_code = 3


        if len(keyword) > 0:
            df, df2, df3 = search_tweet(bearer_token, keyword, max_count, lang)
            plane_text = get_planetext(df['text'])
            _ret_code = 5
            wordcloud_file_path = out_dir + "\\{}.png".format(base_id)
            output_wordcloud(plane_text, font_path, wordcloud_file_path ,"white",colormap)


        print("ret_code={} search_keyword={} file_path={} out_dir={}".format(_ret_code, _search_key, _memo, out_dir + "\\{}.png".format(base_id)))

    except Exception as e:
        print("ret_code={} search_keyword={} message={}".format(_ret_code, _search_key, e.message))
        