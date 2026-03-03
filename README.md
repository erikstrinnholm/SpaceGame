# SPACE GAME
Fly around with a spaceship and shoot down asteroids or explore the spacestation hangar in characterview in this demo.

## Project Overview
**Platform:** macOS & Windows Demo  
**Engine:** Unity 2022.3 LTS  
**Genre:** Space Combat / Action
**ProjectType** Demo / Portfolio Project (Ongoing project that will develop further later)

The repository showcases the architecture and core systems. (not a fully buildable project)
(this is due to third party licences no allow distribution of raw source-files, working on building demo scenes to include a buildable project, but this is an ongoing fix)

See LICENSE and CREDITS.md for the project license and third party asset usage.

Demo builds are avaiable in Release https://github.com/erikstrinnholm/SpaceGame/releases


## How to play the Demo
Download the MAC or Windows.zip from https://github.com/erikstrinnholm/SpaceGame/releases

### MAC users
1. Download and unzip the file.
2. Launch the application 
(If the macOS says the app is damaged, to allow you to play the game)
    1. Open Terminal
    2. Navigate to the folder containing the app.
    3. Run: xattr -cr GameDemoMac.app
    4. Then launch it normally.
    (This is due to:
        - The application is still unsigned.
        - The application is downloaded from the internet.
    
### Windows users
1. Download an unzip the file.
2. Launch the .exe application.

## Game Features
- Spaceship flight (ship rotates towards the aim point, and moves forward by thrust)
- Combat system (weapon switching, firing, reloading etc.)
- Inventory system (work in progress)
- Save system (Player data is stored locally in json format)
- Menu and UI navigation
- Saves AudioLevels between sessions


## Gameplay 
### Start Scene
<p float="left">
  <img src="Media/Menu1.png" width="200" />
  <img src="Media/Menu2.png" width="200" />
  <img src="Media/Menu3.png" width="200" />
</p>

### Ship Scene
<img src="Media/Ship.gif" width="400" />

#### Inventory
<img src="Media/Inventory.png" width="400" />

### Entrance to switch between Ship and Character -Scene
<img src="Media/StationEntrance.png" width="400" />"

### Character Scene
<img src="Media/Character.gif" width="400" />


## Controls
### Universal
| Action        | PC               | Controller (not tested) |
|---------------|-----------------|------------------------|
| Toggle Menu   | ESC / Start      | Start                  |
| Toggle Inventory | I             | Select                 |

### Menu
| Action        | PC                        | Controller (not tested) |
|---------------|---------------------------|------------------------|
| Navigate      | Mouse / WASD / Arrow keys | Left Stick             |

### Inventory
| Action        | PC           | Controller (not tested) |
|---------------|-------------|------------------------|
| Left Tab      | 1           | Left Shoulder          |
| Right Tab     | 2           | Right Shoulder         |

### Ship
| Action        | PC           | Controller (not tested) |
|---------------|-------------|------------------------|
| Aim           | 1           | Right Stick            |
| Fire          | LeftMouse   | Right Shoulder         |
| Throttle      | Space       | Button South           |
| Brake         | C           | Button East            |
| DodgeRight    | D           | Right Trigger          |
| DodgeLeft     | A           | Left Trigger           |
| Boost         | Shift       | Left Shoulder          |
| SwitchWeapon  | Q           | Button North           |
| Reload        | R           | Button West            |
| QuickItem1    | 1           | D-Pad Up               |
| QuickItem2    | 2           | D-Pad Right            |
| QuickItem3    | 3           | D-Pad Down             |
| QuickItem4    | 4           | D-Pad Left             |

### Character
| Action        | PC           | Controller (not tested) |
|---------------|-------------|------------------------|
| Move          | WASD / Arrow keys | Left Stick             |
| Look          | Mouse       | Right Stick            |
| ToggleCrouch  | C           | Button South           |
| ToggleRun     | 0           | Button North           |
| Jump          | Space       | Button East            |
| Fire          | LeftMouse   | Right Shoulder         |
| SwitchWeapon  | Q           | Button North           |
| Reload        | R           | Button West            |
