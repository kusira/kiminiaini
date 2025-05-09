import os
from PIL import Image

# 基本パス
base_dir = os.path.dirname(os.path.abspath(__file__))
input_dir = os.path.join(base_dir, "png")
output_dir = os.path.join(base_dir, "png_resized")

# 画像を10倍にリサイズして保存する関数
def resize_and_save_image(input_path, output_path):
    with Image.open(input_path) as img:
        new_size = (img.width * 10, img.height * 10)
        resized_img = img.resize(new_size, Image.NEAREST)  # アンチエイリアスなしならNEAREST
        resized_img.save(output_path)

# 再帰的に.pngを探して処理
for root, dirs, files in os.walk(input_dir):
    for file in files:
        if file.lower().endswith('.png'):
            input_path = os.path.join(root, file)

            # 入力パスに対応した出力パスを作成
            relative_path = os.path.relpath(input_path, input_dir)
            output_path = os.path.join(output_dir, relative_path)

            # 出力ディレクトリがなければ作成
            os.makedirs(os.path.dirname(output_path), exist_ok=True)

            # 画像のリサイズと保存
            resize_and_save_image(input_path, output_path)
            print(f"Resized and saved: {output_path}")
