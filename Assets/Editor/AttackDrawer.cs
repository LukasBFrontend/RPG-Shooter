using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Attack))]
public class AttackDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var _name = property.FindPropertyRelative("name");
        var _attackType = property.FindPropertyRelative("attackType");
        var _projectilePrefab = property.FindPropertyRelative("projectilePrefab");
        var _projectileOrigin = property.FindPropertyRelative("origin");
        var _projectileVelocity = property.FindPropertyRelative("projectileVelocity");
        var _damage = property.FindPropertyRelative("damage");
        var _knockbackForce = property.FindPropertyRelative("knockbackForce");
        var _cooldown = property.FindPropertyRelative("cooldown");

        float line = EditorGUIUtility.singleLineHeight;
        float spacing = 2f;

        // 🔹 Foldout
        Rect foldoutRect = new Rect(position.x, position.y, position.width, line);
        property.isExpanded = EditorGUI.Foldout(
            foldoutRect,
            property.isExpanded,
            "Attack",
            true,
            EditorStyles.foldoutHeader
        );

        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        // 🔹 HelpBox background
        Rect boxRect = new Rect(
            position.x,
            position.y + line + spacing,
            position.width,
            GetInnerHeight(property)
        );
        EditorGUI.HelpBox(boxRect, GUIContent.none.text, MessageType.None);

        float y = boxRect.y + spacing;

        // Name
        EditorGUI.PropertyField(
            new Rect(position.x + 6, y, position.width - 12, line),
            _name
        );
        y += line + spacing;

        // Attack Type
        EditorGUI.PropertyField(
            new Rect(position.x + 6, y, position.width - 12, line),
            _attackType
        );
        y += line + spacing;

        // Ranged-only fields
        if ((Attack.AttackType)_attackType.enumValueIndex == Attack.AttackType.Ranged)
        {
            EditorGUI.PropertyField(
                new Rect(position.x + 6, y, position.width - 12, line),
                _projectilePrefab,
                new GUIContent("Prefab")
            );
            y += line + spacing;

            EditorGUI.PropertyField(
                new Rect(position.x + 6, y, position.width - 12, line),
                _projectileOrigin,
                new GUIContent("Origin")
            );
            y += line + spacing;

            EditorGUI.PropertyField(
                new Rect(position.x + 6, y, position.width - 12, line),
                _projectileVelocity,
                new GUIContent("Velocity")
            );
            y += line + spacing;
        }

        // Damage
        EditorGUI.PropertyField(
            new Rect(position.x + 6, y, position.width - 12, line),
            _damage
        );
        y += line + spacing;

        // Knockback
        EditorGUI.PropertyField(
            new Rect(position.x + 6, y, position.width - 12, line),
            _knockbackForce
        );
        y += line + spacing;

        // Cooldown
        EditorGUI.PropertyField(
            new Rect(position.x + 6, y, position.width - 12, line),
            _cooldown
        );

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float line = EditorGUIUtility.singleLineHeight;
        float spacing = 2f;

        // Foldout header only
        if (!property.isExpanded)
            return line;

        return line + spacing + GetInnerHeight(property);
    }

    private float GetInnerHeight(SerializedProperty property)
    {
        var _attackType = property.FindPropertyRelative("attackType");

        int lines =
            1 + // name
            1 + // attackType
            1 + // damage
            1 + // knockback
            1;  // cooldown

        if ((Attack.AttackType)_attackType.enumValueIndex == Attack.AttackType.Ranged)
            lines += 2; // prefab + velocity

        return lines * (EditorGUIUtility.singleLineHeight + 2f) + 4f;
    }
}
