#  Solar System Simulation

A realistic 3D solar system simulation built with Unity.

![Solar System Preview](Screenshots/1.png)

##  Features

### Physics Simulation
- **Newtonian Gravity**
- **Velocity Verlet Integration**
- **Orbital Mechanics**

### Celestial Bodies
-  **Sun**
-  **Mercury**
-  **Venus**
-  **Earth**
-  **Mars**
-  **Jupiter**
-  **Saturn**
-  **Uranus**
-  **Neptune**
-  **Moon**

### Visual Features
- **Realistic Textures**
- **Spacetime Curvature Grid**
- **Orbital Trails**
- **Axial Tilt**
- **Day/Night Cycle**
- **Star Field**

### Camera System
- **Follow Camera**
- **Free Camera**
- **Smooth Zoom**

### Time Control
- **Variable Time Scale**
- **Pause Function**
- **Stable Physics**

##  Controls

### Camera
| Key | Action |
|-----|--------|
| `TAB` | Toggle Free/Follow camera |
| `F` | Cycle through planets (Follow mode) |
| `Right Click + Drag` | Rotate camera |
| `Scroll Wheel` | Zoom (Follow) / Speed (Free) |

### Free Camera Movement
| Key | Action |
|-----|--------|
| `W A S D` | Move horizontally |
| `E` | Move up |
| `Q` | Move down |
| `Shift` | Move faster |

### Time
| Key | Action |
|-----|--------|
| `↑` | Increase time speed |
| `↓` | Decrease time speed |
| `Space` | Pause/Resume |

### View
| Key | Action |
|-----|--------|
| `G` | Toggle spacetime curvature grid |
| `H` | Toggle controls UI |

## 🔧 Requirements

- **Unity**: 6000.3.8f1 LTS or newer
- **Render Pipeline**: Universal Render Pipeline (URP)
- **Input System**: New Input System package

##  Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/Breiscot/Solar-System.Breiscot.git

2. Open the project in Unity Hub

3. If prompted, install required packages:
  - Universal RP
  - Input System
  - TextMeshPro

4. Open the main scene:
    ```bash
    Assets/Scenes/SolarSystem.unity

5. And then try it!

##  Project Structure
  ```bash
    Assets/
    ├── Resources/
    │   ├── Textures/       # Planet textures
    │   └── Materials/      # Loaded materials
    ├── Scenes/
    │   └── SolarSystem.unity
    ├── Scripts/
    │   ├── CelestialBody.cs       # Planet/moon component
    │   ├── GravityManager.cs      # Physics simulation
    │   ├── CameraController.cs    # Camera system
    │   ├── SolarSystemSetup.cs    # System generator
    │   ├── PlanetRotation.cs      # Axial rotation
    │   ├── SunLight.cs            # Sun lighting
    │   ├── SaturnRings.cs         # Saturn's rings
    │   ├── StarField.cs           # Background stars
    │   ├── SpacetimeCurvature.cs  # Gravity visualization
    │   ├── TimeController.cs      # Time controls + UI
    │   └── ControlsUI.cs          # Controls display
    └── Settings/            # URP settings
