using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Main : MonoBehaviour
{
	static GameObject instanceHolder = null;
	static Main instance;

	public static Main S
	{
		get
		{
			if (instanceHolder == null)
			{
				instanceHolder = new GameObject("Main Holder");
				DontDestroyOnLoad(instanceHolder);
				instance = instanceHolder.AddComponent<Main>();
			}
			return instance;
		}
	}

	int _health = 0;
	public int health
	{
		get { return _health; }
		set
		{
			_health = value;
			InGameUI.S.UpdateHealth();
		}
	}

	int _flies = 0;
	public int flies
	{
		get { return _flies; }
		set
		{
			_flies = value;
			InGameUI.S.UpdateFlies();
		}
	}

	int _carbon = 0;
	public int carbon
	{
		get { return _carbon; }
		set
		{
			_carbon = value;
			InGameUI.S.UpdateCarbon();
		}
	}

	int _lithium = 0;
	public int lithium
	{
		get { return _lithium; }
		set
		{
			_lithium = value;
			InGameUI.S.UpdateLithium();
		}
	}

	public void Update() {
	}
}
