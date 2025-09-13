# 🐇 Dash and Collect

A Unity-based game where the player must **collect all the rabbits** 🐇 while **avoiding ghosts** 👻 within a given time limit. The player starts with **3 health points** and loses 1 health each time they touch a ghost.  
- If health reaches 0 → **Game Over**  
- If the timer runs out → **Game Over**  
- If all rabbits are collected before time runs out → **You Win!**

---
## Demo APP Video
- [Demo Video](https://drive.google.com/file/d/17waOgiOQ4HfRzhfH7gRU1Xf3PUhB9gLl/view?usp=sharing)

---

## 🎮 Features
- Player movement & controls  
- Rabbit collectibles  
- Ghost enemies (patrol/chase/attack)
- Health system (3 lives)  
- Countdown timer  
- Win/Lose conditions  
- Account and leaderboard integration with **Nakama**

---

## 🛠 Tech Stack
- [Unity](https://unity.com/) (Editor Version 2022.3.7f1 LTS)  
- [Nakama](https://heroiclabs.com/nakama/) (Game server v3.22)  
- [CockroachDB](https://www.cockroachlabs.com/) (Database v23.1)  
- [Docker](https://www.docker.com/)  

---

## Player Movement 
- **WASD** - Movement
- **F** - Collect Item 
- **E** - Dash 
- **Space** - Jump

## Enemy State 
- **Patrol**: Moves randomly in 360° directions around the map. 
- **Chasing**: When the player is spotted, the enemy will chase until an obstacle blocks the line of sight.  
- **Attack**: On collision with the player, the enemy deals damage (reduces health).

## Nakama Data Flow
```text
Unity (client)
  ── Authenticate (email/device) ──▶ Nakama
  ── SubmitRun RPC(payload) ───────▶ Nakama RPC Module
                                        ├─ validate payload
                                        └─ write leaderboard record (authoritative)
  ◀─────────────────────────────────── confirms success
  ── ListLeaderboardRecordsAsync ────▶ Nakama (get top N)
  ◀─────────────────────────────────── returns records with username & metadata
```

## Assets Used 
- [Nakama Unity v3.17.0](https://github.com/heroiclabs/nakama-unity) (Nakama Integration Unity)
- [Haon SD Series Pack](https://assetstore.unity.com/packages/3d/characters/creatures/haon-sd-creature-pack-311173) (Player and Enemy Assets)
- [50 Free PBR Materials](https://assetstore.unity.com/packages/2d/textures-materials/50-free-pbr-materials-242760) (Wall and floor materials)
- [Horrific Music And SFX Free](https://assetstore.unity.com/packages/audio/music/orchestral/horrific-music-and-sfx-free-71006) (BGM & SFX)
- [Horror SFX - 082918](https://assetstore.unity.com/packages/audio/sound-fx/horror-sfx-082918-127389) (SFX)

## 🚀 Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/jearsevan101/Dash-and-Collect.git
cd Dash-and-Collect 
```


### 2. Install Docker
1. Check if Docker is installed
```docker --version```

2. If not installed, download Docker from:
 ```https://docs.docker.com/get-docker/```


### 3. Run Docker Compose
1. Open Terminal 
2. cd to "nakama" folder where docker-compose.yml placed
3. run ```docker-compose up```


### 4. Open the Unity 
1. Open Unity hub 
2. Open Project from disk
3. Select the project folder 
4. Open the project

### 5. Play the Game
1. Register using email, password, and username (you can use dummy data)
2. Have fun


