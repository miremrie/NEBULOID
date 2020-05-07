using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool("Platform Tool")]
public class PatrolTool : EditorWindow
{
    static bool showAll = true;
    static bool autoUpdate = true;
    static bool showLabels = true;
    static bool showNames = true;
    static Vector2 labelOffset = new Vector2(20, -10);
    
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
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= DrawPatrols;
        Selection.selectionChanged -= VisibilityUpdate;
        EditorApplication.hierarchyChanged -= UpdateActivePatrolsAndWaypoints;
    }

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
        GUILayout.Label("Patrols", EditorStyles.boldLabel);

        var oldShowAll = showAll;
        showAll = EditorGUILayout.Toggle("Show all / only selected", showAll);
        autoUpdate = EditorGUILayout.Toggle("Auto update waypoints", autoUpdate);
        showLabels = EditorGUILayout.Toggle("Show labels", showLabels);
        showNames = EditorGUILayout.Toggle("Show names", showNames);
        labelOffset = EditorGUILayout.Vector2Field("Label offset", labelOffset); 
        if (showAll != oldShowAll) UpdateActivePatrolsAndVisibility();
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