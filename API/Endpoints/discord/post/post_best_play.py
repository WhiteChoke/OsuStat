from fastapi import APIRouter
from pydantic import BaseModel

from Endpoints.db.engine import localSession
from Endpoints.db.models import user_best_map

router = APIRouter(prefix="/discord/user", tags=["🌟 POST"])

class MapScore(BaseModel):
    best_map: str

@router.post("/latest/new/best-score")
async def post_latest_best_play(Schema: MapScore):

    obj = user_best_map(
        best_map = Schema.best_map
    )

    async with localSession() as session:
        session.add(obj)
        await session.commit()
        
    return "200"