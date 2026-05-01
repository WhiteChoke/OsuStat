import uvicorn

from fastapi import FastAPI

from offline import calculation
from online import last_time_seen, pp_recieved

app = FastAPI()

app.include_router(calculation.router)
app.include_router(last_time_seen.router)
app.include_router(pp_recieved.router)

if __name__ == "__main__":
    uvicorn.run(app, host="127.0.0.1", port=7272, log_config=None)