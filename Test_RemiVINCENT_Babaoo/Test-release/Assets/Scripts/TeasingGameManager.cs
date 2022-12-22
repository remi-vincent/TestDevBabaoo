using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeasingGameManager : MonoBehaviour
{
	[Header("GameObjects")]
	[SerializeField]
	private GameObject tilePrefab = null;
	[SerializeField]
	private GameObject panelGo = null;

	[Header("Image")]
	[SerializeField]
	private string androidImgPath = null;
	[SerializeField]
	private string IOSImgPath = null;
	private string imgPath = null;

	[Header("Puzzle")]
	[SerializeField]
	private Vector2 emptyTileStartPos = new Vector2(1, 1);
	private TeasingGameTile emptyTile = null;
	public TeasingGameTile EmptyTile { get { return emptyTile; } }

	private int nbRow = 3;
	private int nbColumn = 3;
	private int completionState = 0;

	[SerializeField]
	private int mixUpDepth = 2;

	[Header("Texts")]
	[SerializeField]
	private GameObject timesUpTextGo = null;
	[SerializeField]
	private GameObject youWinTextGo = null;

	// Start is called before the first frame update
	void Start()
	{
		if (tilePrefab == null || panelGo == null)
		{
			Debug.LogError("Missing GameObject in GameManager");
			return;
		}

		completionState = nbRow * nbColumn;
		imgPath = androidImgPath;

#if UNITY_IPHONE
		imgPath = IOSImgPath;
#endif
#if UNITY_ANDROID
		imgPath = androidImgPath;
#endif

		InitTilesPanel();
		MixeUpPuzzle();
	}

	void InitTilesPanel()
	{
		for (int i = 0; i < nbRow * nbColumn; i++)
		{
			GameObject newTileGo = Instantiate(tilePrefab, panelGo.transform);

			// Compute the tilePos in X, Y coordinates
			Vector2 tilePos = new Vector2(i % nbColumn, i / nbColumn);

			TeasingGameTile tile = newTileGo.GetComponent<TeasingGameTile>();

			// Init position values
			if (tile != null)
			{
				tile.currentPos = tile.solutionPos = tilePos;

				Image img = newTileGo.GetComponent<Image>();
				
				if (tilePos != emptyTileStartPos)
				{
					// Set sprite depending on the tile position
					if (img != null)
						img.sprite = Resources.Load<Sprite>(imgPath + (i + 1));
				}
				else
				{
					if (img != null)
						img.color = Color.clear;

					emptyTile = tile;
				}

				tile.GameMgr = this;
			}
		}
	}

	void MixeUpPuzzle()
	{
		int maxId = nbRow * nbColumn - 1;
		int emptyTileID = (int)(emptyTile.currentPos.x + nbColumn * emptyTile.currentPos.y);

		// To be solvable the number of tile swap must be even
		mixUpDepth += mixUpDepth % 2;

		for (int i = 0; i < mixUpDepth; ++i)
		{
			int tileID = Random.Range(0, maxId);

			// Avoid to swap the emptyTile
			if (tileID == emptyTileID)
				tileID = (tileID + 1) % maxId;

			// range btw 1 and maxId - 1 to avoid getting the same tile
			int tileSwapID = (tileID + Random.Range(1, maxId - 1)) % maxId;

			// Avoid to swap the emptyTile
			if (tileSwapID == emptyTileID)
				tileSwapID = (tileSwapID + 1) % maxId;

			SwapTiles(panelGo.transform.GetChild(tileID).GetComponent<TeasingGameTile>(),
					panelGo.transform.GetChild(tileSwapID).GetComponent<TeasingGameTile>(),
					false);
		}
	}

	public void MoveTile(TeasingGameTile tile, Vector2 movement)
	{
		if (tile.currentPos + movement == emptyTile.currentPos)
		{
			SwapTiles(tile, emptyTile);
		}
	}

	void SwapTiles(TeasingGameTile tile_1, TeasingGameTile tile_2, bool checkCompletion = true)
	{
		// Swap currentPos
		Vector2 posTile_2 = tile_2.currentPos;
		tile_2.currentPos = tile_1.currentPos;
		tile_1.currentPos = posTile_2;

		// Test if the tile_1 was moved form his solutionPos or arrived on his solution pos
		if (tile_2.currentPos == tile_1.solutionPos)
			completionState--;
		if (tile_1.currentPos == tile_1.solutionPos)
			completionState++;

		// Same for tile_2
		if (tile_1.currentPos == tile_2.solutionPos)
			completionState--;
		if (tile_2.currentPos == tile_2.solutionPos)
			completionState++;

		// Swap tiles index in the hierarchy to update the tiles position in the panel
		int tile_1Index = tile_1.transform.GetSiblingIndex();
		int tile_2Index = tile_2.transform.GetSiblingIndex();

		tile_1.transform.SetSiblingIndex(tile_2Index);
		tile_2.transform.SetSiblingIndex(tile_1Index);

		if (checkCompletion)
			CheckPuzzleCompletion();
	}

	void CheckPuzzleCompletion()
	{
		if (completionState == nbColumn * nbRow)
		{
			Timer timer = GetComponent<Timer>();

			if (timer != null)
			{
				timer.Stop();
				float time = timer.ElapsedTime; // TODO : Save
				youWinTextGo.SetActive(true);
				Destroy(panelGo);
			}
		}
	}

	public void TimesUp()
	{
		timesUpTextGo.SetActive(true);
		Destroy(panelGo);
	}
}
