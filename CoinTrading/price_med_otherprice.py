import os
import asyncio
import aiohttp
import sqlite3
import datetime

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

async def fetch_prices():
    url = 'https://api.binance.com/api/v3/ticker/24hr'
    async with aiohttp.ClientSession() as session:
        while True:
            async with session.get(url) as response:
                data = await response.json()
                for item in data:
                    symbol = item['symbol']
                    open_price = item['openPrice']
                    close_price = item['lastPrice']
                    high_price = item['highPrice']
                    low_price = item['lowPrice']
                    
                    # Spara priserna i databasen för BTCUSDT
                    if symbol == "BTCUSDT":
                        print(f"{symbol} - Close: {close_price} USD, Open: {open_price} USD, High: {high_price} USD, Low: {low_price} USD")
                        c.execute("INSERT INTO prices (symbol, open_price, close_price, high_price, low_price) VALUES (?, ?, ?, ?, ?)", 
                                  (symbol, open_price, close_price, high_price, low_price))
                        conn.commit()
                        c.execute("DELETE FROM prices WHERE timestamp < DATETIME('now', '-400 seconds')")
                        conn.commit()
                    
                    # Spara priserna i databasen för andra symboler
                    else:
                        price = float(close_price)
                        print(f"Trade executed [{symbol}] price: {price}")
                        c.execute("INSERT INTO other_prices (symbol, price) VALUES (?, ?)", 
                                  (symbol, price))
                        conn.commit()
                        c.execute("DELETE FROM other_prices WHERE timestamp < DATETIME('now', '-400 seconds')")
                        conn.commit()

            await asyncio.sleep(1)

async def main():
    await fetch_prices()

if __name__ == "__main__":
    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        pass
    finally:
        conn.close()
