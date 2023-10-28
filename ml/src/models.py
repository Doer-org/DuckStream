from typing import Optional

from pydantic import BaseModel


class InferenceRequest(BaseModel):
    image: str
    prompt: str
    strength: float
    is_mock: Optional[bool]
