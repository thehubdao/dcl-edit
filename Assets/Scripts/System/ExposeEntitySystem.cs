using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using UnityEngine;

public class ExposeEntitySystem
{
    public static bool IsEntityExposable(DclEntity entity)
    {
        var scene = EditorStates.CurrentSceneState.CurrentScene;
        if (scene == null)
        {
            return false;
        }

        var exposedName = ExposedNameFromEntity(entity);

        return !scene
            .AllEntities
            .Select(pair => pair.Value)
            .Where(e=>e.IsExposed)
            .Any(e => 
                ExposedNameFromEntity(e).Equals(exposedName));
    }

    public static string ExposedNameFromEntity(DclEntity entity)
    {
        return ExposedNameFromName(entity.ShownName);
    }

    public static string ExposedNameFromName(string name)
    {
        var exposedName = new StringBuilder(name.Length);

        // remove forbidden chars
        foreach (var c in name.Where(c => char.IsDigit(c) || char.IsLetter(c) || c == '_' || c == '$'))
        {
            exposedName.Append(c);
        }

        // set name to "_" when name is empty
        if (exposedName.Length <= 0)
        {
            exposedName.Append('_');
        }

        // add "_" when fist char is a digit
        if (char.IsDigit(exposedName[0]))
        {
            exposedName.Insert(0, '_');
        }

        // add "_" when name is reserved word
        if (reservedWords.Contains(exposedName.ToString())) // TODO: make this more efficient
        {
            exposedName.Insert(0, '_');
        }

        return exposedName.ToString();
    }

    private static readonly string[] reservedWords =
    {
        // Reserved Words
        "break",
        "case",
        "catch",
        "class",
        "const",
        "continue",
        "debugger",
        "default",
        "delete",
        "do",
        "else",
        "enum",
        "export",
        "extends",
        "false",
        "finally",
        "for",
        "function",
        "if",
        "import",
        "in",
        "instanceof",
        "new",
        "null",
        "return",
        "super",
        "switch",
        "this",
        "throw",
        "true",
        "try",
        "typeof",
        "var",
        "void",
        "while",
        "with",
        // Strict Mode Reserved Words
        "as",
        "implements",
        "interface",
        "let",
        "package",
        "private",
        "protected",
        "public",
        "static",
        "yield",
        // Contextual Keywords
        "any",
        "boolean",
        "constructor",
        "declare",
        "get",
        "module",
        "require",
        "number",
        "set",
        "string",
        "symbol",
        "type",
        "from",
        "of"
    };
}