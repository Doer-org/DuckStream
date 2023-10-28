## 実行方法
### 準備 : トークンの設定
```bash
echo 'OPENAI_API_KEY=your_openai_api_key' >> .env
```
### モデルのダウンロード
```bash
poetry install
poetry run python src/download_model.py
```
### 実行 + テスト
```bash
poetry install
poetry run uvicorn src.main:app --reload
poetry run pytest # テスト
```

### デプロイ
```bash
poetry run modal deploy src/main.py
```