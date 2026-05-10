from fastapi import APIRouter
from sqlalchemy import select

from Endpoints.db.engine import localSession
from Endpoints.db.models import user_best_map

router = APIRouter(prefix="/discord/user", tags=["🌟 GET"])

@router.get("/latest/best-score/{id}")
async def get_latest_best_play(id: int):
        stmt = (
            select(user_best_map).
            where(user_best_map.user_id == id)
        )
        async with localSession() as session:
            result = await session.execute(stmt)
            final = result.scalars().all()
            value = final[0]
            obj = {
                  "name": value.map_name,
                  "chars": value.map_chars,
                  "map_id": value.map_id
            }
        return obj