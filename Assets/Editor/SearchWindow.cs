using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class SearchWindow : EditorWindow
{
    private enum SearchTab { SceneObjects, CodeSearch }
    private SearchTab _currentTab = SearchTab.SceneObjects;

    private string _query = "";
    private Vector2 _scroll;
    private bool _searched = false;

    private enum SceneSearchMode { NameOrTag, Component }
    private SceneSearchMode _sceneMode = SceneSearchMode.NameOrTag;

    private struct SceneResult
    {
        public GameObject go;
        public string matchInfo;
    }
    private List<SceneResult> _sceneResults = new List<SceneResult>();

    private bool _caseSensitive = false;
    private bool _wholeWord = false;
    private bool _useRegex = false;
    private string[] _fileExtensions = new[] { ".cs", ".shader", ".hlsl", ".glsl", ".cginc" };
    private string _extensionInput = ".cs, .shader, .hlsl, .glsl, .cginc";

    private struct CodeResult
    {
        public string filePath;
        public string projectRelativePath;
        public List<(int line, string text)> matches;
    }
    private List<CodeResult> _codeResults = new List<CodeResult>();
    private Dictionary<string, bool> _foldouts = new Dictionary<string, bool>();
    private int _totalCodeMatches = 0;

    private GUIStyle _headerStyle;
    private GUIStyle _resultFileStyle;
    private GUIStyle _resultLineStyle;
    private GUIStyle _matchHighlightStyle;
    private GUIStyle _tabActiveStyle;
    private GUIStyle _tabInactiveStyle;
    private GUIStyle _dividerStyle;
    private bool _stylesInitialised = false;

    [MenuItem("Tools/Search Window")]
    public static void Open()
    {
        var win = GetWindow<SearchWindow>("Search");
        win.minSize = new Vector2(480, 420);
        win.Show();
    }

    private void InitStyles()
    {
        if(_stylesInitialised) return;
        _stylesInitialised = true;

        _headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 13,
            alignment = TextAnchor.MiddleLeft
        };

        _resultFileStyle = new GUIStyle(EditorStyles.foldout)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 11
        };

        _resultLineStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 11,
            wordWrap = false,
            richText = true
        };
        _resultLineStyle.normal.textColor = new Color(0.75f, 0.75f, 0.75f);

        _matchHighlightStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 11,
            wordWrap = false,
            richText = true
        };

        _tabActiveStyle = new GUIStyle(EditorStyles.toolbarButton)
        {
            fontStyle = FontStyle.Bold
        };
        _tabActiveStyle.normal.textColor = EditorGUIUtility.isProSkin
            ? new Color(0.4f, 0.8f, 1f)
            : new Color(0f, 0.35f, 0.7f);

        _tabInactiveStyle = new GUIStyle(EditorStyles.toolbarButton);

        _dividerStyle = new GUIStyle()
        {
            fixedHeight = 1,
            margin = new RectOffset(0, 0, 4, 4)
        };
    }

    private void OnGUI()
    {
        InitStyles();

        DrawHeader();
        DrawTabs();
        DrawSearchBar();

        EditorGUILayout.Space(2);
        DrawDivider();

        if(_currentTab == SearchTab.SceneObjects)
            DrawSceneOptions();
        else
            DrawCodeOptions();

        DrawDivider();
        DrawResults();
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("🔍 Project Search", _headerStyle ?? EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
        EditorGUILayout.EndHorizontal();
    }

    private void DrawTabs()
    {
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Toggle(_currentTab == SearchTab.SceneObjects, " Scene Objects ",
            _currentTab == SearchTab.SceneObjects ? _tabActiveStyle : _tabInactiveStyle))
        {
            if(_currentTab != SearchTab.SceneObjects) { _currentTab = SearchTab.SceneObjects; ClearResults(); }
        }
        if(GUILayout.Toggle(_currentTab == SearchTab.CodeSearch, " Code Search ",
            _currentTab == SearchTab.CodeSearch ? _tabActiveStyle : _tabInactiveStyle))
        {
            if(_currentTab != SearchTab.CodeSearch) { _currentTab = SearchTab.CodeSearch; ClearResults(); }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSearchBar()
    {
        EditorGUILayout.Space(6);
        EditorGUILayout.BeginHorizontal();

        GUI.SetNextControlName("SearchField");
        string newQuery = EditorGUILayout.TextField(_query, EditorStyles.toolbarSearchField, GUILayout.ExpandWidth(true));
        if(newQuery != _query)
        {
            _query = newQuery;
            _searched = false;
        }

        if(!string.IsNullOrEmpty(_query))
        {
            if(GUILayout.Button("✕", EditorStyles.toolbarButton, GUILayout.Width(24)))
            {
                _query = "";
                ClearResults();
                GUI.FocusControl("SearchField");
            }
        }

        bool doSearch = GUILayout.Button("Search", EditorStyles.toolbarButton, GUILayout.Width(58));
        EditorGUILayout.EndHorizontal();

        if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return
            && GUI.GetNameOfFocusedControl() == "SearchField")
        {
            doSearch = true;
            Event.current.Use();
        }

        if(doSearch && !string.IsNullOrEmpty(_query))
            RunSearch();
    }

    private void DrawSceneOptions()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Search by:", GUILayout.Width(66));
        _sceneMode = (SceneSearchMode)GUILayout.Toolbar((int)_sceneMode,
            new[] { "Name / Tag", "Component" }, EditorStyles.miniButton, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(4);
    }

    private void DrawCodeOptions()
    {
        EditorGUILayout.BeginHorizontal();
        _caseSensitive = EditorGUILayout.ToggleLeft("Case Sensitive", _caseSensitive, GUILayout.Width(115));
        _wholeWord = EditorGUILayout.ToggleLeft("Whole Word", _wholeWord, GUILayout.Width(95));
        _useRegex = EditorGUILayout.ToggleLeft("Regex", _useRegex, GUILayout.Width(60));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Extensions:", GUILayout.Width(70));
        string newExt = EditorGUILayout.TextField(_extensionInput);
        if(newExt != _extensionInput)
        {
            _extensionInput = newExt;
            _fileExtensions = _extensionInput.Split(',')
                .Select(e => e.Trim())
                .Where(e => e.StartsWith("."))
                .ToArray();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(4);
    }

    private void DrawResults()
    {
        if(!_searched) return;

        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        if(_currentTab == SearchTab.SceneObjects)
            DrawSceneResults();
        else
            DrawCodeResults();

        EditorGUILayout.EndScrollView();
    }

    private void DrawSceneResults()
    {
        if(_sceneResults.Count == 0)
        {
            EditorGUILayout.HelpBox($"No GameObjects found matching \"{_query}\".", MessageType.Info);
            return;
        }

        EditorGUILayout.LabelField($"{_sceneResults.Count} result(s) in scene", EditorStyles.miniLabel);
        EditorGUILayout.Space(2);

        foreach(var r in _sceneResults)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            if(GUILayout.Button("◎", EditorStyles.miniButton, GUILayout.Width(22), GUILayout.Height(18)))
            {
                EditorGUIUtility.PingObject(r.go);
                Selection.activeGameObject = r.go;
            }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.ObjectField(r.go, typeof(GameObject), true);
            EditorGUILayout.LabelField(r.matchInfo, EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawCodeResults()
    {
        if(_codeResults.Count == 0)
        {
            EditorGUILayout.HelpBox($"No matches found for \"{_query}\".", MessageType.Info);
            return;
        }

        EditorGUILayout.LabelField(
            $"{_totalCodeMatches} match(es) across {_codeResults.Count} file(s)",
            EditorStyles.miniLabel);
        EditorGUILayout.Space(2);

        foreach(var result in _codeResults)
        {
            if(!_foldouts.ContainsKey(result.filePath))
                _foldouts[result.filePath] = true;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            _foldouts[result.filePath] = EditorGUILayout.Foldout(
                _foldouts[result.filePath],
                $"{result.projectRelativePath} ({result.matches.Count})",
                true, _resultFileStyle ?? EditorStyles.foldout);

            if(GUILayout.Button("Open", EditorStyles.miniButton, GUILayout.Width(44)))
            {
                var asset = AssetDatabase.LoadAssetAtPath<Object>(result.projectRelativePath);
                if(asset != null) AssetDatabase.OpenAsset(asset);
                else Application.OpenURL("file://" + result.filePath);
            }

            if(GUILayout.Button("◎", EditorStyles.miniButton, GUILayout.Width(22)))
            {
                var asset = AssetDatabase.LoadAssetAtPath<Object>(result.projectRelativePath);
                if(asset != null) EditorGUIUtility.PingObject(asset);
            }

            EditorGUILayout.EndHorizontal();

            if(_foldouts[result.filePath])
            {
                foreach(var (lineNum, lineText) in result.matches)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label($"{lineNum,5} ", EditorStyles.miniLabel, GUILayout.Width(50));

                    string display = BuildHighlightedLine(lineText.Trim(), _query);
                    GUILayout.Label(display, _resultLineStyle ?? EditorStyles.label, GUILayout.ExpandWidth(true));

                    if(GUILayout.Button("↗", EditorStyles.miniButton, GUILayout.Width(22)))
                    {
                        var asset = AssetDatabase.LoadAssetAtPath<Object>(result.projectRelativePath);
                        if(asset != null) AssetDatabase.OpenAsset(asset, lineNum);
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
        }
    }

    private void RunSearch()
    {
        ClearResults();
        _searched = true;

        if(_currentTab == SearchTab.SceneObjects)
            RunSceneSearch();
        else
            RunCodeSearch();

        Repaint();
    }

    private void RunSceneSearch()
    {
        var allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.InstanceID);
        string q = _query.ToLower();

        foreach(var go in allObjects)
        {
            if(_sceneMode == SceneSearchMode.NameOrTag)
            {
                bool nameMatch = go.name.ToLower().Contains(q);
                bool tagMatch = !string.IsNullOrEmpty(go.tag) && go.tag.ToLower().Contains(q);

                if(nameMatch || tagMatch)
                {
                    string info = nameMatch && tagMatch
                        ? $"Name & Tag match | Tag: {go.tag}"
                        : nameMatch ? "Name match" : $"Tag match: {go.tag}";

                    _sceneResults.Add(new SceneResult { go = go, matchInfo = info });
                }
            }
            else // Component mode
            {
                var components = go.GetComponents<Component>();
                var matched = components
                    .Where(c => c != null && c.GetType().Name.ToLower().Contains(q))
                    .Select(c => c.GetType().Name)
                    .Distinct()
                    .ToList();

                if(matched.Count > 0)
                {
                    _sceneResults.Add(new SceneResult
                    {
                        go = go,
                        matchInfo = "Component(s): " + string.Join(", ", matched)
                    });
                }
            }
        }

        _sceneResults = _sceneResults.OrderBy(r => r.go.name).ToList();
    }

    private void RunCodeSearch()
    {
        string assetsPath = Application.dataPath;
        var allFiles = new List<string>();

        foreach(var ext in _fileExtensions)
            allFiles.AddRange(Directory.GetFiles(assetsPath, "*" + ext, SearchOption.AllDirectories));

        Regex regex = BuildRegex(_query);
        if(regex == null) return;

        foreach(var filePath in allFiles)
        {
            string[] lines;
            try { lines = File.ReadAllLines(filePath); }
            catch { continue; }

            var matches = new List<(int, string)>();
            for(int i = 0; i < lines.Length; i++)
            {
                if(regex.IsMatch(lines[i]))
                    matches.Add((i + 1, lines[i]));
            }

            if(matches.Count > 0)
            {
                string relative = "Assets" + filePath.Substring(assetsPath.Length).Replace('\\', '/');
                _codeResults.Add(new CodeResult
                {
                    filePath = filePath,
                    projectRelativePath = relative,
                    matches = matches
                });
                _totalCodeMatches += matches.Count;
            }
        }

        _codeResults = _codeResults.OrderBy(r => r.projectRelativePath).ToList();
    }

    private Regex BuildRegex(string query)
    {
        try
        {
            string pattern = _useRegex ? query : Regex.Escape(query);
            if(_wholeWord && !_useRegex) pattern = $@"\b{pattern}\b";
            var options = _caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
            return new Regex(pattern, options);
        }
        catch
        {
            EditorUtility.DisplayDialog("Regex Error", $"Invalid regex pattern:\n{query}", "OK");
            return null;
        }
    }

    private string BuildHighlightedLine(string line, string query)
    {
        const int maxLen = 140;
        if(line.Length > maxLen) line = line.Substring(0, maxLen) + "…";

        try
        {
            string pattern = _useRegex ? query : Regex.Escape(query);
            if(_wholeWord && !_useRegex) pattern = $@"\b{pattern}\b";
            var options = _caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;

            string color = EditorGUIUtility.isProSkin ? "#FFDD55" : "#CC6600";
            return Regex.Replace(line, pattern,
                m => $"<color={color}><b>{m.Value}</b></color>", options);
        }
        catch
        {
            return line;
        }
    }

    private void ClearResults()
    {
        _sceneResults.Clear();
        _codeResults.Clear();
        _foldouts.Clear();
        _totalCodeMatches = 0;
        _searched = false;
    }

    private void DrawDivider()
    {
        var rect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
        EditorGUILayout.Space(2);
    }
}
