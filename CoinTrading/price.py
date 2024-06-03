import os
import asyncio
import websockets
import json
import sqlite3
import random

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


class Manager:
    randomOffset = 0
    def __init__(self):
        self.url = "wss://stream.binance.com:9443/ws"
        self.max_retries = 5
        self.retry_delay = 5

    async def start(self):
        retries = 0
        while retries < self.max_retries:
            try:
                async with websockets.connect(self.url) as websocket:
                    await self.subscribe(websocket)
                    await self.listen(websocket)
            except websockets.ConnectionClosed:
                retries += 1
                print(f"Connection lost, trying to reconnect ({retries}/{self.max_retries})...")
                await asyncio.sleep(self.retry_delay)
            else:
                retries = 0

    async def subscribe(self, websocket):
        info = {"method": "SUBSCRIBE", "params": ["btcusdt@miniTicker"], "id": 1}
        message = json.dumps(info)
        await websocket.send(message)
    
    async def listen(self, websocket):
        async for message in websocket:
            self.handle_text_message_received(message)
    
    # TBD: Det kan vara bra att l�gga till en ping h�r. Tror inte det �r n�gon fara dock n�r vi k�r med btc och �nnu mindre om vi skulle bpolande in flera mynt. 
    # Men om det inte kommer n�gon uppdatering p� 10 minuter nu s� kommer binence atomatiskt avsluta prenumerationen om vi inte k�r en ping innan 10 minuters preioden g�tt ut
    def handle_text_message_received(self, message):
        data = json.loads(message)
        if 'result' in data and data['result'] == None:
            print(data)
        else:
            close_price = data['c']
            open_price = data['o']
            high_price = data['h']
            low_price = data['l']
            symbol = data['s']

            # RANDOM CHEAT TO MAKE GRAPH MORE INTERESTING DURING LOW HOURS
            scale = 0.001;
            close_price = float(close_price)
            self.randomOffset += (random.random()-0.5)*pow(close_price*scale, 1/2).real
            close_price += self.randomOffset

            print(f"{symbol.upper()} - Close: {close_price} USD, Open: {open_price} USD, High: {high_price} USD, Low: {low_price} USD")
            # Spara priserna i databasen
            c.execute("INSERT INTO prices (symbol, open_price, close_price, high_price, low_price) VALUES (?, ?, ?, ?, ?)", 
                    (symbol.upper(), open_price, close_price, high_price, low_price))
            conn.commit()

            c.execute("DELETE FROM prices WHERE timestamp < DATETIME('now', '-400 seconds')")
            conn.commit()


def main():
    manager = Manager()
    asyncio.run(manager.start())

if __name__ == "__main__":
    main()

""" async def get_binance_price(symbol):
    uri = f"wss://stream.binance.com:9443/ws/{symbol.lower()}@miniTicker"

    async with websockets.connect(uri) as websocket:
        while True:
            try:
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
            except:
                get_binance_price(symbol)

if __name__ == "__main__":
    symbol = "btcusdt"
    try:
        asyncio.get_event_loop().run_until_complete(get_binance_price(symbol))
    except KeyboardInterrupt:
        pass
    finally:
        conn.close() """
