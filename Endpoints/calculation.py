import rosu_pp_py as rosu

from pydantic import BaseModel
from fastapi import APIRouter

router = APIRouter(prefix="/pp-calculate", tags=["🌟 POST"])

class classniy_class(BaseModel):
    filePath: str
    n300: int
    n100: int
    n50: int
    misses: int
    combo: int
    mods: int | None = None

@router.post("/beatmap/")
async def Upload(schema: classniy_class):
    try:
        beatmap = rosu.Beatmap(path=schema.filePath)

        diff = rosu.Difficulty(
            mods = schema.mods,
            ar = beatmap.ar,
            ar_with_mods = True
        )
        
        gradual_diff = diff.calculate(beatmap)
        
        if beatmap.is_suspicious():
            return {"ERROR": "Beatmap is suspicious"}

        perf = rosu.Performance(
            n300=schema.n300,
            n100=schema.n100,
            n50=schema.n50,
            combo=schema.combo,
            misses=schema.misses,
            mods=schema.mods
        )

        attrs = perf.calculate(beatmap)
        perf.set_accuracy(100)
        perf.set_misses(0)
        perf.set_combo(None)
        attrs_max_pp = perf.calculate(beatmap)

        current_pp = round(attrs.pp, 2)
        maximum_pp = round(attrs_max_pp.pp, 2)

        return {
            "STATE": "Success",
            
            "beatmap": {
                "AR": float(f"{beatmap.ar:.1f}"),
                "BPM": float(f"{beatmap.bpm:.1f}"),
                "CS": float(f"{beatmap.cs:.1f}"),
                "HP": float(f"{beatmap.hp:.1f}"),
                "SR": float(f"{gradual_diff.stars:.1f}")
            },
            
            "pp": current_pp,
            "max_pp": maximum_pp
        }
    except Exception as e:
        return {"ERROR": f"The map is fucked: {str(e)}"}