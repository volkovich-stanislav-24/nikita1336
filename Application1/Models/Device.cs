using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Application1.Models
{
    public abstract class Device : Model<Device>
    {
        public abstract class ConnectionError : Exception
        {
            public readonly Device from;
            public readonly Device to;

            protected ConnectionError(Device from, Device to)
            {
                this.from = from;
                this.to = to;
            }
        }
        public sealed class CantConnectError : ConnectionError
        {
            public CantConnectError(Device from, Device to) : base(from, to)
            {
            }
        }
        public sealed class CantDiscsonnectError : ConnectionError
        {
            public CantDiscsonnectError(Device from, Device to) : base(from, to)
            {
            }
        }

        bool __is_on;
        public bool IsOn
        {
            get => __is_on;
            set
            {
                __is_on = value;
                _OnPropertyChanged(nameof(IsOn));
            }
        }
        ushort __max_connections;
        public ushort MaxConnections
        {
            get => __max_connections;
            set
            {
                __max_connections = value;
                _OnPropertyChanged(nameof(MaxConnections));
            }
        }
        readonly IList<Device> __connections = new ObservableCollection<Device>();
        public IList<Device> Connections => __connections;

        public bool IsConnected(Device destination)
        {
            foreach (var connection in __connections)
                if (connection == destination)
                    return true;
            return false;
        }
        public bool CanConnect(Device destination)
            => __connections.Count + 1 <= MaxConnections && !IsConnected(destination);
        public delegate void ConnectHandler(Device first, Device second);
        public static event ConnectHandler? OnConnect;
        // Одностороннее соединение.
        public void __Connect(Device device)
        {
            if (!CanConnect(device))
                throw new CantConnectError(this, device);
            __connections.Add(device);
        }
        // Двусторонее соединение.
        public void Connect(Device device)
        {
            __Connect(device);
            device.__Connect(this);
            if (OnConnect != null)
                OnConnect(this, device);
        }
        public delegate void DisconnectHandler(Device first, Device second);
        public static event DisconnectHandler? OnDisconnect;
        // Односторонее отсоединение.
        void __Disconnect(Device device)
        {
            if (!__connections.Remove(device))
                throw new CantDiscsonnectError(this, device);
        }
        // Двустороннее отсоединение.
        public void Disconnect(Device device)
        {
            __Disconnect(device);
            device.__Disconnect(this);
            if (OnDisconnect != null)
                OnDisconnect(this, device);
        }

        public Device(string name) : base(name)
        {
        }
        public delegate void DeleteHandler(Device device);
        public static event DeleteHandler? OnDelete;
        protected override void _OnDelete()
        {
            for (int i = __connections.Count - 1; i >= 0; --i)
                __connections[i].Disconnect(this);
            if (OnDelete != null)
                OnDelete(this);
        }
    }
}
