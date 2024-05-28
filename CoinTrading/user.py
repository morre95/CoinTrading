import os
import time
import sqlite3
from datetime import datetime

base_dir = os.path.dirname(os.path.abspath(__file__))
db_path = os.path.join(base_dir, 'Data', 'data.db')

# Skapa eller anslut till en SQLite-databas
conn = sqlite3.connect(db_path)
c = conn.cursor()


#c.execute('DROP TABLE users')
#conn.commit()

# Skapa en tabell för användare om den inte redan finns
c.execute('''CREATE TABLE IF NOT EXISTS users (
             id INTEGER PRIMARY KEY AUTOINCREMENT, 
             username TEXT, 
             password TEXT,
             email TEXT, 
             token TEXT,
             balance REAL,
             timestamp DATETIME DEFAULT CURRENT_TIMESTAMP)''')
conn.commit()

def add_user(username, password, email):
    c.execute("INSERT INTO users (username, password, email) VALUES (?, ?, ?)", (username, password, email))
    conn.commit()

# Lägg till en ny användare varje sekund
try:
    pass
   # while True:
        #timestamp = datetime.now().strftime("%Y%m%d%H%M%S")
       # username = f"user_{timestamp}"
       # password = "password123"  # Du kan generera eller få ett lösenord på ett mer dynamiskt sätt
       # email = f"{username}@example.com"
       # add_user(username, password, email)
       # print(f"Lagt till användare: {username}")
       # time.sleep(2)
except KeyboardInterrupt:
    pass
finally:
    conn.close()
