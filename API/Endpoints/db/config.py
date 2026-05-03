from pydantic_settings import SettingsConfigDict, BaseSettings

class Settings(BaseSettings):
    DB_NAME: str
    DB_PASS: str
    DB_HOST: str
    DB_PORT: int
    DB_USER: str

    @property
    def DATABASE_url_asyncpg(self):
        return f"postgresql+asyncpg://{self.DB_USER}:{self.DB_PASS}@{self.DB_HOST}:{self.DB_PORT}/{self.DB_NAME}"
    
    model_config = SettingsConfigDict(env_file=".env", extra="ignore")

settings = Settings()
print(settings.DATABASE_url_asyncpg)