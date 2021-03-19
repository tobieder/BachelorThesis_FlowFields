# Flow Fields for motion control of large NPC groups in real-time strategy games

Pathfinding algorithms are a particularly important component of real-time strategy games. In order to solve the dependency of the required computing time on the number of troops
flow fields can be used to solve the dependency of the required computing time on the number of troops on a field. The goal of this work
is to answer the question whether flow fields are a reasonable alternative to the widely used
A*-algorithm and to what extent it can be used in real-time strategy games.
is possible. For this purpose, an application was developed with the help of the Unity Engine, which allows to test both
pathfinding methods in detail and to measure the performance in benchmarks.
It can be seen that flow fields are significantly faster above a certain number of units, but some of the functions provided by the A* algorithm are lost,
are lost. A solution which is based on both methods and dynamically switches between the
algorithms turns out to be the preferred method.

This project was build using the Unity Engine Version 2020.1.4f1. There is an executable available under Releases.

## Features
- Flow Fields
- A*
- Dynamic Pathfinding using Flow Fields and A*
- Build system to dynamically change the grid
- Sandbox to test the algorithms in a RTS-like area
- Benchmarking tool with visual representation of the results

## References
- [Terrain Texture Atlas](https://mtnphil.files.wordpress.com/2011/09/entireatlas.jpg)
- [Dirt Texture for the edges](https://www.deviantart.com/fabooguy/art/Dirt-Ground-Texture-Tileable-2048x2048-441212191)
- [Castle Guard 1](https://www.mixamo.com/#/?page=1&query=guard&type=Character)
