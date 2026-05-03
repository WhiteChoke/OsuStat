from fastapi import APIRouter

router = APIRouter(prefix="/discord/user", tags=["🌟 GET"])

@router.get("/latest")
async def get_latest_statistics():
    return "success"