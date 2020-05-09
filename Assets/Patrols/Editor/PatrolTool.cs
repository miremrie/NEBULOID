using System;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using System.Linq;

[EditorTool("Platform Tool")]
public class PatrolTool : EditorWindow
{
    private const string WAYPOINT_PATH_KEY = "patrols_editor_wayPath";
    private const string TEMPLATE_PATH_KEY = "patrols_editor_template_path";
    private const string SHOWALL_KEY = "patrols_editor_showall";
    private const string AUTOUPDATE_KEY = "patrols_editor_autoupdate";
    private const string SHOWNAMES_KEY = "patrols_editor_shownames";
    private const string SHOW_LABELS_KEY = "patrols_editor_show_labels";
    private const string LABEL_OFFSET_X = "patrols_editor_labeloffsetX";
    private const string LABEL_OFFSET_Y = "patrols_editor_labeloffsetY";

    GameObject waypointPrefab;
    PatrolTemplate templateToSpawn;
    bool showAll = true;
    bool autoUpdate = true;
    bool showLabels = true;
    bool showNames = true;
    Vector2 labelOffset = new Vector2(20, -10);
    PatrolPath[] activePatrols;

    [MenuItem("Window/Patrols #%&p")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        PatrolTool window = (PatrolTool)GetWindow(typeof(PatrolTool));
        window.Show();
    }

    void OnEnable()
    {
        UpdateActivePatrolsAndWaypoints();

        SceneView.duringSceneGui += DrawPatrols;
        Selection.selectionChanged += VisibilityUpdate;
        EditorApplication.hierarchyChanged += UpdateActivePatrolsAndWaypoints;

        LoadPreferences();
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= DrawPatrols;
        Selection.selectionChanged -= VisibilityUpdate;
        EditorApplication.hierarchyChanged -= UpdateActivePatrolsAndWaypoints;

        SavePreferences();

        // hide all
        foreach (var p in activePatrols) { ShowPath(p, false); }
    }
    
    private void LoadPreferences()
    {
        waypointPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(PlayerPrefs.GetString(WAYPOINT_PATH_KEY, ""), typeof(GameObject));
        templateToSpawn = (PatrolTemplate)AssetDatabase.LoadAssetAtPath(PlayerPrefs.GetString(TEMPLATE_PATH_KEY, ""), typeof(PatrolTemplate));
        showAll = IntToBool(PlayerPrefs.GetInt(SHOWALL_KEY, 1));
        autoUpdate = IntToBool(PlayerPrefs.GetInt(AUTOUPDATE_KEY, 1));
        showLabels = IntToBool(PlayerPrefs.GetInt(SHOW_LABELS_KEY, 1));
        showNames = IntToBool(PlayerPrefs.GetInt(SHOWNAMES_KEY, 1));
        labelOffset = new Vector2(PlayerPrefs.GetFloat(LABEL_OFFSET_X, 20), PlayerPrefs.GetFloat(LABEL_OFFSET_Y, -10));
    }

    private void SavePreferences()
    {
        PlayerPrefs.SetString(WAYPOINT_PATH_KEY, AssetDatabase.GetAssetPath(waypointPrefab));
        PlayerPrefs.SetString(TEMPLATE_PATH_KEY, AssetDatabase.GetAssetPath(templateToSpawn));
        PlayerPrefs.SetInt(SHOWALL_KEY, BoolToInt(showAll));
        PlayerPrefs.SetInt(AUTOUPDATE_KEY, BoolToInt(autoUpdate));
        PlayerPrefs.SetInt(SHOW_LABELS_KEY, BoolToInt(showLabels));
        PlayerPrefs.SetInt(SHOWNAMES_KEY, BoolToInt(showNames));
        PlayerPrefs.SetFloat(LABEL_OFFSET_X, labelOffset.x);
        PlayerPrefs.SetFloat(LABEL_OFFSET_Y, labelOffset.y);
    }

    int BoolToInt(bool v) => v ? 1 : 0 ;
    bool IntToBool(int v) => v == 0 ? false : true;

    private void UpdateActivePatrolsAndWaypoints()
    {
        activePatrols = FindObjectsOfType<PatrolPath>();

        if (autoUpdate)
        {
            foreach (var p in activePatrols)
            {
                UpdateWaypoints(p);
            }
        }
    }

    void OnGUI()
    {

        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(500));
        GUILayout.Label("Patrols", EditorStyles.boldLabel);

        var oldShowAll = showAll;
        showAll = EditorGUILayout.Toggle("Show all / only selected", showAll);
        autoUpdate = EditorGUILayout.Toggle("Auto update waypoints", autoUpdate);
        showLabels = EditorGUILayout.Toggle("Show labels", showLabels);
        showNames = EditorGUILayout.Toggle("Show names", showNames);
        labelOffset = EditorGUILayout.Vector2Field("Label offset", labelOffset);

        EditorGUILayout.LabelField("Template Creation", EditorStyles.boldLabel);
        if (GUILayout.Button("Create template (from selected or empty")) CreatePatrolTemplate();

