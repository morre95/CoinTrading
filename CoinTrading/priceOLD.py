import os
import asyncio
import websockets
import json
import sqlite3

base_dir = os.path.dirname(os.path.abspath(__file__))
db_path = os.path.join(base_dir, 'Data', 'data.db')
print(db_path)
# Skapa eller anslut till en SQLite-databas
conn = sqlite3.connect(db_path)
c = conn.cursor()


#c.execute('DROP TABLE prices')
conn.commit()

# Skapa en tabell om den inte redan finns
c.execute('''CREATE TABLE IF NOT EXISTS prices
             (id INTEGER PRIMARY KEY AUTOINCREMENT,
              symbol TEXT, 
              open_price REAL, 
              close_price REAL, 
              high_price REAL, 
              low_price REAL, 
              timestamp DATETIME DEFAULT CURRENT_TIMESTAMP)''')
conn.commit()

async def get_binance_price(symbol):
    uri = f"wss://stream.binance.com:9443/ws/{symbol.lower()}@miniTicker"

    async with websockets.connect(uri) as websocket:
        while True:
            response = await websocket.recv()
            data = json.loads(response)
            close_price = data['c']
            open_price = data['o']
            high_price = data['h']
            low_price = data['l']
            print(f"{symbol.upper()} - Close: {close_price} USD, Open: {open_price} USD, High: {high_price} USD, Low: {low_price} USD")

            # Spara priserna i databasen
            c.execute("INSERT INTO prices (symbol, open_price, close_price, high_price, low_price) VALUES (?, ?, ?, ?, ?)", 
                      (symbol.upper(), open_price, close_price, high_price, low_price))
            conn.commit()

            c.execute("DELETE FROM prices WHERE timestamp < DATETIME('now', '-400 seconds')")
            conn.commit()

            await asyncio.sleep(1)

if __name__ == "__main__":
    symbol = "btcusdt"
    try:
        asyncio.get_event_loop().run_until_complete(get_binance_price(symbol))
    except KeyboardInterrupt:
        pass
    finally:
        conn.close()
