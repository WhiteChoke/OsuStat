import rosu_pp_py as rosu
from pydantic import BaseModel
from fastapi import APIRouter

router = APIRouter(prefix="/beatmap", tags=["🌟 POST"])

class classniy_class(BaseModel):
    filePath: str
    n300: int
    n100: int
    n50: int
    misses: int
    combo: int
    mods: int | None = None

@router.post("/result")
async def Upload(ClassSchema: classniy_class):
    try:
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

        obj = {
            "STATE": "Success",
            
            "beatmap": {
                "AR": float(f"{beatmap.ar:.1f}"),
                "BPM": float(f"{beatmap.bpm:.1f}"),
                "CS": float(f"{beatmap.cs:.1f}"),
                "HP": float(f"{beatmap.hp:.1f}"),
                "OD": float(f"{beatmap.od:.1f}"),
                "SR": float(f"{gradual_diff.stars:.1f}"),
                "stat": {
                    "acc": accuracy,
                    "max_pp": maximum_pp
                }
            }
        }
        return obj
    except Exception as e:
        return {"ERROR": f"The map is fucked: {str(e)}"}