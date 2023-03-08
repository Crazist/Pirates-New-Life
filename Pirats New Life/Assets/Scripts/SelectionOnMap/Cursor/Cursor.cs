using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cursor : MonoBehaviour
{
    [SerializeField] private LayerMask _raycastLayerMask;

    private List<IMoveable> _selectedUnits = new List<IMoveable>();

    private RaycastHit _raycastHit;
    private Vector3 _dragStartPosition;

    private bool _isDraggingMouseBox;

    private void Update()
    {
        // LMB
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _raycastHit, Mathf.Infinity, _raycastLayerMask, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject())
            {
                _isDraggingMouseBox = true;
                _dragStartPosition = Input.mousePosition;

                if (_raycastHit.transform.TryGetComponent(out ISelectable selectable))
                {
                    Events.Cursor.onDeselect?.Invoke();
                    selectable.OnSelect();
                }
                else
                {
                    Events.Cursor.onDeselect?.Invoke();
                }

                _selectedUnits.Clear();
                if (_raycastHit.transform.TryGetComponent(out IMoveable moveable))
                {
                    _selectedUnits.Add(moveable);
                }
            }
        }

        // LMB
        if (Input.GetMouseButtonUp(0))
        {
            _isDraggingMouseBox = false;
        }

        // RMB
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _raycastHit, Mathf.Infinity, _raycastLayerMask, QueryTriggerInteraction.Ignore) && _selectedUnits.Count > 0)
            {
                for (int i = 0; i < _selectedUnits.Count; i++)
                {
                    _selectedUnits[i].MoveToPos(_raycastHit);
                }
            }
        }

        if (_isDraggingMouseBox && _dragStartPosition != Input.mousePosition)
        {
            SelectUnitsInDraggingBox();
        }
    }

    private void SelectUnitsInDraggingBox()
    {
        Bounds selectionBounds = SelectionBoxUI.GetViewportBounds(Camera.main, _dragStartPosition, Input.mousePosition);

        foreach (GameObject unit in UnitsManager.Instance.GetUnitsList())
        {
            bool inBounds = selectionBounds.Contains(Camera.main.WorldToViewportPoint(unit.transform.position));
            if (inBounds)
            {
                if (unit.TryGetComponent(out ISelectable selectable))
                {
                    selectable.OnSelect();
                }

                if (unit.TryGetComponent(out IMoveable moveable))
                {
                    _selectedUnits.Add(moveable);
                }
            }
        }
    }

    private void OnGUI()
    {
        if (_isDraggingMouseBox)
        {
            var rect = SelectionBoxUI.GetScreenRect(_dragStartPosition, Input.mousePosition);
            SelectionBoxUI.DrawScreenRect(rect, new Color(0.5f, 1f, 0.4f, 0.2f));
            SelectionBoxUI.DrawScreenRectBorder(rect, 1, new Color(0.5f, 1f, 0.4f));
        }
    }
}
