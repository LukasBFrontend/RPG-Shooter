# RPG-Shooter

## Naming Conventions
To make files easier to find as well as conveying their intended use, try to give clear and consistent names.
Below is a table giving suggestions for different file types.

| File type | Naming |
| -------- | ------- |
| Folder | Example Name |
| File | example_name <br> example_name_long <br> example_name_long_variant |
| Prefab | Exampel Name <br> Exampel Name (variant)|

### Aditional notes:
- When naming folders, take the parent folder into account, say for example we're creating a new folder in `/Sprites` for player sprites, we should name the new folder to avoid redundancy:
    - **'Player Sprites'** - path becomes `/Sprites/Player Sprites` ❌
    - **'Player'** - path becomes `/Sprites/Player` ✔️
- Asset names (meaning files and prefabs) should be descriptive but can use file type for context
    - .wav files don't have to include 'soundeffect'
    - Animation Controller assets don't have to include 'Animation' or 'Controller'
