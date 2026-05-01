from fastapi import APIRouter
from pydantic import BaseModel
from osu_manager import client

client.set_api_version("20220704")

router = APIRouter(prefix="/online", tags=["🌟 POST"])

class kakoy_class(BaseModel):
    user_id: int

@router.post("/pp_recieved")
async def getPP(Schema: kakoy_class):
    user = client.get_user(Schema.user_id)
    user_pp = int(user.statistics.pp)
    return user_pp