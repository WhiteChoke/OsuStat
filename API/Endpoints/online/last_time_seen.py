import os

from pydantic import BaseModel
from fastapi import APIRouter
from datetime import datetime

router = APIRouter(prefix="/online", tags=["🌟 POST"])

# Not all the warriors come to a war

# async def get_score_pos(top_scores):
#     scores = []
#     try:
#         for score in top_scores:
#             if score.pp != None:
#                 scores.append(int(score.pp))
#         return scores
#     except Exception as e:
#          print(f"An error occured {e}")

# async def calculate_pp_gain(current_top_scores, new_score_pp):
#     old_total = sum(pp * (0.95 ** i) for i, pp in enumerate(current_top_scores))
#     new_top_scores = sorted(current_top_scores + [new_score_pp], reverse=True)[:100]
#     new_total = sum(pp * (0.95 ** i) for i, pp in enumerate(new_top_scores))
    
#     return new_total - old_total

months = {
    1: "January",
    2: "February",
    3: "March",
    4: "April",
    5: "May",
    6: "June",
    7: "July",
    8: "August",
    9: "September",
    10: "October",
    11: "November",
    12: "December"
}

class timePath(BaseModel):
      path: str

@router.post("/last-time")
async def user_last_time_seen(Schema: timePath):
        try:
            info = os.stat(Schema.path).st_mtime
            date_val = datetime.fromtimestamp(info)
            date_month = [value for key, value in months.items() if key == date_val.month]
            date = date_val.isoformat(timespec="seconds")
            obj = {
                  "month": date_month[0],
                  "date": date
            }
            return obj
        except Exception as e:
              return e