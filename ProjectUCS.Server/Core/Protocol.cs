﻿using System.Net;
using System.Net.Sockets;
using ProjectUCS.Common;

namespace ProjectUCS.Server.Core;

public class Protocol
{
    private readonly ConnectionPool _connectionPool = new();
    private readonly Socket _socket;

    public Protocol(int port)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socket.Bind(new IPEndPoint(IPAddress.Any, port));
    }

    public event EventHandler<Connection> OnClientConnected
    {
        add => _connectionPool.OnConnectionCreated += value;
        remove => _connectionPool.OnConnectionRemoved -= value;
    }

    public event EventHandler<Connection> OnClientDisconnected
    {
        add => _connectionPool.OnConnectionRemoved += value;
        remove => _connectionPool.OnConnectionRemoved -= value;
    }

    public void Start()
    {
        _socket.Listen(10);
        
        var socketAsyncEventArgs = new SocketAsyncEventArgs();
        socketAsyncEventArgs.Completed += AcceptCompleted;
        _socket.AcceptAsync(socketAsyncEventArgs);

        Console.WriteLine($"Server started on port {_socket.LocalEndPoint}");
    }
    
    private void AcceptCompleted(object? sender, SocketAsyncEventArgs e)
    {
        var socket = e.AcceptSocket;
        _connectionPool.Add(socket!);
        
        var socketAsyncEventArgs = new SocketAsyncEventArgs();
        socketAsyncEventArgs.Completed += AcceptCompleted;
        _socket.AcceptAsync(socketAsyncEventArgs);
    }
}