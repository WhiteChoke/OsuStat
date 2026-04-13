import uvicorn

from fastapi import FastAPI
import calculation

app = FastAPI()

app.include_router(calculation.router)

if __name__ == "__main__":
    uvicorn.run(app, host="127.0.0.1", port=7272, log_config=None)