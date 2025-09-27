# Solar System Model

the current project is an accurate simulation of Solar System. It is built upon data from JPL Horizons (used to compute objects orbits).
The simulation includes major celestial objects, i.e. Sun, planets, and Earth's Moon, and a subset of artificial satellites. The information about satellites come from a Python server which is located in `server/` directory.

The scales for distance and sizes are different: for distances scale is 66,666 km : 1 unit, for planet sizes - 640 km : 1 unit.

## Demo & screenshots

[![Demo Video](https://i.vimeocdn.com/video/2063896997-7219fd9c894e5f2d393f77aa694aef84635f15cf3291e77d77a0ebb48becca5c-d_640x360?&r=pad&region=us)](https://vimeo.com/1122524180)

![Attached camera view](screenshots/acv.png)

![Entire system view](screenshots/esv.png)

## Controls

There are three available cameras:
- main - attached to Sun, provides view for the whole system;
- floating - attached to a given object (e.g., a planet or a satellite);
- free - not attached to anything, can move freely.

- Common keys:
    - F4/F5 - Change between the cameras
    - Slider - change speed of time
    - InputField at the top - change date (format "dd/MM/yyyy" , e.g. "02/09/2000")
    - Floating Camera (attached to the objects):
    - W,S,A,D - move around object
    - Mouse + Right_Button - move around object
    - R - reset camera position and rotation
    - PageUp/PageDown - move to the next/previous planet(object)
    - Mouse ScrollWheel - zoom
- Main Camera (attached to the Sun):
    - W,S,A,D - move around the sun
    - R - reset camera position
    - Mouse ScrollWheel - zoom
- Free Camera:
    - W,S - move forward the object
    - A,D - turn left/right
    - Q,Z - turn up/down

## Getting Started

### Prerequisites

You will need Unity to install and work with project.

### Installing

Download repository and open as Unity project. The main scene is named `SolarSystem`. 

The satellites list and positions are served by Python backend. The code for the server can be found in `server/`, and the endpoint's URL is configured in `config/urls.txt`.

Built With: Unity - game engine

