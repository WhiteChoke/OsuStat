import rosu_pp_py as rosu
from pydantic import BaseModel
from fastapi import APIRouter

router = APIRouter(prefix="/offline", tags=["🌟 POST"])

class classniy_class(BaseModel):
    filePath: str
    n300: int
    n100: int
    n50: int
    misses: int
    combo: int
    mods: int | None = None

@router.post("/result")
async def UserStats(ClassSchema: classniy_class):
    try:
        beatmap = rosu.Beatmap(path=ClassSchema.filePath)

        calculate_diff = rosu.Difficulty(
            mods = ClassSchema.mods,
            ar = beatmap.ar,
            cs = beatmap.cs,
            hp = beatmap.hp,
            od = beatmap.od,
            lazer = False
        )
        
        diff = calculate_diff.calculate(beatmap)
        
        if beatmap.is_suspicious():
            return {"ERROR": "Beatmap is suspicious"}

        perf = rosu.Performance(
            n300=ClassSchema.n300,
            n100=ClassSchema.n100,
            n50=ClassSchema.n50,
            combo=ClassSchema.combo,
            misses=ClassSchema.misses,
            mods=ClassSchema.mods,
            lazer=False
        )
        
        receive_pp = perf.calculate(beatmap)
        perf.set_accuracy(100)
        perf.set_misses(0)
        perf.set_combo(None)
        max_pp = perf.calculate(beatmap)

        max = round(max_pp.pp, 2)

        obj = {
            "gained": receive_pp,
            "max_pp": max,
            "star_rate": diff.stars
        }
        return obj
    
    except Exception as e:
        return {"ERROR": f"An error occured during the map being proceeded: {str(e)}"}