### Escaper - Abandoned Beta

This is my unfinished Unity 5.6 project - a first-person survival shooter I've been working on (the very first Unity game as well). The "Beta" tag means it's a work in progress (I'm not working on that anymore though), but most of the core systems are functional.

**What I've Built:**

The game is set in a sorta-post-apocalyptic world where you play as a survivor trying to escape an abandoned city. I focused on creating a deep, immersive experience with several interconnected systems:

*   **Inventory & Looting:** I built a grid-based inventory system where items take up physical space. You can rotate items to fit them better, loot containers, and manage your limited carrying capacity. Items like weapons, medkits, and armor can be found throughout the world.

*   **Modular Weapons:** This is probably the most complex system I've made. Weapons are assembled from individual parts - barrels, grips, magazines, sights. Each part affects how the weapon handles (recoil, fire rate, damage). The game can generate thousands of unique weapon combinations procedurally.

*   **AI Enemies (Scavs):** The city is populated by AI scavengers that patrol the map, detect the player using line-of-sight, and engage in combat. They use the same modular weapon system as the player, so encounters feel varied. Their spawns and behavior are controlled by a manager that simply spawns them all over the place to ensure it is chaotic.

*   **Quest System:** I added simple objectives to give players direction - gathering specific items, destroying targets, or reaching certain locations. A quest is given on the start of a new day.

*   **Graphics Settings:** Since performance can vary, I built a comprehensive settings menu that lets players toggle post-processing effects (bloom, vignette, grain, etc.), adjust texture quality, and even control shader complexity for lower-end machines. Well, I had to make this thing on a potato PC too, keep it in mind.

*   **Save/Load System:** The game saves almost everything - player position, inventory, weapon configurations, world items, enemy states, and quest progress across multiple slots.

**Tools & Assets I Used:**

*   **SabreCSG** - For in-editor level creation. Makes building environments much faster. Mainly I have exported complex pieces made with this thing and then reused those (such as buildings).
*   **ProBuilder/ProGrids** - For modeling and snapping things into place.
*   **Unity Post-Processing Stack** - For all the visual effects.
*   **Custom Shaders** - I wrote several shaders for decals, weapon sights, and material blending.
*   **Asset Library** - A huge collection of models, textures, and sounds I've gathered to build the world.

**Current State:**

The project is functional but incomplete - hence "Beta." All the core systems work together, but it needs more content, polishing, and probably a lot of bug fixes. I'm sharing it as a reference for anyone interested in how these systems can be implemented in Unity.

**What's Here:**
- Complete C# source code for all systems
- Hundreds of 3D models and prefabs
- Textures and materials
- Sound effects
- Multiple test scenes
- Editor tools and extensions

**Built with:** Unity, C#

**Note:** This was built with a specific Unity version (check ProjectVersion.txt). The `.meta` files are standard Unity stuff - keep them if you want to open the project.
