# GitHub Copilot Instructions for Vanilla Outposts Expanded Expanded: Ranch (Continued)

Welcome to the development guide for "Vanilla Outposts Expanded Expanded: Ranch (Continued)". This file serves as an instructional manual for utilizing GitHub Copilot efficiently within this RimWorld modding project. 

## Mod Overview and Purpose

Vanilla Outposts Expanded Expanded: Ranch (Continued) is an enhancement of the original Vanilla Outposts Expanded mods, specifically focusing on creating ranch outposts. These outposts are temporary, autonomously-led camps specialized in raising and butchering animals to produce meat, leather, and other valuable animal products like ChemfuelMilk and Eggs.

### Key Purpose:
- To automate the process of animal farming and product gathering within RimWorld, enhancing gameplay with minimal player intervention.
- To be compatible with Vanilla Outposts Expanded, though it can function independently.

## Key Features and Systems

- **Ranch Outpost Creation**: Players can create a caravan with a male and female animal, along with a capable pawn to establish the outpost.
- **Autonomous Animal Management**: Animals are raised, reproduce, and products are gathered automatically. Older animals are culled when necessary to produce resources.
- **Customizable Multipliers**: Configurable options to adjust production rates and animal management rules.
- **Compatibility and Mod Interaction**: Integrates seamlessly with existing outpost systems and allows raising non-graze animals with specific settings.

## Coding Patterns and Conventions

- **C# Coding Standards**:
  - Adherence to C# coding conventions with clear method naming, such as `GetExtraOptions` and `ResultOptions`.
  - Use of comments to explain complex logic and code behavior.
  - Efficient and readable code with optimized performance in mind (especially crucial in cycle-heavy processes like autonomous animal management).

- **XML File Structure**:
  - Use of clear, descriptive names for XML nodes and attributes.
  - Ensuring XML def types like `WorldObjectDef` and `StructureLayoutDef` are accurately represented and linked to game functionalities.

## XML Integration

- **XML Defs**:
  - The mod includes multiple XML files primarily for defining game elements such as structures (`Outpost_Structure_Ranch.xml`) and world objects (`Ranch.xml`).

- **Structure and Content**:
  - Each XML file should define necessary attributes and be formatted for both readability and functionality.
  - Proper linking of XML with associated C# logic to allow seamless data flow and interaction.

## Harmony Patching

- **Modifying Game Behavior**:
  - Utilize Harmony for patching core game methods to introduce or alter behaviors, essential for modifying how outposts and animal management work.
  - Follow best practices for Harmony use, such as non-invasive patching and avoiding conflicts with other mods.

## Suggestions for Copilot

- **Code Suggestions**: Encourage Copilot to suggest optimized C# patterns that are compatible with Harmony patches.
- **XML Templates**: Leverage Copilot to generate XML templates for new def additions or adjustments.
- **Documentation**: Use Copilot for drafting detailed method documentation and inline comments for better code understanding.
- **Error Handling**: Suggest robust error handling patterns within C# to ensure stability across different game scenarios and mod configurations.

By following the above guidelines and utilizing these instructions in conjunction with GitHub Copilot, development and maintenance of the mod should become more streamlined and efficient.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).

