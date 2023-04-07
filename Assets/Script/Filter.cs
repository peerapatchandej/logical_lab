using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Filter : MonoBehaviour
{
    [SerializeField] private Dropdown filterdropdown;
    [SerializeField] private InputField input;
    [SerializeField] private Button search;
    [SerializeField] private GameObject warningTxt;
    [SerializeField] private OpenWindow openWindow;
    [SerializeField] private GameObject UserList;

    // Use this for initialization
    void Start ()
    {
        filterdropdown.onValueChanged.AddListener(FilterData);
        search.onClick.AddListener(SearchData);
    }

    private void FilterData(int value)
    {
        if (value == 1) FilterAToZ();
        else if (value == 2) FilterZToA();
        else if (value == 3) FilterNewToOld();
        else if (value == 4) FilterOldToNew();
        else if (value == 5) FilterChecked();
        else if (value == 6) FilterUnchecked();
    }

    private void FilterAToZ()
    {
        ActiveAll();
        openWindow.listName.Sort();

        for (int x = 0; x < openWindow.listName.Count; x++)
        {
            for (int i = 1; i < UserList.transform.childCount; i++)
            {
                var list = UserList.transform.GetChild(i);
                var name = list.GetChild(0).GetComponent<Text>().text;

                if (name == openWindow.listName[x]) list.SetSiblingIndex(x + 1);
            }
        }
    }

    private void FilterZToA()
    {
        ActiveAll();
        openWindow.listName.Sort();

        for (int x = openWindow.listName.Count - 1; x >= 0; x--)
        {
            for (int i = 1; i < UserList.transform.childCount; i++)
            {
                var list = UserList.transform.GetChild(i);
                var name = list.GetChild(0).GetComponent<Text>().text;

                if (name == openWindow.listName[x]) list.SetSiblingIndex(openWindow.listName.Count - x);
            }
        }
    }

    private void FilterNewToOld()
    {
        ActiveAll();
        List<DateTime> DateList = new List<DateTime>();
        ConvertStringToDate(DateList);
        var SortList = SortDateDescending(DateList);
        SortDateObject(ConvertDateToString(SortList));
    }

    private void FilterOldToNew()
    {
        ActiveAll();
        List<DateTime> DateList = new List<DateTime>();
        ConvertStringToDate(DateList);
        var SortList = SortDateAscending(DateList);
        SortDateObject(ConvertDateToString(SortList));
    }

    private void ConvertStringToDate(List<DateTime> DateList)
    {
        for (int i = 0; i < openWindow.listDate.Count; i++)
        {
            string DateStr = openWindow.listDate[i];
            DateTime Date;
            DateTime.TryParse(DateStr, out Date);
            DateList.Add(Date);
        }
    }

    private List<string> ConvertDateToString(List<DateTime> SortList)
    {
        List<string> DateStr = new List<string>();

        for(int i = 0; i < SortList.Count; i++)
        {
            DateStr.Add(SortList[i].ToString("yyyy-MM-dd HH:mm:ss"));
        }

        return DateStr;
    }

    static List<DateTime> SortDateAscending(List<DateTime> list)
    {
        list.Sort((a, b) => a.CompareTo(b));
        return list;
    }

    static List<DateTime> SortDateDescending(List<DateTime> list)
    {
        list.Sort((a, b) => b.CompareTo(a));
        return list;
    }

    private void SortDateObject(List<string> DateStr)
    {
        for (int x = 0; x < DateStr.Count; x++)
        {
            for (int i = 1; i < UserList.transform.childCount; i++)
            {
                var list = UserList.transform.GetChild(i);
                var date = list.GetChild(1).GetComponent<Text>().text;

                if (date == DateStr[x]) list.SetSiblingIndex(x + 1);
            }
        }
    }

    private void SearchData()
    {
        if (input.text != "")
        {
            ActiveAll();
            warningTxt.SetActive(false);

            for (int i = 1; i < UserList.transform.childCount; i++)
            {
                var list = UserList.transform.GetChild(i);
                var name = list.GetChild(0).GetComponent<Text>().text;

                if (name.ToLower().IndexOf(input.text.ToLower()) == -1) list.gameObject.SetActive(false);
            }
        }
        else warningTxt.SetActive(true);
    }

    private void FilterChecked()
    {
        ActiveAll();

        for (int i = 1; i < UserList.transform.childCount; i++)
        {
            var list = UserList.transform.GetChild(i);
            var check = list.GetChild(2).gameObject;

            if (check.activeSelf == false) list.gameObject.SetActive(false);
        }
    }

    private void FilterUnchecked()
    {
        ActiveAll();

        for (int i = 1; i < UserList.transform.childCount; i++)
        {
            var list = UserList.transform.GetChild(i);
            var check = list.GetChild(2).gameObject;

            if (check.activeSelf) list.gameObject.SetActive(false);
        }
    }

    private void ActiveAll()
    {
        for (int i = 1; i < UserList.transform.childCount; i++)
        {
            UserList.transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
