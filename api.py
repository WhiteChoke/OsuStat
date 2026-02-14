import uvicorn

from fastapi import FastAPI
from Endpoints import calculation

app = FastAPI()

app.include_router(calculation.router)

if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=727)