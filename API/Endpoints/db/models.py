from sqlalchemy.orm import DeclarativeBase, mapped_column, Mapped, relationship
from sqlalchemy import BIGINT, JSON, ForeignKey

class Base(DeclarativeBase):
    pass

class user_latest_info(Base):
    __tablename__ = "user"

    def __repr__(self):
        return f"id: {self.user_id}"

    # Play count,
    # Different maps played,
    # How much pp was gained,
    # AVG bpm played,
    # AVG star rate,
    # AVG acc

    id: Mapped[int] = mapped_column(primary_key=True, nullable=False)
    user_id: Mapped[int] = mapped_column(BIGINT, unique=True)
    play_count: Mapped[int]
    maps_played: Mapped[int]
    raw_pp_gain: Mapped[int]
    avg_bpm: Mapped[int]
    avg_star_rate: Mapped[int]
    avg_accuracy: Mapped[int]

    user_best: Mapped["user_best_map"] = relationship(back_populates="user_latest")

class user_best_map(Base):
    __tablename__ = "best" 
    
    def __repr__(self):
        return f'"map_name": {self.map_name}, "map_chars": {self.map_chars}'

    id: Mapped[int] = mapped_column(primary_key=True, nullable=False)
    user_id: Mapped[int] = mapped_column(BIGINT, ForeignKey("user.user_id"), unique=True)
    map_name: Mapped[str] = mapped_column(nullable=False)

    # 1. AR 
    # 2. CS 
    # 3. HP 
    # 4. length
    # 5. BPM
    # 6. Star Rate
    map_chars: Mapped[dict[str, int | float]] = mapped_column(JSON, nullable=False)
    map_id: Mapped[str] = mapped_column(BIGINT, nullable=False)

    user_latest: Mapped["user_latest_info"] = relationship(back_populates="user_best")
