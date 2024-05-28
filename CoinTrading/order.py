import os
import sqlite3

base_dir = os.path.dirname(os.path.abspath(__file__))
db_path = os.path.join(base_dir, 'Data', 'data.db')

# Skapa eller anslut till en SQLite-databas
conn = sqlite3.connect(db_path)
c = conn.cursor()

c.execute('''CREATE TABLE IF NOT EXISTS positions
        (
            id        INTEGER  PRIMARY KEY AUTOINCREMENT,
            userid    INTEGER, 
            symbol    TEXT, 
            is_closed INTEGER  CHECK (is_closed IN (0, 1)) DEFAULT 0,
            timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
        )''')
conn.commit()


c.execute('''CREATE TABLE IF NOT EXISTS orders
        (
            id           INTEGER    PRIMARY KEY AUTOINCREMENT,
            positionid   INTEGER, 
            open_price   REAL       DEFAULT 0.00, 
            close_price  REAL       DEFAULT 0.00,
            leverage     INTEGER,
            type         TEXT,
            side         TEXT,
            timestamp    DATETIME   DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY(positionid) REFERENCES positions(id)
        )''')
conn.commit()


""" c.execute("INSERT INTO positions (symbol) VALUES ('BTCUSDT')")
conn.commit()


import random

for x in range(6):
    c.execute("INSERT INTO orders (positionid, open_price, leverage, type, side) VALUES (?, ?, ?, ?, ?)", 
                      (1, random.randrange(45000, 85000), random.randrange(1, 10), 'market', 'buy'))
    conn.commit()

c.execute("INSERT INTO orders (positionid, close_price, leverage, type, side, is_closed) VALUES (?, ?, ?, ?, ?, ?)", 
                      (1, random.randrange(45000, 85000), random.randrange(1, 10), 'market', 'sell', 1))
conn.commit() """

# SELECT * FROM orders LEFT JOIN positions ON positions.id = orders.id ORDER BY orders.close_price;