import requests

headers = { 'User-Agent': 'Mozilla/5.0 (Windows NT 6.0; WOW64; rv:24.0) Gecko/20100101 Firefox/24.0' }
res = requests.post("https://csszl.webtrax.hu/app/index.php?ZS2VPY8UKH=kdFg7w3M_d$4&NK8DOL3ERH=null", data = {"action": "selector", "mode": "dictionary"}, headers=headers)

if res.status_code == 200:
    letters = res.json()

karomkodasok = []
for letter in letters:
    print(letter)
    res = requests.post("https://csszl.webtrax.hu/app/index.php?ZS2VPY8UKH=kdFg7w3M_d$4&NK8DOL3ERH=null", data = {"action": "dictionary", "keyword": letter}, headers=headers)
    if res.status_code == 200:
        betuKaromkodasok = res.json()
        for k in betuKaromkodasok:
            karomkodasok.append(k["value"])
           
with open("karomkodasok.txt", "w", encoding="utf-8") as file:
    file.write("{ ")
    for karomkodas in karomkodasok:
        file.write(f"\"{karomkodas}\", ")
    file.write("}\n")