# GitHub Copilot Instructions for Vanilla Outposts Expanded Expanded: Ranch (Continued) Mod

## Mod Overview and Purpose

**Vanilla Outposts Expanded Expanded: Ranch (Continued)** is a mod for RimWorld that updates the original Clown Cats mod. This mod adds the ability to create Ranch Outposts, which are temporary, autonomously-led camps specialized in raising and butchering animals. The mod allows players to raise a variety of animals, collect their products, and cull older animals when necessary.

## Key Features and Systems

- **Ranch Outpost**: A temporary outpost that specializes in raising and butchering animals.
- **Animal Farming**: Animals with the trade-tag `AnimalFarm` can be raised, except for Boomalopes.
- **Animal Products**: Players can collect products such as Chemfuel Milk, Eggs, meat, and leather.
- **Culling System**: Older animals are culled based on their skills, producing additional products like elephant tusks and thrumbo horns.
- **Configurable Multipliers**: Players can tweak various multipliers for the outpost.
- **Non-Grazer Animals**: Option to raise animals that do not graze, which is off by default.

## Coding Patterns and Conventions

1. **Consistent Naming**: Use clear and descriptive names for classes, methods, and variables.
2. **Code Organization**: Group related code together and separate concerns into different classes.
3. **Documentation**: Comment your code where necessary, especially for complex logic or external dependencies.
4. **Error Handling**: Implement robust error handling to ensure the mod does not break the game.
5. **Testing**: Test your mod thoroughly to ensure it works as expected and does not conflict with other mods.

## XML Integration

XML files are crucial for defining various aspects of the mod, such as structures and world objects. Here are some key XML files and their purposes:

- **Outpost_Structure_Ranch.xml**: Defines the structure layout for the Ranch Outpost.
- **Ranch.xml**: Defines the WorldObjectDef for the Ranch outpost.
- **About.xml**: Contains metadata about the mod, including dependencies.
- **TranslationTemplate/Ranch.xml**: Provides translation templates for the mod.

### Example XML Snippet

xml
<StructureLayoutDef Name="Outpost_Structure_Ranch" Description="Ranch Outpost" Category="Outpost">
    <Tags>
        <li>Ranch</li>
    </Tags>
    <Layout>
        <li>Structure_Ranch</li>
    </Layout>
</StructureLayoutDef>


## Harmony Patching

Harmony is used to patch existing RimWorld code to add new functionality or modify existing behavior. Here are some common patterns for Harmony patching:

1. **Prefix Patch**: Executes before the original method.
2. **Postfix Patch**: Executes after the original method.
3. **Transpiler Patch**: Modifies the bytecode of the original method.

### Example Harmony Patch

csharp
using HarmonyLib;
using RimWorld;
using System.Reflection;
using UnityEngine;

namespace VanillaOutpostsExpandedExpandedRanch
{
    [HarmonyPatch(typeof(Outpost), "Update")]
    public static class Outpost_Update_Patch
    {
        public static void Postfix(Outpost __instance)
        {
            // Custom logic to update the Ranch outpost
        }
    }
}


## Suggestions for Copilot

1. **Provide Examples**: When asking Copilot for code snippets or examples, provide context and specific requirements.
2. **Review Code**: Always review the generated code to ensure it aligns with the mod's coding patterns and conventions.
3. **Test Thoroughly**: Before using generated code in the mod, thoroughly test it to ensure it works as expected and does not introduce bugs.
4. **Documentation**: Use Copilot to generate or assist in writing documentation for your code, especially for complex logic or external dependencies.

By following these guidelines, you can effectively use GitHub Copilot to enhance your mod development process for Vanilla Outposts Expanded Expanded: Ranch (Continued).

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
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

