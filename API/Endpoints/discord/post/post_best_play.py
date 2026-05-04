from fastapi import APIRouter
from pydantic import BaseModel, Field

from Endpoints.db.engine import localSession
from Endpoints.db.models import user_best_map

router = APIRouter(prefix="/discord/user", tags=["🌟 POST"])

class MapScore(BaseModel):
    user_id: int
    map_chars: dict[str, int | float] = Field(max_length=6)
    map_name: str
    map_id: int

@router.post("/latest/new/best-score")
async def post_latest_best_play(Schema: MapScore):

    obj = user_best_map(
        user_id = Schema.user_id,
        map_name = Schema.map_name,
        map_chars = Schema.map_chars,
        map_id = Schema.map_id
    )

    async with localSession() as session:
        session.add(obj)
        await session.commit()
        
    return "200"