import base64
import io
import unittest

from fastapi.testclient import TestClient
from PIL import Image

from src.main import app
from src.models import InferenceRequest

client = TestClient(app)


class TestInferenceEndpoint(unittest.TestCase):
    def setUp(self) -> None:
        self.test_image_path = "data/sample/duck.png"

    def test_inference(self) -> None:
        with open(self.test_image_path, "rb") as image_file:
            encoded_string = base64.b64encode(image_file.read()).decode()

        request = InferenceRequest(image=encoded_string, prompt="ラーメン", strength=0.1)
        response = client.post("/inference", json=request.dict())

        self.assertEqual(response.status_code, 200)

        image = base64.b64decode(response.json()["converted_image"])
        image = Image.open(io.BytesIO(image))
        self.assertIsNotNone(image)
