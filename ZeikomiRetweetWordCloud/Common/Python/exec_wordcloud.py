import sys
import demoji
import pandas as pd
from janome.tokenizer import Tokenizer
import re
from wordcloud import WordCloud, STOPWORDS
from PIL import Image
from matplotlib import rcParams,cm
import collections
from collections import Counter, defaultdict

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

def remove_emoji(src_str):
    """絵文字の削除処理

    Args:
        src_str (string): 元の文字列

    Returns:
        string: 絵文字を取り除いた文字列
    """
    return demoji.replace(string=src_str, repl="")

if __name__ == '__main__':
    """ワードクラウドの画像を生成する

    Args:
        args[1]: 入力ファイルパス
        args[2]: フォントファイルのパス.ttf
        args[3]: 背景色
        args[4]: カラーマップ
        args[5]: 出力ファイルパス1(WordCloud画像ファイル)
        args[6]: 出力ファイルパス2(resultファイル(.xlsx))

    """

    args = sys.argv

    input_file_path = args[1]
    font_path = args[2]
    bgcolor = str(args[3])
    colormap = str(args[4])
    path1 = str(args[5])
    path2 = str(args[6])

    # デバッグ用
    print('--- 引数 ---')
    print(input_file_path)
    print(font_path)
    print(bgcolor)
    print(colormap)
    print(path1)
    print(path2)

    # 絵文字コードの取得
    demoji.download_codes()

    f = open(input_file_path, 'r', encoding='utf-8')
    plane_text = f.read()
    f.close()

    # url関連は予め取り除いておく
    plane_text = re.sub(r"(https?|ftp)(:\/\/[-_\.!~*\'()a-zA-Z0-9;\/?:\@&=\+\$,%#]+)", "" ,plane_text)
    plane_text = plane_text.replace("tco","")
    plane_text = plane_text.replace("RT","")
    plane_text = plane_text.replace("amp","")

    # 絵文字を取り除いておく
    plane_text = remove_emoji(plane_text)

    # ワードクラウドデータの出力
    result = output_wordcloud(plane_text, font_path, path1, bgcolor, colormap)
    result = result.split(" ")
    result = collections.Counter(result).most_common()

    # データフレームを作成
    #pd.DataFrame(result).to_excel(path2,sheet_name='data')
