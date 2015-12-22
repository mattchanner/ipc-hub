# ipc-hub

This project is a messaging library to provide inter-process communication to .NET applications.

At the moment, the main transport mechanism uses named pipes, so will only work for processes running on the same machine, but could be extended to provide a socket based transport mechanism in future.

The library works with a single server (hub) and multiple clients.  To create the message bus on the server, simply do the following:

```cs
ICompositeServerMessageBus bus = new CompositeServerMessageBus();
IServerMessageBus serverBus = bus.GetOrCreate(clientId);
```

Likewise, clients can be created with the following:

```cs
// clientId is passed to the client process and is known by the 'server'
IClientMessageBus clientBus = new ClientMessageBus(clientId);
```

Each message bus is strongly typed based on the type of message to be published or subscribed.

Subscribing to a message of a given type can be done in the following way:

```cs
class MyMessage {
   public string Text { get; set; }
}

void HandleMessage(object sender, MessageReceivedEventArgs<MyMessage> args) {
    Console.WriteLine(args.Message.Text);
}

bus.Subscribe<MyMessage>(HandleMessage);

```

Messages are published by calling Publish on the message bus:

```cs
bus.Publish(new MyMessage { Text = "Hello World" });
```
