# SpaceInvaders

# How to Play:
- A/D to move the ship horizontally
- Spacebar to fire bullets
# Win condition:
- destroy all invaders
# Lose condition:
- an invader reach the bottom edge of the screen.

# Addressable:
- resources stores in Assets/Bundles/...
- support hotfix by rebuild ad redistribute adreessable

# Data Definitions CSV files stored in folder "Assets/Bundles/CSV"
- link to sheet: https://docs.google.com/spreadsheets/d/1Le4jAmQ4ETGi7tmYy2YpP3_WHF2NUBKQOVQO90fYIMU/edit?usp=sharing
- Ships.csv: content definitions for ships. Can be generate from tab Ships
- Bullets.csv:  content definitions for bullets. Can be generate from tab Bullets
- Aliens.csv: content definitions for aliens. Can be generate from tab Aliens
- Some runtime configs can be edit in prefab "BattleGround.prefab"

# How to build new ship skins:
- Add new prepared prefabs for new ships
- Add new records in above sheet, tab Ships (with SkinPath fiedl is ship-prefab's adrressable path)
- export Ships.csv and replace the original file "Assets/Bundles/CSV/Ships.csv"
- currently ships will be pick randomly every battle start.

# How to build new bullets type:
- Add new prepared prefabs for new bullets
- Add new records in above sheet, tab Bullet (with SkinPath fiedl is bullet-prefab's adrressable path)
- export Bullets.csv and replace the original file "Assets/Bundles/CSV/Bullets.csv"
- currently bullets will be spawn randomly from ships.
- currently bullets have a simple behaviour so only supporting skins variations for now. 


# How to build new bullets type:
- Add new prepared prefabs for new aliens
- Add new records in above sheet, tab Aliens (with SkinPath fiedl is alien-prefab's adrressable path)
- export Aliens.csv and replace the original file "Assets/Bundles/CSV/Aliens.csv"
- currently aliens will be spawn randomly at the start of battle.
- currently aliens have a simple behaviour so only supporting skins variations for now. 


# Localization use Unity Localization
- string content can be add/modify in tool bar "Window/Asset ManageMent/Localization Tables"
- can only change languages in Unity Editor Play Mode for now.