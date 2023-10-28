import torch
from diffusers import StableDiffusionImg2ImgPipeline


def download_model(model_id: str, save_path: str) -> None:
    pipe = StableDiffusionImg2ImgPipeline.from_pretrained(model_id, torch_dtype=torch.float16)
    pipe.save_pretrained(save_path)


if __name__ == "__main__":
    model_id = "runwayml/stable-diffusion-v1-5"
    save_path = "data/stable-diffusion-v1-5"
    download_model(model_id, save_path)
