from fastapi import APIRouter, Depends
from pydantic import BaseModel

from typing import Annotated

from sqlalchemy.orm import Session
from sqlalchemy import insert

from Endpoints.db.engine import localSession
from Endpoints.db.models import user_latest_info


router = APIRouter(prefix="/discord/user", tags=["🌟 POST"])

    # Play count,
    # Different maps played,
    # How much pp was gained,
    # AVG bpm played,
    # AVG star rate,
    # AVG acc

class insert_data(BaseModel):
    user_id: int
    play_count: int
    maps_played: int
    raw_pp_gain: int
    avg_bpm: int
    avg_star_rate: int
    avg_accuracy: int

@router.post("/new/latest")
async def post_latest_statistics(Schema: insert_data):
    async with localSession() as session:
        user = user_latest_info(
            user_id = Schema.user_id,
            play_count = Schema.play_count,
            maps_played = Schema.maps_played,
            raw_pp_gain = Schema.raw_pp_gain,
            avg_bpm = Schema.avg_bpm,
            avg_star_rate = Schema.avg_star_rate,
            avg_accuracy = Schema.avg_accuracy
        )
        session.add(user)
        await session.commit()
    return "200"