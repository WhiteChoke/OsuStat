import os

from osu import Client
from dotenv import load_dotenv

load_dotenv()

CLIENT_SECRET = os.getenv("CLIENT_SECRET")
CLIENT_ID = os.getenv("CLIENT_ID")

redirect_url = "http://127.0.0.1:8080"
client = Client.from_credentials(CLIENT_ID, CLIENT_SECRET, redirect_url)