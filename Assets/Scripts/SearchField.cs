using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
[RequireComponent(typeof(InputField))]
public class SearchField : MonoBehaviour {
    ComboBox cb;
    [SerializeField]
    private int _minCharsToShow = 2;

    [SerializeField]
    string[] _items;
    public string[] Items {
        get {
            return _items;
        }

        set {
            _items = value;
        }
    }

    public UnityAction<string> OnSelect {
        get {
            return onSelect;
        }

        set {
            onSelect = value;
        }
    }

    public int MinCharsToShow {
        get {
            return _minCharsToShow;
        }

        set {
            _minCharsToShow = value;
        }
    }

    int _lastLength = int.MaxValue; 
    string[] _selectedItems = new string[0];
    [SerializeField]
    private UnityAction<string> onSelect;
    
    void Start () {
        if(onSelect == null) {
            onSelect = delegate (string x) {
                Debug.Log(x);
            };
        }
        cb = GetComponentInChildren<ComboBox>();
        Debug.Log(cb);
        cb.Items = (from item in Items
                   select new ComboBoxItem(item))
                   .ToArray();
        GetComponent<InputField>().onEndEdit.AddListener(new UnityAction<string>( x=> {
            //ToggleComboBox(false, false);
            //onSelect(x);
        }));
        GetComponent<InputField>().onValueChanged.AddListener(ShowSimilars);
        cb.OnSelection = new System.Action<string>(x=> {
            GetComponent<InputField>().text = x;
            onSelect(x);
            ToggleComboBox(false, true);
        });
	}
	
	
	void Update () {

    }
    void ShowSimilars(string t) {
        if (t.Length < MinCharsToShow) {
            ToggleComboBox(false, false);
            return;
        }
        ToggleComboBox(true, false);
        t = t.ToLower();
        _selectedItems = (from s in (_lastLength > t.Length ? Items : _selectedItems)
                  where Contains(s.ToLower(), t.Split(' '))
                  orderby s.Length
                  select s).ToArray();
        cb.Items = _selectedItems.Select(x=>new ComboBoxItem(x)).ToArray();
        cb.Refresh();
        _lastLength = t.Length;
    }
    void ToggleComboBox(bool state, bool directClick) {
        if(cb.Active != state) {
            cb.ToggleComboBox(directClick);
        }
    }
    bool Contains(string s, string[] parts) {
        foreach(string p in parts) {
            if (!s.Contains(p)) return false;
        }
        return true;
    }
}
