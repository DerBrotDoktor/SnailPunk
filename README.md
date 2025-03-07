# **SnailPunk**

 **Snail Punk is a small village building game with the focus on a tutorial that is intended to teach the player the basic functions of a building game.**

 Snail Punk was developed as a student project at the S4G School for Games in the summer of 2024.
  
  Play it on [Itch.io](https://s4g.itch.io/snail-punk)!
 
## Role
As the only engineer on the team, I was responsible for all the programming, engine and implementation, as well as the design and implementation of the user interface. As there was no game designer, I was also involved in the game design.

## Tools

The game was developed using Unity 2022.3.32f1 and C#. <br />
We used Perforce for version control and FMOD for sounds.

## Highlights
- **[The Snail Behaviour System](Assets/Snails/Scripts)** <br/>
Each snail is assigned a workstation, if space is available. They are then given the appropriate snail behaviour script, which manages all the tasks they need to complete. They also take care of their basic needs, such as food and sleep.
- **[The Tutorial System](Assets/Tutorial/Scripts)** <br/>
The tutorial system works with Scriptable Objects. Each tutorial consists of multiple tutorial steps, which in turn contain several tutorial tasks. I aimed to make the system as modular as possible but have since developed a new tutorial system that is easier to use and more expandable. You can find it [here](https://github.com/DerBrotDoktor/TutorialSystem).

