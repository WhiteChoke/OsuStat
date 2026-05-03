from sqlalchemy.orm import DeclarativeBase, mapped_column, Mapped
from sqlalchemy import BIGINT

class Base(DeclarativeBase):
    pass

class user_latest_info(Base):
    __tablename__ = "user"

    # Play count,
    # Different maps played,
    # How much pp was gained,
    # AVG bpm played,
    # AVG star rate,
    # AVG acc

    id: Mapped[int] = mapped_column(BIGINT, primary_key=True, nullable=False)
    play_count: Mapped[int]
    maps_played: Mapped[int]
    raw_pp_gain: Mapped[int]
    avg_bpm: Mapped[int]
    avg_star_rate: Mapped[int]
    avg_accuracy: Mapped[int]

class user_best_map(Base):
    __tablename__ = "best"

    id: Mapped[int] = mapped_column(BIGINT, primary_key=True, nullable=False)
    best_map: Mapped[str] = mapped_column(nullable=None)