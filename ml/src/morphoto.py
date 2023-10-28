from PIL import Image

from configs import MorphotoConfig
from text_filter import TextFilter
from image_converter import ImageConverter
from prompt_converter import PromptConverter


class Morphoto:
    def __init__(self, converter_config: MorphotoConfig):
        self.config = converter_config
        self.text_filter = TextFilter(self.config.text_filter_config)
        self.prompt_converter = PromptConverter(self.config.chatgpt_config)
        self.image_converter = ImageConverter(self.config.diffusion_config)

    def convert(self, text: str, image, strength: float = 0.8):
        filterd_text = self.text_filter.filter(text)
        prompt = self.prompt_converter.convert(filterd_text)
        transformed_image = self.image_converter.convert(prompt, image, strength)
        return transformed_image, prompt


if __name__ == "__main__":
    from omegaconf import OmegaConf

    text = "可愛い鴨"
    image_path = "data/sample/poor_duck2.png"
    strength = 0.8
    image = Image.open(image_path)
    morphoto_config = OmegaConf.create(MorphotoConfig)
    morphoto = Morphoto(morphoto_config)
    icon, prompt = morphoto.convert(text, image, strength)
    print(prompt)
    icon.save("result.png")
