using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
///A manager for all events 
/// </summary>
public class EventPool : MonoBehaviour {
	
	public string[] eventNames;
	private Dictionary <string, GameEvent> _eventDictionary = new Dictionary<string, GameEvent>();

	void Awake () {
        for (int i = 0; i < eventNames.Length; i++)
            AddEventToPool(eventNames[i], new GameEvent());
	}
    
	void Update () {
		foreach (string s in _eventDictionary.Keys)
			_eventDictionary [s].CheckEventTriggered ();
	}

	//Adds a game event to the collection of events if it does not exist
	//or REPLACES the event with the Key if it does exist
	public void AddEventToPool(string key, GameEvent gameEvent)
	{
		if (!_eventDictionary.ContainsKey (key))
			_eventDictionary.Add (key, gameEvent);
		else
			_eventDictionary [key] = gameEvent;
	}

	//Adds a game event to the collection of events if it does not exist
	//or UPDATES the event with the Key if it does exist
	public void AddEventToPool(string key, OnHandleEvent action)
	{
		if (_eventDictionary.ContainsKey(key))
			_eventDictionary[key].onHandleEvent += action;
		else
		{
			var gameEvent = new GameEvent();
			gameEvent.onHandleEvent += action;

			AddEventToPool(key, gameEvent);
		}
	}
	
	public void RemoveEventFromPool(string key)
	{
		if (_eventDictionary.ContainsKey(key))
			_eventDictionary.Remove(key);
	}

	//Out's a game event if it exists otherwise it will out null
	public void GetEventFromPool(string key, out GameEvent gameEvent)
	{
		if (_eventDictionary.ContainsKey (key))
			gameEvent = _eventDictionary [key];
		else
			gameEvent = null;
	}

	//Adds a trigger to the event of your choice
	public void AddTriggerToEvent(string key, Trigger trigger)
	{
		if (_eventDictionary.ContainsKey (key)) {
			_eventDictionary [key].AddEventTrigger (trigger);
			Debug.Log (trigger.ToString () + " added to " + key);
		} else
			Debug.LogWarning (key + " is not registared in the event pool");
	}
	
	public void RemoveTriggerToEvent(string key, Trigger trigger)
	{
		if (_eventDictionary.ContainsKey (key)) {
			_eventDictionary [key].RemoveEventTrigger (trigger);
			Debug.Log (trigger.ToString () + " removed from" + key);
		} else
			Debug.LogWarning (key + " is not registared in the event pool");
	}
	
	public void RemoveTriggerToEvent(Trigger trigger)
	{
		if (_eventDictionary.ContainsKey (trigger.eventKey)) {
			_eventDictionary [trigger.eventKey].RemoveEventTrigger (trigger);
			Debug.Log (trigger.ToString () + " removed from" + trigger.eventKey);
		} else
			Debug.LogWarning (trigger.eventKey + " is not registared in the event pool");
	}

	public void ClearEvents()
	{
		_eventDictionary.Clear();
	}

	public bool HasEvent(string key)
	{
		return _eventDictionary.ContainsKey(key);
	}
}
