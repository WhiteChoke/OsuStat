import rosu_pp_py as rosu

from osu import UserScoreType

from pydantic import BaseModel
from fastapi import APIRouter
from datetime import datetime

from osu_manager import client

client.set_api_version("20220704")

router = APIRouter(prefix="/statistics", tags=["🌟 POST"])

async def get_score_pos(top_scores):
    scores = []
    try:
        for score in top_scores:
            if score.pp != None:
                scores.append(int(score.pp))
        return scores
    except Exception as e:
         print(f"An error occured {e}")

# Calculates output pp value based on user's 100 previous top scores ( 1 step before ),
# then applying new user score to the current top 100 and reversing it.
# At the end applying step 1 to with new_total and calculating the final value "new_total - old_total"

async def calculate_pp_gain(current_top_scores, new_score_pp):
    old_total = sum(pp * (0.95 ** i) for i, pp in enumerate(current_top_scores))
    new_top_scores = sorted(current_top_scores + [new_score_pp], reverse=True)[:100]
    new_total = sum(pp * (0.95 ** i) for i, pp in enumerate(new_top_scores))
    
    return new_total - old_total

class Stat(BaseModel):
      score_id: int
      user_id: int 

@router.post("/user")
async def get_user_data(Schema: Stat):

        top_scores = client.get_user_scores(Schema.user_id, UserScoreType.BEST, limit=100)

        obj = client.get_score_by_id_only(Schema.score_id)
        score_pp = int(obj.pp)
        value = await get_score_pos(top_scores)
        pp_gained = await calculate_pp_gain(value, score_pp)

        user = client.get_user(Schema.user_id)
        time = str(user.last_visit)
        converted_time = datetime.fromisoformat(time)
        last_time_logged_days = int(converted_time.strftime("%d"))
        last_time_logged_time = converted_time.strftime("%H:%M:%S")

        user = {
                "pp_gained": int(pp_gained),
                "last_seen": {
                    "daysAgo": last_time_logged_days,
                    "timeAgo": last_time_logged_time
                }
            }

        return user