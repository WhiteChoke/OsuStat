import rosu_pp_py as rosu

from osu import UserScoreType

from pydantic import BaseModel
from fastapi import APIRouter
from datetime import datetime

from Endpoints.osu_manager import client

client.set_api_version("20220704")

router = APIRouter(prefix="/pp-calculate", tags=["🌟 POST"])

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

class classniy_class(BaseModel):
    filePath: str
    n300: int
    n100: int
    n50: int
    misses: int
    combo: int
    mods: int | None = None

class Stat(BaseModel):
      score_id: int
      user_id: int 

@router.post("/beatmap/")
async def Upload(ClassSchema: classniy_class, BaseSchema: Stat):
    try:
        top_scores = client.get_user_scores(BaseSchema.user_id, UserScoreType.BEST, limit=100)
        beatmap = rosu.Beatmap(path=ClassSchema.filePath)

        diff = rosu.Difficulty(
            mods = ClassSchema.mods,
            ar = beatmap.ar,
            cs = beatmap.cs,
            hp = beatmap.hp,
            od = beatmap.od,
            lazer = False
        )
        
        gradual_diff = diff.calculate(beatmap)
        
        if beatmap.is_suspicious():
            return {"ERROR": "Beatmap is suspicious"}

        perf = rosu.Performance(
            n300=ClassSchema.n300,
            n100=ClassSchema.n100,
            n50=ClassSchema.n50,
            combo=ClassSchema.combo,
            misses=ClassSchema.misses,
            mods=ClassSchema.mods
        )

        perf.set_accuracy(100)
        perf.set_misses(0)
        perf.set_combo(None)
        attrs_max_pp = perf.calculate(beatmap)

        maximum_pp = round(attrs_max_pp.pp, 2)
        
        total_objects = ClassSchema.n300 + ClassSchema.n100 + ClassSchema.n50 + ClassSchema.misses

        if total_objects > 0:
            accuracy = round((ClassSchema.n300 * 300 + ClassSchema.n100 * 100 + ClassSchema.n50 * 50) / (300 * total_objects) * 100, 2)
        else:
            accuracy = 0.0

        obj = client.get_score_by_id_only(Stat.score_id)
        score_pp = int(obj.pp)
        value = await get_score_pos(top_scores)
        pp_gained = await calculate_pp_gain(value, score_pp)

        user = client.get_user(BaseSchema.user_id)
        time = str(user.last_visit)
        converted_time = datetime.fromisoformat(time)
        last_time_logged_days = int(converted_time.strftime("%d"))
        last_time_logged_time = converted_time.strftime("%H:%M:%S")

        obj = {
            "STATE": "Success",
            
            "beatmap": {
                "AR": float(f"{beatmap.ar:.1f}"),
                "BPM": float(f"{beatmap.bpm:.1f}"),
                "CS": float(f"{beatmap.cs:.1f}"),
                "HP": float(f"{beatmap.hp:.1f}"),
                "OD": float(f"{beatmap.od:.1f}"),
                "SR": float(f"{gradual_diff.stars:.1f}")
            },
            "user": {
                "acc": accuracy,
                "pp_gained": pp_gained,
                "max_pp": maximum_pp,
                "last_seen": {
                    "daysAgo": last_time_logged_days,
                    "timeAgo": last_time_logged_time
                }
            }
        }

        return obj
    except Exception as e:
        return {"ERROR": f"The map is fucked: {str(e)}"}