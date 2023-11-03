## 実行方法 : Poetry
### トークン設定
```bash
echo 'OPENAI_API_KEY=your_openai_api_key' >> .env
echo 'HUGGING_FACE_TOKEN=your_huggingface_token' >> .env
```
### インストール
```bash
poetry install
```
### 実行
```bash
poetry run uvicorn src.main:app
```
### テスト
```bash
poetry run pytest
```
## 実行方法 : Modal
### 実行
```bash
modal run src/main.py
```
### デプロイ
```bash
modal deploy src/main.py
```
