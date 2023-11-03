import subprocess
from modal import Image, Secret, Stub, asgi_app, method, gpu
from .models import InferenceRequest

def download_models():
    import torch
    from diffusers import StableDiffusionImg2ImgPipeline
    pipe = StableDiffusionImg2ImgPipeline.from_pretrained("runwayml/stable-diffusion-v1-5", torch_dtype=torch.float16, revision="fp16")
    pipe.save_pretrained("data/stable-diffusion-v1-5")
    subprocess.run(['git', 'clone', "https://github.com/MosasoM/inappropriate-words-ja.git", "data/inappropriate-words-ja"])

image = (
    Image.debian_slim()
    .apt_install("git")
    .poetry_install_from_file(poetry_pyproject_toml="pyproject.toml", poetry_lockfile="poetry.lock")
    .run_function(download_models)
)

stub = Stub("duckstream-fastapi", image=image, secrets=[Secret.from_name("duckstream-ml-secrets")])
@stub.cls(gpu=gpu.T4(),container_idle_timeout=60)
class Model:
    def __enter__(self):
        from .morphoto import Morphoto
        from .configs import MorphotoConfig
        from omegaconf import OmegaConf

        morphoto_config = OmegaConf.create(MorphotoConfig)
        self.morphoto = Morphoto(morphoto_config)

    @method()
    def inference(self, request: InferenceRequest):
        if request.is_mock:
            return {"converted_image": request.image, "prompt": request.prompt}

        from PIL import Image
        import io
        import base64

        image = io.BytesIO(base64.b64decode(request.image))
        image = Image.open(image)
        converted_image, prompt = self.morphoto.convert(request.prompt, image, request.strength)

        buffered = io.BytesIO()
        converted_image.save(buffered, format="PNG")
        converted_image = base64.b64encode(buffered.getvalue()).decode()
        result = {"converted_image": converted_image, "prompt": prompt}
        return result

@stub.local_entrypoint()
def main():
    import uvicorn
    from fastapi import FastAPI
    
    app = FastAPI()
    model_instance = Model()
    
    @app.post("/inference")
    def inference(request: InferenceRequest):
        return model_instance.inference.remote(request)
    @app.get("/health")
    def health() -> dict[str, str]:
        from .configs import DiffusionConfig
        return {"status": "ok", "device": DiffusionConfig.device}
    uvicorn.run(app, host="0.0.0.0", port=8000)

@stub.function(
    container_idle_timeout=60,
)
@asgi_app()
def app():
    from fastapi import FastAPI
    from fastapi.middleware.cors import CORSMiddleware
    web_app = FastAPI()

    @web_app.post("/inference")
    async def inference(request: InferenceRequest):
        return Model().inference.remote(request)

    @web_app.get("/health")
    async def health():
        from .configs import DiffusionConfig
        return {"status": "ok", "device": DiffusionConfig.device}

    return web_app

if __name__ == "__main__":
    main()