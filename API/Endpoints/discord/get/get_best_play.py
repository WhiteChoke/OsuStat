from fastapi import APIRouter
from sqlalchemy import select

from Endpoints.db.engine import localSession
from Endpoints.db.models import user_best_map

router = APIRouter(prefix="/discord/user", tags=["🌟 GET"])

@router.get("/latest/best-score")
async def get_latest_best_play():
    stmt = (
        select(user_best_map.id).
        where(user_best_map.id == 1)
    )
    async with localSession() as session:
        result = await session.execute(stmt)
        final = result.scalar()
        print(final)
    return "success"