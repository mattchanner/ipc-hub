# ipc-hub

This project is a messaging library to provide inter-process communication to .NET applications.

At the moment, the main transport mechanism uses named pipes, so will only work for processes running on the same machine, but could be extended to provide a socket based transport mechanism in future.

The library works with a single server (hub) and multiple clients.  To create the hub message bus, simply do the following:

```cs
ICompositeServerMessageBus bus = new CompositeServerMessageBus();
IServerMessageBus serverBus = bus.GetOrCreate(clientId);
```

Likewise, clients of the hub can be created in the following way:

```cs
// clientId is passed to the client process and is known by the 'server'
IClientMessageBus clientBus = new ClientMessageBus(clientId);
```

Messages sent on the bus are strongly typed.  Subscribing to a message of a given type can be done in the following way:

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

The value returned from the Subscribe method implements the IDispoable interface.  When Dispose is called on this instance, the client will be unsubscribed from the message.

```cs
IDisposable unsubscriber = bus.Subscribe<MyMessage>((s, e) => Console.WriteLine(e.Message.Text));

// remove handler from bus
unsubscriber.Dispose();
```
