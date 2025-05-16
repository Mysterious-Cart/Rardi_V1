using System;
using System.Collections.Generic;

namespace CHKS.Services
{
    // Define a delegate for the event
    public delegate void RemoteControlEventHandler(object sender, EventArgs e);


    public class RemoteControlService
    {
        // Event to notify listeners
        public event RemoteControlEventHandler RemoteControlEvent;

        // Listeners can subscribe to this event
        public void AddListener(RemoteControlEventHandler listener)
        {
            RemoteControlEvent += listener;
        }

        public void RemoveListener(RemoteControlEventHandler listener)
        {
            RemoteControlEvent -= listener;
        }

        // Method to fire the event and notify all listeners
        public void FireEvent()
        {
            RemoteControlEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}