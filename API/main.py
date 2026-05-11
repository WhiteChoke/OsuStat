import uvicorn
import asyncio

from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from Endpoints.offline import calculation
from Endpoints.online import last_time_seen, pp_recieved


app = FastAPI()

# app.add_middleware(
#     CORSMiddleware,
#     origins=["*"]
# )

app.include_router(calculation.router)
app.include_router(last_time_seen.router)
app.include_router(pp_recieved.router)

# @app.on_event("startup")
# async def on_startup():
#     async with engine.begin() as conn:
#         await conn.run_sync(Base.metadata.drop_all)
#         await conn.run_sync(Base.metadata.create_all)

if __name__ == "__main__":
    uvicorn.run(app, host="127.0.0.1", port=7272, log_config=None)