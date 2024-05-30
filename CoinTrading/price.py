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

# Skapa en tabell för btcusdt om den inte redan finns
c.execute('''CREATE TABLE IF NOT EXISTS prices
             (id INTEGER PRIMARY KEY AUTOINCREMENT,
              symbol TEXT, 
              open_price REAL, 
              close_price REAL, 
              high_price REAL, 
              low_price REAL, 
              timestamp DATETIME DEFAULT CURRENT_TIMESTAMP)''')
conn.commit()

# Skapa en tabell för andra priser om den inte redan finns
c.execute('''CREATE TABLE IF NOT EXISTS other_prices
             (id INTEGER PRIMARY KEY AUTOINCREMENT,
              symbol TEXT, 
              price REAL, 
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

async def get_other_prices(symbol):
    uri = f"wss://stream.binance.com:9443/ws/{symbol.lower()}@trade"

    async with websockets.connect(uri) as websocket:
        while True:
            response = await websocket.recv()
            data = json.loads(response)
            price = float(data['p'])
            print(f"Trade executed [{symbol.upper()}] price: {price}")

            # Spara priset i databasen
            c.execute("INSERT INTO other_prices (symbol, price) VALUES (?, ?)", 
                      (symbol.upper(), price))
            conn.commit()

            c.execute("DELETE FROM other_prices WHERE timestamp < DATETIME('now', '-400 seconds')")
            conn.commit()

            await asyncio.sleep(1)

async def main():
    symbol = "btcusdt"
    symbols = [
        "ethbtc", "bnbbtc", "kcsbtc", "crobtc", 
        "adabtc", "renbtc", "dotbtc", "unibtc", 
        "linkbtc", "linkusdt", "adausdt", "uniusdt"
    ]

    btc_task = asyncio.create_task(get_binance_price(symbol))
    other_tasks = [asyncio.create_task(get_other_prices(sym)) for sym in symbols]

    await asyncio.gather(btc_task, *other_tasks)

if __name__ == "__main__":
    try:
        asyncio.get_event_loop().run_until_complete(main())
    except KeyboardInterrupt:
        pass
    finally:
        conn.close()
