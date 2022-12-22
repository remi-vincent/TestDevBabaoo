using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TeasingGameTile : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	public Vector2 solutionPos  = Vector2.zero;
	public Vector2 currentPos   = Vector2.zero;

	private TeasingGameManager gameMgr = null;
	public TeasingGameManager GameMgr { set { gameMgr = value; } }

	private Canvas canvas = null;
	private RectTransform rectTransform = null;

	private Vector2 originalPos = Vector2.zero;
	private Vector2 targetPos = Vector2.zero;

	[SerializeField]
	private float minDistToMove = 0;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		TeasingGameTile emptyTile = gameMgr.EmptyTile;

		if ((emptyTile.currentPos - currentPos).sqrMagnitude != 1)
		{
			// Case the user try to move the emptyTile or blocked tile
			originalPos = targetPos = rectTransform.anchoredPosition;
		}
		else
		{
			// Define position limits
			originalPos = rectTransform.anchoredPosition;
			targetPos = emptyTile.rectTransform.anchoredPosition;
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (canvas == null)
			canvas = FindObjectOfType<Canvas>();

		// Use canvas scale in case it isn't equal to 1, 1, 1
		Vector2 updatedPos = rectTransform.anchoredPosition + eventData.delta / canvas.scaleFactor;

		// Clamp position to respect the game movement restrictions
		updatedPos.x = Mathf.Clamp(updatedPos.x, Mathf.Min(originalPos.x, targetPos.x), Mathf.Max(originalPos.x, targetPos.x));
		updatedPos.y = Mathf.Clamp(updatedPos.y, Mathf.Min(originalPos.y, targetPos.y), Mathf.Max(originalPos.y, targetPos.y));

		rectTransform.anchoredPosition = updatedPos;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if ((originalPos - rectTransform.anchoredPosition).magnitude >= minDistToMove)
		{
			Vector2 movement = (targetPos - originalPos).normalized;
			// The panel start at the top so the y coordinate hase to be reverse
			movement.y *= -1;

			gameMgr.MoveTile(this, movement);
		}

		LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
	}

	public void OnPointerDown(PointerEventData eventData)
	{
	}
}