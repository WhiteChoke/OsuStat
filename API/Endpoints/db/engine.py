from Endpoints.db.config import settings
from sqlalchemy.ext.asyncio import create_async_engine
from sqlalchemy.ext.asyncio import async_sessionmaker

URL = settings.DATABASE_url_asyncpg
engine = create_async_engine(URL, echo=True)
localSession = async_sessionmaker(engine)