## 実行方法
### 準備 : トークンの設定
```bash
echo 'OPENAI_API_KEY=your_openai_api_key' >> .env
echo 'HUGGING_FACE_TOKEN=your_huggingface_token' >> .env
```
### モデルのダウンロード
```bash
poetry install
poetry run python src/download_model.py
```
### 実行 + テスト
```bash
poetry install
poetry run uvicorn src.main:app
poetry poetry run python -m pytest
```

### デプロイ
```bash
poetry run modal deploy src/main.py
```