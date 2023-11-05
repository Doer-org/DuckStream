import os

import torch
from diffusers import StableDiffusionImg2ImgPipeline
from dotenv import load_dotenv
from PIL import Image, ImageFilter, ImageChops

from .configs import DiffusionConfig


class ImageConverter:
    def __init__(self, diffusion_config: DiffusionConfig):
        self.config = diffusion_config
        self.device = torch.device(self.config.device)
        self.model, self.generator = self._inference_setup()

    def _inference_setup(self):
        load_dotenv()
        token = os.environ.get("HUGGING_FACE_TOKEN")
        model = StableDiffusionImg2ImgPipeline.from_pretrained(self.config.model, torch_dtype=torch.float16, revision="fp16", use_auth_token=token)
        model = model.to(self.device)
        model.safety_checker = lambda images, **kwargs: (images, [False])
        generator = torch.Generator(device=self.device).manual_seed(self.config.seed)
        return model, generator

    def resize_image(self, image):
        width, height = image.size

        if width > height:
            ratio = self.config.max_size / width
            new_width = self.config.max_size
            new_height = int(height * ratio)
        else:
            ratio = self.config.max_size / height
            new_height = self.config.max_size
            new_width = int(width * ratio)
        image = image.resize((new_width, new_height))
        return image

    def dilate_line(self, image):
        binary_image = image.point(lambda p: 255 if p > self.config.threshold else 0)
        for _ in range(self.config.dilation_num):
            binary_image = binary_image.filter(ImageFilter.MinFilter(self.config.kernel_size))
        return binary_image
    
    def crop_image(self, image):
        bg = Image.new(image.mode, image.size, "white")
        diff = ImageChops.difference(image, bg)
        bbox = diff.getbbox()
        
        if not bbox:
            return image
        x1, y1, x2, y2 = bbox
        x1 = max(0, x1 - self.config.padding)
        y1 = max(0, y1 - self.config.padding)
        x2 = min(image.width, x2 + self.config.padding)
        y2 = min(image.height, y2 + self.config.padding)
        cropped_image = image.crop((x1, y1, x2, y2))
        return cropped_image
        
    def replace_bg(self, image):
        new_image = Image.new("RGBA", image.size, (255, 255, 255, 255))
        new_image.paste(image, mask=image.split()[3]) 
        return new_image
    def convert(self, prompt: str, image, strength: float = 0.8):
        image = self.replace_bg(image)
        image = image.convert("RGB")
        image = self.resize_image(image)
        image = self.model(
            prompt=prompt,
            image=image,
            strength=strength,
            generator=self.generator,
        ).images[0]
        return image


if __name__ == "__main__":
    from omegaconf import OmegaConf

    diffusion_config = OmegaConf.create(DiffusionConfig)
    prompt = "best quality masterpiece makoto shinkai duck"
    image_path = "data/sample/poor_duck3.png"
    image = Image.open(image_path)
    converter = ImageConverter(diffusion_config)
    image = converter.convert(prompt, image)
    image.save("result.png")
