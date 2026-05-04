from fastapi import APIRouter
from sqlalchemy import select

from Endpoints.db.engine import localSession
from Endpoints.db.models import user_latest_info

router = APIRouter(prefix="/discord/user", tags=["🌟 GET"])

@router.get("/latest/{id}")
async def get_latest_statistics(id: int):
    async with localSession() as session:
        stmt = (
            select(user_latest_info).
            where(user_latest_info.user_id == id)
        )
        result = await session.execute(stmt)
        value = result.scalars().all()
        final = value[0]

        obj = {
            "user_id": final.user_id,
            "playcount": final.play_count,
            "maps_played": final.maps_played,
            "raw_pp": final.raw_pp_gain,
            "average": {
                "bpm": final.avg_bpm,
                "sr": final.avg_star_rate,
                "acc": final.avg_accuracy
            }
        }

    return obj