        EditorGUILayout.BeginFadeGroup(1);
        EditorGUILayout.LabelField("Patrol spawning", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Patrol template to spawn");
            templateToSpawn = (PatrolTemplate) EditorGUILayout.ObjectField(templateToSpawn, typeof(PatrolTemplate), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndFadeGroup();
        
        EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Waypoint prefab");
            waypointPrefab = (GameObject) EditorGUILayout.ObjectField(waypointPrefab, typeof(GameObject), false);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Spawn patrol from template")) CreatePatrolFromTemplate();

        EditorGUILayout.EndVertical();

        if (showAll != oldShowAll) UpdateActivePatrolsAndVisibility();
    }

    private void CreatePatrolFromTemplate()
    {
        if (waypointPrefab == null) {
            EditorUtility.DisplayDialog("Could not create patrol", "No waypoint prefab selected", "ok");
            return;
        }

        var newPatrolGo = new GameObject("PatrolPath");
        newPatrolGo.transform.position = Selection.activeTransform?.position ?? Vector3.zero;
        var patrol = newPatrolGo.AddComponent<PatrolPath>();

        patrol.color = Color.white;

        if (templateToSpawn != null)
        {
            patrol.color = templateToSpawn.color;
            patrol.waypoints = new Transform[templateToSpawn.waypoints.Length];

            for (int i = 0; i < patrol.waypoints.Length; i++)
            {
                var way = Instantiate(waypointPrefab, newPatrolGo.transform);
                way.name = $"{waypointPrefab.name} ({i})";
                way.transform.localPosition = templateToSpawn.waypoints[i];
            }
        }

        Undo.RegisterCreatedObjectUndo(newPatrolGo, "Created go");
    }

    private void CreatePatrolTemplate()
    {
        var existing = Selection.activeGameObject?.GetComponentInParent<PatrolPath>();

        var asset = ScriptableObject.CreateInstance<PatrolTemplate>();

        if (existing) {

            asset.color = existing.color;

            asset.waypoints = existing.waypoints
                .Select(x => new Vector2(x.localPosition.x, x.localPosition.y))
                .ToArray();
        }

        ProjectWindowUtil.CreateAsset(asset, "New " + typeof(PatrolTemplate).Name + ".asset");
    }

    private void UpdateActivePatrolsAndVisibility()
    {
        activePatrols = FindObjectsOfType<PatrolPath>();
        VisibilityUpdate();
    }

    private void VisibilityUpdate()
    {
        foreach (var p in activePatrols)
        {
            ShowPath(p, ShouldBeVisible(p));
        }
    }

    bool ShouldBeVisible(PatrolPath path)
    {
        var selected = Selection.activeGameObject;
        return showAll || (selected != null && selected.GetComponentInParent<PatrolPath>() == path);
    }

    private void ShowPath(PatrolPath p, bool show)
    {
        foreach (var ob in p.waypoints)
        {
            if (ob == null) continue;
            ob.gameObject.SetActive(show);
        }
    }

    void DrawPatrols(SceneView view)
    {
        foreach (var p in activePatrols)
        {
            if (p != null && ShouldBeVisible(p))
                DrawPatrol(p);
        }
    }

    private void DrawPatrol(PatrolPath p)
    {
        var waypoints = p.waypoints;

        if (waypoints == null) return;

        Handles.color = p.color;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = p.color;

        var len = waypoints.Length;
        if (len < 2) return;

        for ( int i = 0; i < waypoints.Length; i++ )
        {
            var next = i == len - 1 ? waypoints[0] : waypoints[i + 1];
            var curr = waypoints[i];
            
            if (curr != null && next != null)
            {
                string text;
                if (showNames) text = $" {i.ToString() } :: {curr.name}";
                else text = i.ToString();

                Handles.DrawLine(curr.transform.position, next.transform.position);
                Handles.SphereHandleCap(i, curr.transform.position, Quaternion.identity, 0.35f, EventType.Repaint);
                if (showLabels)
                {
                    Handles.BeginGUI();
                    Vector3 pos = curr.transform.position;
                    Vector2 pos2D = HandleUtility.WorldToGUIPoint(pos);
                    var rect = new Rect(pos2D.x + labelOffset.x, pos2D.y + labelOffset.y, 100, 100);
                    GUI.Label(rect, text, style);
                    Handles.EndGUI();
                }
            }
        }
    }

    public void UpdateWaypoints(PatrolPath patrol)
    {
        patrol.waypoints = new Transform[patrol.transform.childCount];
        int i = 0;
        foreach (Transform t in patrol.transform)
        {
            patrol.waypoints[i++] = t;
        }
    }

    [MenuItem("Editor Extensions/Duplicate Selected and make it as a sibling #%d")] // Ctrl-Shift-d
    static void DuplicateAsNextSiblingAndSelect()
    {
        var s = Selection.activeGameObject;

        GameObject duped = Instantiate(s, s.transform.parent);
        duped.transform.SetSiblingIndex(s.transform.GetSiblingIndex() + 1);
        Selection.activeGameObject = duped;
        Undo.RegisterCreatedObjectUndo(duped, "Created go");
        
    }
}