using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Application1.Models
{
    public abstract class Device : Model<Device>
    {
        // Errors

        public sealed class MaxConnectionsError : Exception
        {
            public MaxConnectionsError() : base("Максимальное количество соединений не должно быть меньше текущего.")
            {
            }
        }
        public abstract class ConnectionError : Exception
        {
            protected ConnectionError(string message) : base(message)
            {
            }
        }
        public sealed class NoConnectionError : ConnectionError
        {
            public NoConnectionError() : base("Должно быть свободное соединение.")
            {
            }
        }
        public sealed class ConnectedError : ConnectionError
        {
            public ConnectedError() : base("Устройства не должны быть уже соединены.")
            {
            }
        }
        public sealed class DisconnectedError : ConnectionError
        {
            public DisconnectedError() : base("Устройства должны быть соединены.")
            {
            }
        }

        // Data

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
                if (value < Connections.Count) // Если новое максимальное количество соединений меньше текущего, то ошибка, так как непонятно, какие отключить.
                    throw new MaxConnectionsError();
                __max_connections = value;
                _OnPropertyChanged(nameof(MaxConnections));
            }
        }
        readonly ObservableCollection<Device> __connections = new(); // Используем ObservableCollection, чтобы автоматически сообщать WPF об изменении элементов.
        public IReadOnlyList<Device> Connections => __connections;
        public bool IsConnected(Device destination)
            => __connections.Contains(destination);

        // Behavior: mutable methods

        public delegate void ConnectDisconnectHandler(Device source, Device destination);
        // Пусть модель сообщает о соединении.
        public static event ConnectDisconnectHandler? OnConnect;
        void __PreConnect(Device destination)
        {
            if (__connections.Count == MaxConnections) // Если текущее количество соединений равно максимальному, то ошибка.
                throw new NoConnectionError();
            if (IsConnected(destination)) // Если модели уже соединены, то ошибка.
                throw new ConnectedError();
        }
        // Одностороннее соединение. Применяется только с обратным.
        void __Connect(Device destination)
            => __connections.Add(destination);
        // Двусторонее соединение.
        public void Connect(Device destination)
        {
            __PreConnect(destination);
            destination.__PreConnect(this);
            __Connect(destination);
            destination.__Connect(this);
            if (OnConnect != null)
                OnConnect(this, destination);
        }
        // Пусть модель сообщает об отсоединении.
        public static event ConnectDisconnectHandler? OnDisconnect;
        // Односторонее отсоединение. Применяется только с обратным.
        void __Disconnect(Device destination)
        {
            if (!__connections.Remove(destination)) // Если модели не соединены, то ошибка.
                throw new DisconnectedError();
        }
        // Двустороннее отсоединение.
        public void Disconnect(Device destination)
        {
            __Disconnect(destination);
            destination.__Disconnect(this);
            if (OnDisconnect != null)
                OnDisconnect(this, destination);
        }

        // Construction

        public Device(string name) : base(name)
        {
        }

        // Destruction

        // Пусть при удалении все соединения отключатся.
        protected override void _OnDelete()
        {
            foreach (var connection in __connections)
            {
                connection.__Disconnect(this); // Односторонне отсоединяем, так как модель будет удалена, нет смысла изменять её данные.
                if (OnDisconnect != null) // Всё равно уведомляем о двустороннем отсоединении, так как модели больше не соединены, первая будет удалена.
                    OnDisconnect(this, connection);
            }
        }
    }
}
