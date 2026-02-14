import uvicorn

from fastapi import FastAPI
from Endpoints import calculation

app = FastAPI()

app.include_router(calculation.router)

if __name__ == "__main__":
    uvicorn.run("api:app", port=727)