import rosu_pp_py as rosu

from fastapi import APIRouter, UploadFile
from pydantic import BaseModel, Field

class Item(BaseModel):
    n300: int
    n100: int
    n50: int
    misses: int
    combo: int
    mods: list[int] | int | None = None

router = APIRouter(prefix="/pp-calculate", tags=["🌟 POST"])

@router.post("/beatmap/")
async def Upload(item: Item):
    try:
        # temprorary path, should be used an actual file
        path = r"D:\Programming\OsuStat\Release Hallucination - Chronostasis (P A N) [Extra].osu"
        beatmap = rosu.Beatmap(path=path)
        
        # beatmap.convert(rosu.GameMode.Osu) # No need in it for now, fuck the file
        if beatmap.is_suspicious():
            return {"error": "Beatmap is suspicious"}
        
        # Calculates current ammount of pp
        perf = rosu.Performance(
            n300 = item.n300,
            n100 = item.n100,
            n50 = item.n50,
            combo = item.combo,
            misses = item.misses,
            mods = item.mods
        )
        
        attrs = perf.calculate(beatmap)
        
        # Calculates MAX ammount of pp
        
        perf.set_accuracy(100)
        perf.set_misses(None)
        perf.set_combo(None)
        
        attrs_max_pp = perf.calculate(attrs)
        
        
        # Rounds the ammount of pp so it's not Long type
        current_pp = round(attrs.pp, 2)
        maximum_pp = round(attrs_max_pp.pp, 2)
        
        
        return {"message": "Data received", "item": item, "PP": current_pp, "MAX_pp": maximum_pp}
    
    except FileNotFoundError as e:
        return {"ERROR": f"Beatmap file not found: {str(e)}"}
    
    except Exception as e:
        return {"ERROR": f"The map is fucked: {str(e)}"}
    
    # dk if this thing will actually be needed here
    # file = upload.file
    # filename = upload.filename
    
    # with open(f'{filename}', 'wb') as f:
    #     f.write(file.read())